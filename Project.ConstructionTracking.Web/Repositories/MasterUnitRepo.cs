using System;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MUnitModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IMasterUnitRepo
	{
		dynamic GetProjectList();
        dynamic GetUnitTypeList();
        dynamic GetModelTypeInProjectList(Guid projectID);
        dynamic GetProjectCompanyVendor(Guid projectID);
        dynamic GetProjectId(Guid unitID);

        dynamic GetUnitList(DTParamModel param, MasterUnitModel criteria);

        dynamic CreateUnit(CreateUnitModel model);
        dynamic EditUnit(EditUnitModel model);
        dynamic DeleteUnit(Guid unitID, Guid requestUserID);

        dynamic GetPEFromProject(Guid projectID);
        bool ActionMappingPE(ActionMappingPeModel model, Guid requestUserID);
    }

	public class MasterUnitRepo : IMasterUnitRepo
	{
        private readonly ContructionTrackingDbContext _context;

        public MasterUnitRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

		public dynamic GetProjectList()
		{
			var query = (from p in _context.tm_Project
						 where p.FlagActive == true
						 select new
						 {
							 p.ProjectID,
							 p.ProjectName
						 }).ToList();
			return query;
		}

        public dynamic GetUnitTypeList()
        {
            var query = (from ut in _context.tm_UnitType
                         where ut.FlagActive == true
                         select new
                         {
                             ut.ID,
                             ut.Name
                         }).ToList();

            return query;
        }

        public dynamic GetModelTypeInProjectList(Guid projectID)
        {
            var query = (from mt in _context.tm_ModelType
                         where mt.ProjectID == projectID && mt.FlagActive == true
                         select new
                         {
                             mt.ID,
                             mt.ModelName
                         }).ToList();

            return query;
        }

		public dynamic GetUnitList(DTParamModel param, MasterUnitModel criteria)
		{
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            //variable = (condition) ? expressionTrue :  expressionFalse;
            criteria.strSearch = (criteria.strSearch == null) ? string.Empty : criteria.strSearch.ToString() ?? string.Empty;

            var query = from u in _context.tm_Unit
                        join p in _context.tm_Project on u.ProjectID equals p.ProjectID
                        join ut in _context.tm_UnitType on u.UnitTypeID equals ut.ID into utGroup
                        from ut in utGroup.DefaultIfEmpty()
                        join mt in _context.tm_ModelType on u.ModelTypeID equals mt.ID
                        join ext in _context.tm_Ext on u.UnitStatusID equals ext.ID
                        join v in _context.tm_Vendor on u.VendorID equals v.ID into vGroup
                        from v in vGroup.DefaultIfEmpty()
                        join pu in _context.tr_PE_Unit on u.UnitID equals pu.UnitID into puGroup
                        from pu in puGroup.DefaultIfEmpty()
                        join us in _context.tm_User on pu.UserID equals us.ID into usGroup
                        from us in usGroup.DefaultIfEmpty()
                        where u.FlagActive == true && p.FlagActive == true
                        select new
                        {
                            p.ProjectID,
                            p.ProjectCode,
                            p.ProjectName,
                            u.UnitID,
                            u.UnitCode,
                            u.UnitTypeID,
                            UnitTypeName = ut.Name,
                            UnitAddress = u.AddreessNo,
                            UnitArea = u.Area,
                            UnitStatus = u.UnitStatusID,
                            UnitStatusDesc = ext.Name,
                            UnitVendor = u.VendorID,
                            UnitVendorName = v.Name,
                            UnitPO = u.PONo,
                            UnitStartDate = u.StartDate,
                            UnitEndDate = u.EndDate,
                            u.UpdateDate,
                            us.FirstName,
                            us.LastName
                        };

            if (!string.IsNullOrEmpty(criteria.strSearch))
            {
                query = query.Where(o =>
                    o.ProjectCode.Contains(criteria.strSearch) ||
                    o.ProjectName.Contains(criteria.strSearch) ||
                    o.UnitCode.Contains(criteria.strSearch) ||
                    o.UnitTypeName.Contains(criteria.strSearch) ||
                    o.UnitAddress.Contains(criteria.strSearch) ||
                    o.UnitStatusDesc.Contains(criteria.strSearch) ||
                    o.UnitPO.Contains(criteria.strSearch)
                );
            }

            if (criteria.ProjectID != Guid.Empty)
            {
                query = query.Where(o => o.ProjectID == criteria.ProjectID);
            }

            var result = query.Page(param.start, param.length, i => i.UpdateDate, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;

            return result.AsEnumerable().Select(e => new
            {
                ProjectID = e.ProjectID,
                ProjectCode = e.ProjectCode,
                ProjectName = e.ProjectName,
                UnitID = e.UnitID,
                UnitCode = e.UnitCode,
                UnitTypeID = e.UnitTypeID,
                UnitTypeName = e.UnitTypeName,
                UnitAddress = e.UnitAddress,
                UnitArea = e.UnitArea,
                UnitStatus = e.UnitStatus,
                UnitStatusDesc = e.UnitStatusDesc,
                UnitVendor = e.UnitVendor,
                UnitVendorName = e.UnitVendorName,
                UnitPO = e.UnitPO,
                UnitStartDate = e.UnitStartDate.ToStringDate(),
                UnitEndDate = e.UnitEndDate.ToStringDate(),
                UpdateDate = e.UpdateDate.ToStringDateTime(),
                UserName = e.FirstName + " " + e.LastName
            }).ToList();
        }

        public dynamic CreateUnit(CreateUnitModel model)
        {
            tm_Unit createUnit = new tm_Unit();
            createUnit.UnitID = Guid.NewGuid();
            createUnit.ProjectID = model.ProjectID;
            createUnit.ModelTypeID = model.ModelTypeID;
            createUnit.UnitTypeID = model.UnitTypeID;
            createUnit.UnitCode = model.UnitCode;
            createUnit.AddreessNo = model.UnitAddress;
            createUnit.Area = model.UnitArea;
            createUnit.TitledeedArea = model.UnitArea;
            createUnit.UnitStatusID = SystemConstant.Unit_Status.FREE;
            createUnit.FlagActive = true;
            createUnit.CreateBy = model.RequestUserID; 
            createUnit.CreateDate = DateTime.Now;
            createUnit.UpdateBy = model.RequestUserID;
            createUnit.UpdateDate = DateTime.Now;

            _context.tm_Unit.Add(createUnit);
            _context.SaveChanges();

            return createUnit;
        }

        public dynamic EditUnit(EditUnitModel model)
        {
            tm_Unit? edit = _context.tm_Unit.Where(o => o.UnitID == model.UnitID && o.FlagActive == true).FirstOrDefault();
            if (edit == null) throw new Exception("ไม่พบข้อมูลของยูนิต");

            edit.CompanyVendorID = model.CompanyVendorID;
            edit.PONo = model.PONo;
            edit.StartDate = model.StartDate;
            edit.UpdateDate = DateTime.Now;
            edit.UpdateBy = model.RequestUserID;

            int sumDuration = 0;

            tr_ProjectModelForm? modelForm = _context.tr_ProjectModelForm
                                            .Where(o => o.ProjectID == edit.ProjectID
                                            && o.ModelTypeID == edit.ModelTypeID
                                            && o.FlagActive == true).FirstOrDefault();
            if (modelForm == null) throw new Exception("ไม่พบข้อมูลโมเดลฟอร์ม");

            tm_FormType? formType = _context.tm_FormType
                                    .Where(o => o.ID == modelForm.FormTypeID && o.FlagActive == true).FirstOrDefault();
            if (formType == null) throw new Exception("ไม่พบข้อมูลประเภทฟอร์ม");

            List<tm_Form>? form = _context.tm_Form.Where(o => o.FormTypeID == formType.ID && o.FlagActive == true).ToList();
            if (form == null) throw new Exception("ไม่พบข้อมูลฟอร์ม");

            foreach (var data in form)
            {
                sumDuration += (int)data.DurationDay;
            }

            edit.EndDate = model.StartDate.AddDays(sumDuration);

            _context.tm_Unit.Update(edit);
            _context.SaveChanges();

            return edit;
        }

        public dynamic GetProjectCompanyVendor(Guid projectID)
        {
            var query = (from cvp in _context.tr_CompanyVendorProject
                         join mcv in _context.tm_CompanyVendor on cvp.CompanyVendorID equals mcv.ID
                         where cvp.ProjectID == projectID && cvp.FlagActive == true
                         && mcv.FlagActive == true
                         select new
                         {
                             mcv.ID,
                             mcv.Name
                         }).ToList();

            return query;
        }

        public dynamic GetProjectId(Guid unitID)
        {
            var query = (from u in _context.tm_Unit
                         where u.UnitID == unitID && u.FlagActive == true
                         select new
                         {
                             u.UnitID,
                             u.ProjectID,
                             u.CompanyVendorID,
                             u.PONo,
                             u.StartDate
                         }).FirstOrDefault();

            return query;
        }

        public dynamic DeleteUnit(Guid unitID, Guid requestUserID)
        {
            bool verify = ValidateDeleteUnit(unitID);
            if (!verify) throw new Exception("ข้อมูลยูนิตถูกใช้งานแล้ว");

            tm_Unit? delete = _context.tm_Unit.Where(o => o.UnitID == unitID && o.FlagActive == true).FirstOrDefault();
            if (delete == null) throw new Exception("ไม่พบข้อมูลยูนิต");

            delete.FlagActive = false;
            delete.UpdateDate = DateTime.Now;
            delete.UpdateBy = requestUserID;

            _context.tm_Unit.Update(delete);
            _context.SaveChanges();

            return delete;
        }

        private bool ValidateDeleteUnit(Guid unitID)
        {
            bool flag = false;

            tm_Unit? valid = _context.tm_Unit.Where(o => o.UnitID == unitID && o.FlagActive == true).FirstOrDefault();
            if (valid == null) throw new Exception("ไม่พบข้อมูลยูนิต");

            if (valid.StartDate == null || valid.PONo == null || valid.VendorID == null) flag = true;

            return flag;
        }

        public dynamic GetPEFromProject(Guid projectID)
        {
            var query = (from pp in _context.tr_ProjectPermission
                        join us in _context.tm_User on pp.UserID equals us.ID
                        where pp.ProjectID == projectID && pp.FlagActive == true
                        && us.RoleID == SystemConstant.UserRole.PE
                        select new
                        {
                            UserID = us.ID,
                            FirstName = us.FirstName,
                            LastName = us.LastName
                        }).ToList();

            return query;
        }

        public bool ActionMappingPE(ActionMappingPeModel model, Guid RequestUserID)
        {
            foreach (var data in model.ListUnitID)
            {
                tr_PE_Unit? peUnit = _context.tr_PE_Unit
                                    .Where(o => o.UnitID == data && o.FlagActive == true)
                                    .FirstOrDefault();

                if (peUnit == null)
                {
                    tr_PE_Unit createMapping = new tr_PE_Unit();
                    createMapping.UnitID = data;
                    createMapping.UserID = model.UserID;
                    createMapping.FlagActive = true;
                    createMapping.CreateBy = RequestUserID;
                    createMapping.CreateDate = DateTime.Now;
                    createMapping.UpdateBy = RequestUserID;
                    createMapping.UpdateDate = DateTime.Now;

                    _context.tr_PE_Unit.Add(createMapping);
                    _context.SaveChanges();
                }
                else if(peUnit.UserID != model.UserID)
                {
                    peUnit.UserID = model.UserID;
                    peUnit.UpdateBy = RequestUserID;
                    peUnit.UpdateDate = DateTime.Now;

                    _context.tr_PE_Unit.Update(peUnit);
                    _context.SaveChanges();
                }
            }
            return true;
        }
    }
}

