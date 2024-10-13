using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class GetDDLRepo : IGetDDLRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public GetDDLRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<GetDDL> GetDDLList(GetDDL Model)
        {
            switch (Model.Act)
            {
                case "Ext":
                    var extQuery = from ext in _context.tm_Ext
                                   where ext.ExtTypeID == Model.ID
                                   orderby ext.LineOrder
                                   select new GetDDL
                                   {
                                       Value = ext.ID,
                                       Text = ext.Name
                                   };

                return extQuery.ToList();

                case "Vender":
                    var result = from t1 in _context.tm_CompanyVendor
                                 join t2 in _context.tr_CompanyVendor
                                     on new { CompanyVendorID = (int?)t1.ID, FlagActive = (bool?)true } equals new { t2.CompanyVendorID,t2.FlagActive } into gj1
                                 from t2 in gj1.DefaultIfEmpty()
                                 join t3 in _context.tm_Vendor
                                     on new { t2.VendorID, FlagActive = (bool?)true } equals new { VendorID = (int?)t3.ID, t3.FlagActive } into gj2
                                 from t3 in gj2.DefaultIfEmpty()
                                 where t1.ID == Model.ID && t1.FlagActive == true
                                 orderby t2.VendorID
                                  select new GetDDL
                                  {
                                      Value = t3.ID,
                                      Text = t3.Name
                                  };

                return result.ToList();

                case "Project":
                    var ListProject = from t1 in _context.tm_Project
                                      join t2 in _context.tr_ProjectPermission
                                        on t1.ProjectID equals t2.ProjectID into joined
                                      from t2 in joined.DefaultIfEmpty()
                                      where t1.FlagActive == true && t2.FlagActive == true && t2.UserID == Model.UserID
                                      select new GetDDL
                                      {
                                        ValueGuid = t1.ProjectID,
                                        Text = t1.ProjectName
                                      };

                return ListProject.ToList();

                case "Unit":
                    var ListUnit = from tu in _context.tm_Unit
                                   where tu.FlagActive == true && tu.ProjectID == Model.ValueGuid
                                   orderby tu.UnitCode
                                   select new GetDDL
                                   {
                                       ValueGuid = tu.UnitID,
                                       Text = tu.UnitCode
                                   };

                return ListUnit.ToList();

                case "UnitFormStatus":
                    var ListUnitFormStatus = from tu in _context.tm_UnitFormStatus
                                             where tu.FlagActive == true && tu.ID > 1
                                             orderby tu.LineOrder
                                             select new GetDDL
                                             {
                                                Value = tu.ID,
                                                Text = tu.Name
                                             };

                return ListUnitFormStatus.ToList();

                case "UserName":
                    var UserName = from us in _context.tm_User
                                   where us.ID == Model.ValueGuid
                                   select new GetDDL
                                   {
                                       Text = us.FirstName + " " + us.LastName 
                                   };

                 return UserName.ToList();

                case "ProjectAdmin":
                    var ListProjectAdmint = from t1 in _context.tm_Project
                                      where t1.FlagActive == true 
                                      select new GetDDL
                                      {
                                          ValueGuid = t1.ProjectID,
                                          Text = t1.ProjectName
                                      };

                return ListProjectAdmint.ToList();

                case "DefectArea":
                    var ListDefectArea = from t1 in _context.tm_DefectArea
                                         where t1.FlagActive == true
                                               && t1.ProjectTypeID == Model.ID
                                               && (string.IsNullOrEmpty(Model.searchTerm) || t1.Name.Contains(Model.searchTerm))
                                         orderby t1.Name
                                         select new GetDDL
                                         {
                                             Value = t1.ID,
                                             Text = t1.Name
                                         };

                    return ListDefectArea.ToList();

                case "DefectType":
                    var ListDefectType = from t1 in _context.tm_DefectType
                                         join t2 in _context.tm_DefectAreaType_Mapping
                                         on t1.ID equals t2.DefectTypeID into mappingGroup
                                         from t2 in mappingGroup.DefaultIfEmpty() // Left join
                                         where t1.FlagActive == 1
                                               && t2.DefectAreaID == Model.ID
                                               && (string.IsNullOrEmpty(Model.searchTerm) || t1.Name.Contains(Model.searchTerm))
                                         orderby t1.Name
                                         select new GetDDL
                                         {
                                             Value = t1.ID,
                                             Text = t1.Name
                                         };

                    return ListDefectType.ToList();

                case "DefectDescription":
                    var ListDefectDescription = from t1 in _context.tm_DefectDescription
                                                where t1.FlagActive == 1
                                                      && t1.DefectTypeID == Model.ID
                                                      && (string.IsNullOrEmpty(Model.searchTerm) || t1.Name.Contains(Model.searchTerm))
                                                orderby t1.LineOrder
                                                select new GetDDL
                                                {
                                                    Value = t1.ID,
                                                    Text = t1.Name
                                                };

                    return ListDefectDescription.ToList();

                case "PEUnit":
                    var PEUnit = from t1 in _context.tr_PE_Unit
                                 join t2 in _context.tm_User on t1.UserID equals t2.ID into joined
                                 from t2 in joined.DefaultIfEmpty()
                                 where t1.UnitID == Model.GuID
                                                    
                                select new GetDDL
                                {
                                    ValueGuid = t1.UserID,
                                    Text = t2.FirstName + " " + t2.LastName
                                };

                    return PEUnit.ToList();

                case "ImageQC5Unit":
                    var ImageQC5UnitList = from t1 in _context.tr_QC_UnitCheckList_Resource
                                           join t2 in _context.tm_Resource on t1.ResourceID equals t2.ID into joined
                                 from t2 in joined.DefaultIfEmpty()
                                 where t1.QCUnitCheckListID == Model.GuID && t1.DefectID == null && t1.IsSign == false && t1.FlagActive == true
                                 select new GetDDL
                                 {
                                     ValueGuid = t1.ResourceID,
                                     Text = t2.FilePath
                                 };

                    return ImageQC5UnitList.ToList();

                default:

                return new List<GetDDL>();
            }
        }
    }
}
