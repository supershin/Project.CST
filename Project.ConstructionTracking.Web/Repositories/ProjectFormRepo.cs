using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Project.ConstructionTracking.Web.Models.ProjectFormModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class ProjectFormRepo : IProjectFormRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public ProjectFormRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }


        public ProjectFormDetail GetDetail(int unitID)
        {
            var results =  unitID;
            return null;

            // Query to get actions
            //var query1 = from t1 in _context.tr_UnitForm_Action
            //             where t1.ID == unitID
            //             select new
            //             {
            //                 t1.SaveBy_PE,
            //                 t1.SaveDate_PE,
            //                 t1.SubmitBy_PE,
            //                 t1.SubmitDate_PE,
            //                 t1.Remark_PE,

            //                 t1.SaveBy_QC,
            //                 t1.SaveDate_QC,
            //                 t1.SubmitBy_QC,
            //                 t1.SubmitDate_QC,
            //                 t1.Remark_QC,

            //                 t1.SaveBy_PM,
            //                 t1.SaveDate_PM,
            //                 t1.ApproveBy_PM,
            //                 t1.ApproveDate_PM,
            //                 t1.RejectBy_PM,
            //                 t1.RejectDate_PM,
            //                 t1.Remark_PM,

            //                 t1.ApproveBy_VP,
            //                 t1.ApproveDate_VP,
            //                 t1.RejectBy_VP,
            //                 t1.RejectDate_VP,
            //                 t1.Remark_VP
            //             };

            //var queryResult = query1.FirstOrDefault();

            //if (queryResult == null)
            //{
            //    return new ProjectFormDetail();
            //}

            //// Query to get user information
            //var query2 = from t2 in _context.tm_User
            //             select new
            //             {
            //                 t2.ID,
            //                 t2.FirstName,
            //                 t2.LastName
            //             };

            //var queryResult2 = query2.ToList();

            //var result = from action in new[] { queryResult }
            //             join PE_SAVE in queryResult2 on action.SaveBy_PE equals PE_SAVE.ID into peSaveGroup
            //             from PE_SAVE in peSaveGroup.DefaultIfEmpty()
            //             join PE_SUBMIT in queryResult2 on action.SubmitBy_PE equals PE_SUBMIT.ID into peSubmitGroup
            //             from PE_SUBMIT in peSubmitGroup.DefaultIfEmpty()
            //             join QC_SAVE in queryResult2 on action.SaveBy_QC equals QC_SAVE.ID into qcSaveGroup
            //             from QC_SAVE in qcSaveGroup.DefaultIfEmpty()
            //             join QC_SUBMIT in queryResult2 on action.SubmitBy_QC equals QC_SUBMIT.ID into qcSubmitGroup
            //             from QC_SUBMIT in qcSubmitGroup.DefaultIfEmpty()
            //             join PM_SAVE in queryResult2 on action.SaveBy_PM equals PM_SAVE.ID into pmSaveGroup
            //             from PM_SAVE in pmSaveGroup.DefaultIfEmpty()
            //             join PM_APPROVE in queryResult2 on action.ApproveBy_PM equals PM_APPROVE.ID into pmApproveGroup
            //             from PM_APPROVE in pmApproveGroup.DefaultIfEmpty()
            //             join PM_REJECT in queryResult2 on action.RejectBy_PM equals PM_REJECT.ID into pmRejectGroup
            //             from PM_REJECT in pmRejectGroup.DefaultIfEmpty()
            //             join VP_APPROVE in queryResult2 on action.ApproveBy_VP equals VP_APPROVE.ID into vpApproveGroup
            //             from VP_APPROVE in vpApproveGroup.DefaultIfEmpty()
            //             join VP_REJECT in queryResult2 on action.RejectBy_VP equals VP_REJECT.ID into vpRejectGroup
            //             from VP_REJECT in vpRejectGroup.DefaultIfEmpty()
            //             select new
            //             {
            //                 action,
            //                 PE_SAVE,
            //                 PE_SUBMIT,
            //                 QC_SAVE,
            //                 QC_SUBMIT,
            //                 PM_SAVE,
            //                 PM_APPROVE,
            //                 PM_REJECT,
            //                 VP_APPROVE,
            //                 VP_REJECT
            //             };

            //return result.AsEnumerable().Select(e => new ProjectFormDetail
            //{
            //    SaveBy_PE = e.action.SaveBy_PE,
            //    SaveBy_PE_Name = e.PE_SAVE != null ? e.PE_SAVE.FirstName + " " + e.PE_SAVE.LastName : null,
            //    SaveDate_PE = e.action.SaveDate_PE.ToStringDate(),
            //    SubmitBy_PE = e.action.SubmitBy_PE,
            //    SubmitBy_PE_Name = e.PE_SUBMIT != null ? e.PE_SUBMIT.FirstName + " " + e.PE_SUBMIT.LastName : null,
            //    SubmitDate_PE = e.action.SubmitDate_PE.ToStringDate(),
            //    Remark_PE = e.action.Remark_PE,

            //    SaveBy_QC = e.action.SaveBy_QC,
            //    SaveBy_QC_Name = e.QC_SAVE != null ? e.QC_SAVE.FirstName + " " + e.QC_SAVE.LastName : null,
            //    SaveDate_QC = e.action.SaveDate_QC.ToStringDate(),
            //    SubmitBy_QC = e.action.SubmitBy_QC,
            //    SubmitBy_QC_Name = e.QC_SUBMIT != null ? e.QC_SUBMIT.FirstName + " " + e.QC_SUBMIT.LastName : null,
            //    SubmitDate_QC = e.action.SubmitDate_QC.ToStringDate(),
            //    Remark_QC = e.action.Remark_QC,

            //    SaveBy_PM = e.action.SaveBy_PM,
            //    SaveBy_PM_Name = e.PM_SAVE != null ? e.PM_SAVE.FirstName + " " + e.PM_SAVE.LastName : null,
            //    SaveDate_PM = e.action.SaveDate_PM.ToStringDate(),
            //    ApproveBy_PM = e.action.ApproveBy_PM,
            //    ApproveBy_PM_Name = e.PM_APPROVE != null ? e.PM_APPROVE.FirstName + " " + e.PM_APPROVE.LastName : null,
            //    ApproveDate_PM = e.action.ApproveDate_PM.ToStringDate(),
            //    RejectBy_PM = e.action.RejectBy_PM,
            //    RejectBy_PM_Name = e.PM_REJECT != null ? e.PM_REJECT.FirstName + " " + e.PM_REJECT.LastName : null,
            //    RejectDate_PM = e.action.RejectDate_PM.ToStringDate(),
            //    Remark_PM = e.action.Remark_PM,

            //    ApproveBy_VP = e.action.ApproveBy_VP,
            //    ApproveBy_VP_Name = e.VP_APPROVE != null ? e.VP_APPROVE.FirstName + " " + e.VP_APPROVE.LastName : null,
            //    ApproveDate_VP = e.action.ApproveDate_VP.ToStringDate(),
            //    RejectBy_VP = e.action.RejectBy_VP,
            //    RejectBy_VP_Name = e.VP_REJECT != null ? e.VP_REJECT.FirstName + " " + e.VP_REJECT.LastName : null,
            //    RejectDate_VP = e.action.RejectDate_VP.ToStringDate(),
            //    Remark_VP = e.action.Remark_VP

            //}).FirstOrDefault() ?? new ProjectFormDetail();
        }

        public List<ProjectFormModel.ProjectForm_getForm> GetFormCheckUnitList(int formID)
        {
            //var query = from t1 in _context.tr_ProjectForm                      
            //            join t2 in _context.tr_ProjectFormGroup on t1.ID equals t2.FormID into _t2Group
            //            from t2Group in _t2Group.DefaultIfEmpty()
            //            join t3 in _context.tr_ProjectFormPackage on t2Group.ID equals t3.GroupID into _t3Group
            //            from t3Group in _t3Group.DefaultIfEmpty()
            //            join t4 in _context.tr_ProjectFormCheckList on t3Group.ID equals t4.PackageID into _t4Group
            //            from t4Group in _t4Group.DefaultIfEmpty()
            //            join t5 in _context.tr_UnitForm on t1.ID equals t5.FormID into _t5Group
            //            from t5Group in _t5Group.DefaultIfEmpty()
            //            join t6 in _context.tr_UnitForm_Detail on t5Group.ID equals t6.UnitFormID into _t6Group
            //            from t6Group in _t6Group.DefaultIfEmpty()
            //            where t1.ID == formID && t1.FormTypeID == SystemConstant.Ext.FORM_TYPE_PE && t1.FormRefID == null && t6Group.CheckListID == t4Group.ID
            //            select new
            //            {
            //                t1,
            //                t2Group,
            //                t3Group,
            //                t4Group,
            //                t5Group,
            //                t6Group
            //            };

            //var queryResult = query.ToList();

            var results = new List<ProjectFormModel.ProjectForm_getForm>();

            //foreach (var item in queryResult)
            //{
            //    var existingForm = results.FirstOrDefault(f => f.FormID == item.t1.ID);
            //    if (existingForm == null)
            //    {
            //        existingForm = new ProjectFormModel.ProjectForm_getForm
            //        {
            //            FormID = item.t1.ID,
            //            FormName = item.t1.Name,
            //            ProjectID = item.t1.ProjectID,
            //            //UnitID = item.t1.U
            //            Desc = item.t1.Description,
            //            StatusID = item.t5Group.StatusID,
            //            ListGroups = new List<ProjectFormModel.ProjectForm_getListGroups>()
            //        };
            //        results.Add(existingForm);
            //    }

            //    if (item.t2Group != null)
            //    {
            //        var existingGroup = existingForm.ListGroups.FirstOrDefault(g => g.GroupID == item.t2Group.ID);
            //        if (existingGroup == null)
            //        {
            //            existingGroup = new ProjectFormModel.ProjectForm_getListGroups
            //            {
            //                GroupID = item.t2Group.ID,
            //                GroupName = item.t2Group.Name,
            //                ListPackages = new List<ProjectFormModel.ProjectForm_getListPackages>()
            //            };
            //            existingForm.ListGroups.Add(existingGroup);
            //        }

            //        if (item.t3Group != null)
            //        {
            //            var existingPackage = existingGroup.ListPackages.FirstOrDefault(p => p.PackagesID == item.t3Group.ID);
            //            if (existingPackage == null)
            //            {
            //                existingPackage = new ProjectFormModel.ProjectForm_getListPackages
            //                {
            //                    PackagesID = item.t3Group.ID,
            //                    PackagesName = item.t3Group.Name,
            //                    ListCheckLists = new List<ProjectFormModel.ProjectForm_getListCheckLists>()
            //                };
            //                existingGroup.ListPackages.Add(existingPackage);
            //            }

            //            if (item.t4Group != null)
            //            {
            //                var existingCheckList = existingPackage.ListCheckLists.FirstOrDefault(c => c.CheckListID == item.t4Group.ID);
            //                if (existingCheckList == null)
            //                {
            //                    existingCheckList = new ProjectFormModel.ProjectForm_getListCheckLists
            //                    {
            //                        CheckListID = item.t4Group.ID,
            //                        CheckListName = item.t4Group.Name,
            //                        StatusID = item.t6Group.CheckListStatusID,
            //                        Remark = item.t6Group.Remark
            //                    };
            //                    existingPackage.ListCheckLists.Add(existingCheckList);
            //                }
            //            }
            //        }
            //    }
            //}

            return results;
        }

        public void InsertFormCheckListUnit(UnitForm model)
        {
            var newForm = new tr_UnitForm
            {
                ID = Guid.NewGuid(),
                ProjectID = model.ProjectID,
                UnitID = model.UnitID,
                FormID = model.FormID
            };
            _context.tr_UnitForm.Add(newForm);
            _context.SaveChanges();

            //foreach (var form in UnitForm)
            //{
            //    var newForm = new ProjectForm
            //    {
            //        ProjectID = form.ProjectID,
            //        UnitID = form.UnitID,
            //        FormID = form.FormID,
            //        Name = form.FormName,
            //        Description = form.Desc,
            //        StatusID = form.StatusID
            //    };

            //    foreach (var group in form.ListGroups)
            //    {
            //        var newGroup = new ProjectFormGroup
            //        {
            //            FormID = newForm.ID,
            //            Name = group.GroupName
            //        };

            //        foreach (var package in group.ListPackages)
            //        {
            //            var newPackage = new ProjectFormPackage
            //            {
            //                GroupID = newGroup.ID,
            //                Name = package.PackagesName
            //            };

            //            foreach (var checklist in package.ListCheckLists)
            //            {
            //                var newCheckList = new ProjectFormCheckList
            //                {
            //                    //PackageID = newPackage.ID,
            //                    CheckListID = checklist.CheckListID,
            //                    //StatusID = checklist.StatusID,
            //                    //Remark = checklist.Remark
            //                };
            //                //_context.tr_UnitForm_Detail.Add(newCheckList);
            //                //_context.SaveChanges();
            //            }
            //        }
            //    }
            //}
        }
    }
}
