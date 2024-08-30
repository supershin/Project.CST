using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MProjectModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IMasterProjectRepo
	{
        dynamic GetProjectList(DTParamModel param, MasterProjectModel criteria);

        dynamic InfomationForProejct();

        dynamic DetailProjectInformation(Guid projectID);

        dynamic CreateProject(CreateProjectModel model);

        dynamic GetFormTypeList(int projectTypeId);

        dynamic DeleteProject(Guid projectID);

        EditProjectResp EditProject(EditProjectModel model);
    }

    public class MasterProjectRepo : IMasterProjectRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public MasterProjectRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public dynamic GetProjectList(DTParamModel param, MasterProjectModel criteria)
        {
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            //variable = (condition) ? expressionTrue :  expressionFalse;
            criteria.strSearch = (criteria.strSearch == null) ? string.Empty : criteria.strSearch.ToString() ?? string.Empty;

            var query = from tmp in _context.tm_Project
                        where tmp.FlagActive == true
                        select new
                        {
                            tmp.ProjectID,
                            tmp.ProjectCode,
                            tmp.ProjectName,
                            tmp.CreateDate,
                            tmp.UpdateDate
                        };

            if (!string.IsNullOrEmpty(criteria.strSearch))
            {
                query = query.Where(o =>
                    o.ProjectCode.Contains(criteria.strSearch) ||
                    o.ProjectName.Contains(criteria.strSearch)
                );
            }

            var result = query.Page(param.start, param.length, i => i.CreateDate, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;

            return result.AsEnumerable().Select(e => new
            {
                ProjectId = e.ProjectID,
                ProjectCode = e.ProjectCode,
                ProjectName = e.ProjectName,
                CreateDate = e.CreateDate,
                UpdateDate = e.UpdateDate.ToStringDateTime()
            }).ToList();
        }

        public dynamic InfomationForProejct()
        {
            List<tm_BU> bu = _context.tm_BU.Where(o => o.FlagActive == true).ToList();

            List<tm_Ext> ext = _context.tm_Ext
                            .Where(o => o.ExtTypeID == SystemConstant.Ext_Type.PROJECT_TYPE
                            && o.FlagActive == true).ToList();
            var resp = new
            {
                bu,
                ext
            };
            return resp;
        }

        public dynamic DetailProjectInformation(Guid projectID)
        {
            var query = (from mp in _context.tm_Project
                        join mb in _context.tm_BU on mp.BUID equals mb.ID
                        join me in _context.tm_Ext on mp.ProjectTypeID equals me.ID
                        where mp.ProjectID == projectID && mp.FlagActive == true
                        select new
                        {
                            mp.ProjectID,
                            mp.ProjectCode,
                            mp.ProjectName,
                            BUID = mb.ID,
                            BUName = mb.Name,
                            ProjectTypeID = me.ID,
                            ProjectTypeName = me.Name,
                            ListModelType = (from mm in _context.tm_ModelType
                                             where mm.ProjectID == mp.ProjectID && mm.FlagActive == true
                                             select new
                                             {
                                                 ModelID = mm.ID,
                                                 ModelCode = mm.ModelCode,
                                                 ModelName = mm.ModelName,
                                                 ModelTypeCode = mm.ModelTypeCode,
                                                 ModelTypeName = mm.ModelTypeName,
                                                 MappingForm = (from pmf in _context.tr_ProjectModelForm
                                                                join mft in _context.tm_FormType on pmf.FormTypeID equals mft.ID
                                                                where pmf.ProjectID == mp.ProjectID && pmf.ModelTypeID == mm.ID && pmf.FlagActive == true
                                                                select new
                                                                {
                                                                    FormTypeID = mft.ID,
                                                                    FormTypeName = mft.Name
                                                                }).FirstOrDefault()
                                             }).ToList()
                        }).FirstOrDefault();

            return query;
        }

        public dynamic CreateProject(CreateProjectModel model)
        {
            Guid guid = Guid.NewGuid();

            tm_Project create = new tm_Project();
            create.ProjectID = guid;
            create.BUID = model.BUID;
            create.ProjectTypeID = model.ProjectTypeID;
            create.ProjectCode = model.ProjectCode;
            create.ProjectName = model.ProjectName;
            create.FlagActive = true;
            create.CreateDate = DateTime.Now;
            create.UpdateDate = DateTime.Now;
            //create.CreateBy = ;
            //create.UpdateBy = ;

            _context.tm_Project.Add(create);
            _context.SaveChanges();

            return create;
        }

        public dynamic GetFormTypeList(int projectTypeId)
        {
            var query = (from ft in _context.tm_FormType
                         where ft.ProjectTypeID == projectTypeId && ft.FlagActive == true
                         select new
                         {
                             ft.ID,
                             ft.Name
                         }).ToList();

            return query;
        }

        public dynamic DeleteProject(Guid projectID)
        {
            bool verify = VerifyFormTypeUsing(projectID);
            if (verify) throw new Exception("ข้อมูลโครงการถูกใช้งานแล้ว");

            tm_Project? delete = _context.tm_Project.Where(o => o.ProjectID == projectID && o.FlagActive == true).FirstOrDefault();
            if (delete == null) throw new Exception("ไม่พบข้อมูลโครงการ");

            delete.FlagActive = false;
            delete.UpdateDate = DateTime.Now;
            //delete.UpdateBy = ;

            _context.tm_Project.Update(delete);
            _context.SaveChanges();

            return delete;
        }

        public EditProjectResp EditProject(EditProjectModel model)
        {
            bool verify = VerifyFormTypeUsing(model.ProjectID);
            if (verify) throw new Exception("ข้อมูลโครงการถูกใช้งานแล้ว");

            EditProjectResp resp = new EditProjectResp();
            ModelForm modelForm = new ModelForm();

            tm_Project? edit = _context.tm_Project
                            .Where(o => o.ProjectID == model.ProjectID
                            && o.FlagActive == true).FirstOrDefault();

            if (edit == null) throw new Exception("ไม่พบข้อมูลโครงการ");
            edit.BUID = model.BUID;
            edit.ProjectTypeID = model.ProjectTypeID;
            edit.ProjectCode = model.ProjectCode;
            edit.ProjectName = model.ProjectName;
            edit.UpdateDate = DateTime.Now;
            //edit.UpdateBy =;

            _context.tm_Project.Update(edit);

            resp = new EditProjectResp()
            {
                BUID = (int)edit.BUID,
                ProjectTypeID = (int)edit.ProjectTypeID,
                ProjectID = edit.ProjectID,
                ProjectCode = edit.ProjectCode,
                ProjectName = edit.ProjectName,
                ModelMapping = new List<ModelForm>()
            };

            if(model.ModelMapping != null)
            {
                foreach (var list in model.ModelMapping)
                {
                    tr_ProjectModelForm? editModel = _context.tr_ProjectModelForm
                                            .Where(o => o.ProjectID == model.ProjectID
                                            && o.ModelTypeID == list.ModelID
                                            && o.FlagActive == true)
                                            .FirstOrDefault();
                    if(editModel == null)
                    {
                        tr_ProjectModelForm? createNew = new tr_ProjectModelForm();
                        createNew.ProjectID = edit.ProjectID;
                        createNew.ModelTypeID = list.ModelID;
                        createNew.FormTypeID = list.FormTypeID;
                        createNew.FlagActive = true;
                        //createNew.CreateBy =;
                        createNew.CreateDate = DateTime.Now;
                        createNew.UpdateDate = DateTime.Now;
                        //createNew.UpdateBy = ;

                        _context.tr_ProjectModelForm.Add(createNew);
                        _context.SaveChanges();
                    }
                    else
                    {
                        if (editModel.FormTypeID != list.FormTypeID)
                        {
                            editModel.FormTypeID = list.FormTypeID;
                            editModel.UpdateDate = DateTime.Now;
                            //editModel.UpdateBy = ;

                            _context.tr_ProjectModelForm.Update(editModel);

                            modelForm = new ModelForm()
                            {
                                ModelID = editModel.ID,
                                FormTypeID = (int)editModel.FormTypeID
                            };

                            resp.ModelMapping.Add(modelForm);
                        }
                    }
                }
            }

            _context.SaveChanges();


            return resp;
        }

        private bool VerifyFormTypeUsing(Guid projectId)
        {
            bool query = (from pmf in _context.tr_ProjectModelForm
                          join u in _context.tm_Unit on pmf.ModelTypeID equals u.ModelTypeID
                          join uf in _context.tr_UnitForm on u.UnitID equals uf.UnitID
                          where pmf.ProjectID == projectId && pmf.FormTypeID != null
                          select new { pmf, u, uf }
                        ).Any();

            return query;
        }
    }
}

