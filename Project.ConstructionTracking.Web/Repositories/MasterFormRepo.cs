using System;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MFormModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using PackageModel = Project.ConstructionTracking.Web.Models.MFormModel.PackageModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IMasterFormRepo
	{
		dynamic GetFormTypeList(DTParamModel param, MasterFormModel criteria);
        dynamic GetProjectTypeList();   
        dynamic GetFormTypeDetial(int FormTypeId);
        dynamic ActionFormType(FormTypeModel model);

        dynamic GetFormList(DTParamModel param, int formTypeID);
        dynamic GetFormGroupList(DTParamModel param, int formID);
        dynamic GetFormPackageList(DTParamModel param, int groupID);
        dynamic GetFormCheckList(DTParamModel param, int packageID);
        dynamic GetQcList(int formTypeID);

        dynamic GetFormDetail(int formID);
        dynamic GetGroupDetail(int groupID);
        dynamic GetPackageDetail(int packageID);
        dynamic GetCheckListDetail(int checkID);

        // using for check condition in repo
        //bool VerifyFormTypeUsing(int formTypeID);

        dynamic ActionForm(FormModel model);
        dynamic ActionGroup(GroupModel model);
        dynamic ActionPacakge(PackageModel model);
        dynamic ActionCheckList(CheckListModel model);

    }

	public class MasterFormRepo : IMasterFormRepo
	{
		private readonly ContructionTrackingDbContext _context;

        public MasterFormRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

		public dynamic GetFormTypeList(DTParamModel param, MasterFormModel criteria)
		{
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            //variable = (condition) ? expressionTrue :  expressionFalse;
            criteria.strSearch = (criteria.strSearch == null) ? string.Empty : criteria.strSearch.ToString() ?? string.Empty;

			var query = from ft in _context.tm_FormType
                        join ext in _context.tm_Ext on ft.ProjectTypeID equals ext.ID
						where ft.FlagActive == true
						select new
						{
                            ext.Name,
                            ft.ID,
							FormName = ft.Name,
							ft.Description,
							ft.UpdateDate,
                            
						};

            if (!string.IsNullOrEmpty(criteria.strSearch))
            {
                query = query.Where(o =>
                    o.Name.Contains(criteria.strSearch) ||
                    o.Description.Contains(criteria.strSearch)
                );
            }

            var result = query.Page(param.start, param.length, i => i.UpdateDate , param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;
            return result.AsEnumerable().Select(e => new
            {
                Name = e.Name,
                ID = e.ID,
                FormName = e.FormName,
                Description = e.Description,
                UpdateDate = e.UpdateDate.ToStringDateTime()
            }).ToList();
        }

        public dynamic GetProjectTypeList()
        {
            var query = (from i in _context.tm_Ext
                        where i.ExtTypeID == SystemConstant.Ext_Type.PROJECT_TYPE
                        select new
                        {
                            i.ID,
                            i.Name
                        }).ToList();

            return query;
        }

        public dynamic GetFormTypeDetial(int FormTypeId)
        {
            var query = (from ft in _context.tm_FormType
                         join ext in _context.tm_Ext on ft.ProjectTypeID equals ext.ID
                         where ft.ID == FormTypeId
                         select new
                         {
                             FormTypeId = ft.ID,
                             FormTypeName = ft.Name,
                             ft.Description,
                             ext.ID,
                             ext.Name
                         }).FirstOrDefault();

            return query;
        }

        public dynamic ActionFormType(FormTypeModel model)
        {
            FormTypeResp? data = null;
            if (model.TypeData == FormTypeConstant.Action_Form_Type.CREATE)
            {
                if (!String.IsNullOrWhiteSpace(model.FormTypeName)
                    && !String.IsNullOrWhiteSpace(model.FormTypeDesc)
                    && model.ProjectTypeID > 0)
                {
                    tm_FormType create = new tm_FormType();
                    create.ProjectTypeID = model.ProjectTypeID;
                    create.Name = model.FormTypeName;
                    create.Description = model.FormTypeDesc;
                    create.FlagActive = true;
                    create.CreateDate = DateTime.Now;
                    //Create.CreateBy = ; // wait for permission login
                    create.UpdateDate = DateTime.Now;
                    //Create.UpdateBy = ; // wait for permission login

                    _context.tm_FormType.Add(create);
                    _context.SaveChanges();

                    data = new FormTypeResp()
                    {
                        FormTypeID = create.ID,
                        ProjectTypeID = (int)create.ProjectTypeID,
                        FormTypeName = create.Name,
                        FormTypeDesc = create.Description,
                        FlagActive = create.FlagActive
                    };
                }
            }
            else if (model.TypeData == FormTypeConstant.Action_Form_Type.EDIT)
            {
                if (!String.IsNullOrWhiteSpace(model.FormTypeName)
                    && !String.IsNullOrWhiteSpace(model.FormTypeDesc)
                    && model.ProjectTypeID > 0 && model.FormTypeID > 0)
                {
                    tm_FormType? edit = _context.tm_FormType.Where(o => o.ID == model.FormTypeID
                                                                    && o.FlagActive == true)
                                                            .FirstOrDefault();
                    if (edit == null) throw new Exception("ไม่พบข้อมูลประเภทฟอร์ม");

                    edit.ProjectTypeID = model.ProjectTypeID;
                    edit.Name = model.FormTypeName;
                    edit.Description = model.FormTypeDesc;
                    edit.UpdateDate = DateTime.Now;
                    //Edit.UpdateBy = ; // wait for permission login

                    // condition for check formtype using
                    bool verify = VerifyFormTypeUsing((int)model.FormTypeID);
                    if (verify) throw new Exception("ข้อมูลประเภทฟอร์มถูกใช้งานแล้ว");

                    // condition pass
                    _context.tm_FormType.Update(edit);
                    _context.SaveChanges();

                    data = new FormTypeResp()
                    {
                        FormTypeID = edit.ID,
                        ProjectTypeID = (int)edit.ProjectTypeID,
                        FormTypeName = edit.Name,
                        FormTypeDesc = edit.Description,
                        FlagActive = edit.FlagActive
                    };
                }
            }
            else if (model.TypeData == FormTypeConstant.Action_Form_Type.DELETE)
            {
                if (model.FormTypeID > 0)
                {
                    tm_FormType? delete = _context.tm_FormType.Where(o => o.ID == model.FormTypeID
                                                                   && o.FlagActive == true)
                                                           .FirstOrDefault();
                    if (delete == null) throw new Exception("ไม่พบข้อมูลประเภทฟอร์ม");

                    delete.FlagActive = false;
                    delete.UpdateDate = DateTime.Now;
                    //delete.UpdateBy = ; // wait for permission login

                    // condition for check formtype using
                    bool verify = VerifyFormTypeUsing((int)model.FormTypeID);
                    if (verify) throw new Exception("ข้อมูลประเภทฟอร์มถูกใช้งานแล้ว");

                    // condition pass
                    _context.tm_FormType.Update(delete);
                    _context.SaveChanges();

                    data = new FormTypeResp()
                    {
                        FormTypeID = delete.ID,
                        ProjectTypeID = (int)delete.ProjectTypeID,
                        FormTypeName = delete.Name,
                        FormTypeDesc = delete.Description,
                        FlagActive = delete.FlagActive
                    };
                }
            }

            return data;
        }

        public dynamic GetFormList(DTParamModel param, int formTypeID)
        {
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            var query = from f in _context.tm_Form
                        where f.FlagActive == true && f.FormTypeID == formTypeID
                        select new
                        {
                            f.ID,
                            f.FormTypeID,
                            f.Name,
                            f.Description,
                            f.Progress,
                            f.DurationDay,
                            f.Sort
                        };

            var result = query.Page(param.start, param.length, i => i.Sort, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;
            return result.AsEnumerable().Select(e => new
            {
                ID = e.ID,
                FormTypeID = e.FormTypeID,
                Name = e.Name,
                Description = e.Description,
                Progress = e.Progress,
                DurationDay = e.DurationDay,
                Sort = e.Sort
            }).ToList();
        }

        public dynamic GetFormGroupList(DTParamModel param, int formID)
        {
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            var query = from f in _context.tm_FormGroup
                        where f.FlagActive == true && f.FormID == formID
                        select new
                        {
                            f.ID,
                            f.Name,
                        };

            var result = query.Page(param.start, param.length, i => i.ID, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;
            return result.AsEnumerable().Select(e => new
            {
                ID = e.ID,
                Name = e.Name,
            }).ToList();
        }

        public dynamic GetFormPackageList(DTParamModel param, int groupID)
        {
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            var query = from f in _context.tm_FormPackage
                        where f.FlagActive == true && f.GroupID == groupID
                        select new
                        {
                            f.ID,
                            f.Name,
                        };

            var result = query.Page(param.start, param.length, i => i.ID, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;
            return result.AsEnumerable().Select(e => new
            {
                ID = e.ID,
                Name = e.Name,
            }).ToList();
        }

        public dynamic GetFormCheckList(DTParamModel param, int packageID)
        {
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            var query = from f in _context.tm_FormCheckList
                        where f.FlagActive == true && f.PackageID == packageID
                        select new
                        {
                            f.ID,
                            f.Name,
                        };

            var result = query.Page(param.start, param.length, i => i.ID, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;
            return result.AsEnumerable().Select(e => new
            {
                ID = e.ID,
                Name = e.Name,
            }).ToList();
        }

        public dynamic GetQcList(int formTypeID)
        {
            var queryFt = (from i in _context.tm_FormType
                         where i.ID == formTypeID && i.FlagActive == true
                         select new
                         {
                             i.ProjectTypeID,
                             i.ID
                         }).FirstOrDefault();

            var queryQc = (from qc in _context.tm_QC_CheckList
                           join ext in _context.tm_Ext on qc.QCTypeID equals ext.ID
                           where qc.ProjectTypeID == queryFt.ProjectTypeID && qc.FlagActive == true
                           select new
                           {
                               qc.ID,
                               ext.Name
                           }).ToList();

            return queryQc;
        }

        public dynamic GetFormDetail(int formID)
        {
            var result = (from f in _context.tm_Form
                          where f.ID == formID && f.FlagActive == true
                          select new
                          {
                              f.ID,
                              f.Name,
                              f.Description,
                              f.Progress,
                              f.DurationDay,
                              QcLists = (from cl in _context.tr_Form_QCCheckList
                                         join mcl in _context.tm_QC_CheckList on cl.CheckListID equals mcl.ID
                                         join ext in _context.tm_Ext on mcl.QCTypeID equals ext.ID
                                         where cl.FormID == f.ID && cl.FlagActive == true
                                         select new
                                         {
                                             mcl.ID,
                                             ext.Name,
                                         }).ToList()
                          }).FirstOrDefault();

            return result;
        }

        public dynamic GetGroupDetail(int groupID)
        {
            var query = (from fg in _context.tm_FormGroup
                         where fg.ID == groupID && fg.FlagActive == true
                         select new
                         {
                             fg.ID,
                             fg.Name
                         }).FirstOrDefault();

            return query;
        }

        public dynamic GetPackageDetail(int packageID)
        {
            var query = (from fg in _context.tm_FormPackage
                         where fg.ID == packageID && fg.FlagActive == true
                         select new
                         {
                             fg.ID,
                             fg.Name
                         }).FirstOrDefault();

            return query;
        }

        public dynamic GetCheckListDetail(int checkID)
        {
            var query = (from fg in _context.tm_FormCheckList
                         where fg.ID == checkID && fg.FlagActive == true
                         select new
                         {
                             fg.ID,
                             fg.Name
                         }).FirstOrDefault();

            return query;
        }

        private bool VerifyFormTypeUsing(int formTypeID)
        {
            bool query = (from pmf in _context.tr_ProjectModelForm
                         join u in _context.tm_Unit on pmf.ModelTypeID equals u.ModelTypeID
                         join uf in _context.tr_UnitForm on u.UnitID equals uf.UnitID
                         where pmf.FormTypeID == formTypeID
                         select new { pmf, u, uf }
                        ).Any();

            return query;
        }

        public dynamic ActionForm(FormModel model)
        {
            // condition for check formtype using
            bool verify = VerifyFormTypeUsing((int)model.FormTypeID);
            if (verify) throw new Exception("ข้อมูลประเภทฟอร์มถูกใช้งานแล้ว");

            FormResp? formResp = null;
            if ( model.TypeData == FormTypeConstant.Action_Form_Type.CREATE )
            {               
                // query for get sort 
                tm_Form? sort = _context.tm_Form
                                .Where(o => o.FormTypeID == model.FormTypeID)
                                .OrderByDescending(o => o.Sort)
                                .FirstOrDefault();

                // new create form 
                tm_Form create = new tm_Form();
                create.FormTypeID = model.FormTypeID;
                create.Name = model.FormName;
                create.Description = model.FormDesc;
                create.Progress = model.Progress;
                create.DurationDay = model.Duration;
                create.Sort = sort != null ? sort.Sort + 1 : 1;
                create.FlagActive = true;
                create.CreateDate = DateTime.Now;
                create.UpdateDate = DateTime.Now;
                //create.CreateBy = ;
                //create.UpdateBy = ;

                _context.tm_Form.Add(create);
                _context.SaveChanges();

                // add qc in form
                if (model.QcList != null)
                {
                    var qcCheckList = new List<tr_Form_QCCheckList>();

                    foreach (var qc in model.QcList)
                    {
                        tr_Form_QCCheckList createQc = new tr_Form_QCCheckList
                        {
                            FormID = create.ID,
                            CheckListID = qc,
                            FlagActive = true,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            //CreateBy = ,
                            //UpdateBy = ,
                        };
                        qcCheckList.Add(createQc);
                    }

                    _context.tr_Form_QCCheckList.AddRange(qcCheckList);
                    _context.SaveChanges();
                }

                formResp = new FormResp()
                {
                    FormTypeID = (int)model.FormTypeID,
                    FormID = create.ID,
                    FormName = create.Name,
                    FormDesc = create.Description,
                    Progress = (decimal)create.Progress,
                    Duration = (int)create.DurationDay,
                    QcList = model.QcList
                };
            }
            else if ( model.TypeData == FormTypeConstant.Action_Form_Type.EDIT )
            {
                // query form for edit
                tm_Form? edit = _context.tm_Form
                            .Where(o => o.ID == model.FormID && o.FlagActive == true)
                            .FirstOrDefault();
                if (edit == null) throw new Exception("ไม่พบข้อมูลฟอร์ม");

                edit.Name = model.FormName;
                edit.Description = model.FormDesc;
                edit.Progress = model.Progress;
                edit.DurationDay = model.Duration;
                edit.UpdateDate = DateTime.Now;
                //edit.UpdateBy = ;

                _context.tm_Form.Update(edit);
                _context.SaveChanges();

                // check qc in form 
                List<int> qc = _context.tr_Form_QCCheckList
                                            .Where(o => o.FormID == model.FormID && o.FlagActive == true)
                                            .Select(o => o.ID).ToList();
                if (qc.Count > 0)
                {
                    // Check if model.QcList is null
                    IEnumerable<int> checkQc;
                    if (model.QcList == null)
                    {
                        // If model.QcList is null, remove all items in qc
                        checkQc = qc;
                    }
                    else
                    {
                        checkQc = qc.Except(model.QcList);

                        IEnumerable<int> newQcs = model.QcList.Except(qc);

                        // Add these new items to `qc`
                        foreach (var newItem in newQcs)
                        {
                            tr_Form_QCCheckList? qcForm = new tr_Form_QCCheckList
                            {
                                FormID = model.FormID,
                                CheckListID = newItem,
                                FlagActive = true,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                //CreateBy = ,
                                //UpdateBy = ,
                            };

                            _context.tr_Form_QCCheckList.Add(qcForm);
                        }
                    }
                    foreach (var data in checkQc)
                    {
                        tr_Form_QCCheckList? qcForm = _context.tr_Form_QCCheckList
                                                    .Where(o => o.ID == data && o.FormID == model.FormID)
                                                    .FirstOrDefault();

                        qcForm.FlagActive = false;
                        //qcForm.UpdateBy = ;
                        qcForm.UpdateDate = DateTime.Now;

                        _context.tr_Form_QCCheckList.Update(qcForm);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    var qcCheckList = new List<tr_Form_QCCheckList>();

                    foreach (var value in model.QcList)
                    {
                        tr_Form_QCCheckList createQc = new tr_Form_QCCheckList
                        {
                            FormID = model.FormID,
                            CheckListID = value,
                            FlagActive = true,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            //CreateBy = ,
                            //UpdateBy = ,
                        };
                        qcCheckList.Add(createQc);
                    }

                    _context.tr_Form_QCCheckList.AddRange(qcCheckList);
                    _context.SaveChanges();
                }
                
                formResp = new FormResp()
                {
                    FormTypeID = (int)model.FormTypeID,
                    FormID = edit.ID,
                    FormName = edit.Name,
                    FormDesc = edit.Description,
                    Progress = (decimal)edit.Progress,
                    Duration = (int)edit.DurationDay,
                    QcList = model.QcList
                };
            }
            else if ( model.TypeData == FormTypeConstant.Action_Form_Type.DELETE )
            {
                // query form for delete
                tm_Form? delete = _context.tm_Form
                            .Where(o => o.ID == model.FormID && o.FlagActive == true)
                            .FirstOrDefault();
                if (delete == null) throw new Exception("ไม่พบข้อมูลฟอร์ม");

                delete.FlagActive = false;
                //delete.UpdateBy = ;
                delete.UpdateDate = DateTime.Now;

                _context.tm_Form.Update(delete);

                List<int> formQcList = _context.tr_Form_QCCheckList
                                            .Where(o => o.FormID == delete.ID)
                                            .Select(o => o.ID)
                                            .ToList();
                if( formQcList != null)
                {
                    foreach (var qc in formQcList)
                    {
                        tr_Form_QCCheckList? formQc = _context.tr_Form_QCCheckList
                                                    .Where(o => o.ID == qc && o.FlagActive == true)
                                                    .FirstOrDefault();

                        formQc.FlagActive = false;
                        //formQc.UpdateBy = ;
                        formQc.UpdateDate = DateTime.Now;

                        _context.tr_Form_QCCheckList.Update(formQc);
                    }
                }

                _context.SaveChanges();

                formResp = new FormResp()
                {
                    FormTypeID = (int)model.FormTypeID,
                    FormID = delete.ID,
                    FormName = delete.Name,
                    FormDesc = delete.Description,
                    Progress = (decimal)delete.Progress,
                    Duration = (int)delete.DurationDay,
                    QcList = model.QcList
                };
            }

            return formResp;
        }

        public dynamic ActionGroup(GroupModel model)
        {
            // condition for check formtype using
            bool verify = VerifyFormTypeUsing((int)model.FormTypeID);
            if (verify) throw new Exception("ข้อมูลประเภทฟอร์มถูกใช้งานแล้ว");

            GroupResp? groupResp = null;

            if (model.TypeData == FormTypeConstant.Action_Form_Type.CREATE)
            {
                // query group for sort
                tm_FormGroup? sort = _context.tm_FormGroup
                                    .Where(o => o.FormID == model.FormID)
                                    .OrderByDescending(o => o.Sort)
                                    .FirstOrDefault();

                // create data form group
                tm_FormGroup create = new tm_FormGroup();
                create.FormID = model.FormID;
                create.Name = model.GroupName;
                create.Sort = sort != null ? sort.Sort + 1 : 1;
                create.FlagActive = true;
                create.CreateDate = DateTime.Now;
                create.UpdateDate = DateTime.Now;
                //create.CreateBy = ;
                //create.UpdateBy = ;

                _context.tm_FormGroup.Add(create);
                _context.SaveChanges();

                groupResp = new GroupResp()
                {
                    GroupID = create.ID,
                    FormID = (int)create.FormID,
                    GroupName = create.Name
                };

            }
            else if (model.TypeData == FormTypeConstant.Action_Form_Type.EDIT)
            {
                tm_FormGroup? edit = _context.tm_FormGroup
                                    .Where(o => o.ID == model.GroupID && o.FlagActive == true)
                                    .FirstOrDefault();
                if (edit == null) throw new Exception("ไม่พบข้อมูลกลุ่มฟอร์ม");

                edit.Name = model.GroupName;
                //edit.UpdateBy =;
                edit.UpdateDate = DateTime.Now;

                _context.tm_FormGroup.Update(edit);
                _context.SaveChanges();

                groupResp = new GroupResp()
                {
                    GroupID = edit.ID,
                    FormID = (int)edit.FormID,
                    GroupName = edit.Name
                };
            }
            else if (model.TypeData == FormTypeConstant.Action_Form_Type.DELETE)
            {
                tm_FormGroup? delete = _context.tm_FormGroup
                                    .Where(o => o.ID == model.GroupID && o.FlagActive == true)
                                    .FirstOrDefault();
                if (delete == null) throw new Exception("ไม่พบข้อมูลกลุ่มฟอร์ม");

                delete.FlagActive = false;
                //delete.UpdateBy =;
                delete.UpdateDate = DateTime.Now;

                _context.tm_FormGroup.Update(delete);
                _context.SaveChanges();

                groupResp = new GroupResp()
                {
                    GroupID = delete.ID,
                    FormID = (int)delete.FormID,
                    GroupName = delete.Name
                };
            }
             
            return groupResp;
        }

        public dynamic ActionPacakge(PackageModel model)
        {
            // condition for check formtype using
            bool verify = VerifyFormTypeUsing((int)model.FormTypeID);
            if (verify) throw new Exception("ข้อมูลประเภทฟอร์มถูกใช้งานแล้ว");

            PackageResp? packageResp = null;

            if (model.TypeData == FormTypeConstant.Action_Form_Type.CREATE)
            {
                // query package for sort
                tm_FormPackage? sort = _context.tm_FormPackage
                                    .Where(o => o.GroupID == model.GroupID)
                                    .OrderByDescending(o => o.Sort)
                                    .FirstOrDefault();

                // create data form package
                tm_FormPackage create = new tm_FormPackage();
                create.GroupID = model.GroupID;
                create.Name = model.PackageName;
                create.Sort = sort != null ? sort.Sort + 1 : 1;
                create.FlagActive = true;
                create.CreateDate = DateTime.Now;
                create.UpdateDate = DateTime.Now;
                //create.CreateBy = ;
                //create.UpdateBy = ;

                _context.tm_FormPackage.Add(create);
                _context.SaveChanges();

                packageResp = new PackageResp()
                {
                    PackageID = (int)create.ID,
                    GroupID = (int)create.GroupID,
                    PackageName = create.Name
                };
            }
            else if (model.TypeData == FormTypeConstant.Action_Form_Type.EDIT)
            {
                tm_FormPackage? edit = _context.tm_FormPackage
                                    .Where(o => o.ID == model.PackageID && o.FlagActive == true)
                                    .FirstOrDefault();
                if (edit == null) throw new Exception("ไม่พบข้อมูลแพ็คเกจฟอร์ม");

                edit.Name = model.PackageName;
                edit.UpdateDate = DateTime.Now;
                //edit.UpdateBy = ;

                _context.tm_FormPackage.Update(edit);
                _context.SaveChanges();

                packageResp = new PackageResp()
                {
                    PackageID = (int)edit.ID,
                    GroupID = (int)edit.GroupID,
                    PackageName = edit.Name
                };
            }
            else if (model.TypeData == FormTypeConstant.Action_Form_Type.DELETE)
            {
                tm_FormPackage? delete = _context.tm_FormPackage
                                    .Where(o => o.ID == model.PackageID && o.FlagActive == true)
                                    .FirstOrDefault();
                if (delete == null) throw new Exception("ไม่พบข้อมูลแพ็คเกจฟอร์ม");

                delete.FlagActive = false;
                delete.UpdateDate = DateTime.Now;
                //edit.UpdateBy = ;

                _context.tm_FormPackage.Update(delete);
                _context.SaveChanges();

                packageResp = new PackageResp()
                {
                    PackageID = (int)delete.ID,
                    GroupID = (int)delete.GroupID,
                    PackageName = delete.Name
                };
            }

            return packageResp;
        }

        public dynamic ActionCheckList(CheckListModel model)
        {
            // condition for check formtype using
            bool verify = VerifyFormTypeUsing((int)model.FormTypeID);
            if (verify) throw new Exception("ข้อมูลประเภทฟอร์มถูกใช้งานแล้ว");

            CheckListResp? checkListResp = null;

            if (model.TypeData == FormTypeConstant.Action_Form_Type.CREATE)
            {
                // query checklist for sort
                tm_FormCheckList? sort = _context.tm_FormCheckList
                                    .Where(o => o.PackageID == model.PackageID)
                                    .OrderByDescending(o => o.Sort)
                                    .FirstOrDefault();

                // create data form checklist
                tm_FormCheckList create = new tm_FormCheckList();
                create.PackageID = model.PackageID;
                create.Name = model.CheckListName;
                create.Sort = sort != null ? sort.Sort + 1 : 1;
                create.FlagActive = true;
                create.CreateDate = DateTime.Now;
                create.UpdateDate = DateTime.Now;
                //create.CreateBy = ;
                //create.UpdateBy = ;

                _context.tm_FormCheckList.Add(create);
                _context.SaveChanges();

                checkListResp = new CheckListResp()
                {
                    CheckListID = (int)create.ID,
                    PackageID = (int)create.PackageID,
                    CheckListName = create.Name
                };
            }
            else if (model.TypeData == FormTypeConstant.Action_Form_Type.EDIT)
            {
                tm_FormCheckList? edit = _context.tm_FormCheckList
                                    .Where(o => o.ID == model.CheckListID && o.FlagActive == true)
                                    .FirstOrDefault();
                if (edit == null) throw new Exception("ไม่พบข้อมูลฟอร์มรายการตรวจ");

                edit.Name = model.CheckListName;
                edit.UpdateDate = DateTime.Now;
                //edit.UpdateBy = ;

                _context.tm_FormCheckList.Update(edit);
                _context.SaveChanges();

                checkListResp = new CheckListResp()
                {
                    CheckListID = (int)edit.ID,
                    PackageID = (int)edit.PackageID,
                    CheckListName = edit.Name
                };
            }
            else if (model.TypeData == FormTypeConstant.Action_Form_Type.DELETE)
            {
                tm_FormCheckList? delete = _context.tm_FormCheckList
                                    .Where(o => o.ID == model.CheckListID && o.FlagActive == true)
                                    .FirstOrDefault();
                if (delete == null) throw new Exception("ไม่พบข้อมูลฟอร์มรายการตรวจ");

                delete.FlagActive = false;
                delete.UpdateDate = DateTime.Now;
                //edit.UpdateBy = ;

                _context.tm_FormCheckList.Update(delete);
                _context.SaveChanges();

                checkListResp = new CheckListResp()
                {
                    CheckListID = (int)delete.ID,
                    PackageID = (int)delete.PackageID,
                    CheckListName = delete.Name
                };
            }

            return checkListResp;
        }
    }
}

