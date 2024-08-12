using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class PMApproveRepo : IPMApproveRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public PMApproveRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }
        public List<PMApproveModel> GetPMApproveFormList()
        {
            var result = (from t1 in _context.tr_UnitForm
                          join t2 in _context.tm_Vendor on t1.VendorID equals t2.ID into vendors
                          from vendor in vendors.DefaultIfEmpty()
                          join t3 in _context.tr_UnitFormAction.Where(a => a.RoleID == 1)
                              on t1.ID equals t3.UnitFormID into unitFormActionsPE
                          from actionPE in unitFormActionsPE.DefaultIfEmpty()
                          join t3m in _context.tr_UnitFormAction.Where(a => a.RoleID == 2)
                              on t1.ID equals t3m.UnitFormID into unitFormActionsPM
                          from actionPM in unitFormActionsPM.DefaultIfEmpty()
                          join t4 in _context.tm_Project on t1.ProjectID equals t4.ProjectID into projects
                          from project in projects.DefaultIfEmpty()
                          join t5 in _context.tm_Unit on t1.UnitID equals t5.UnitID into units
                          from unit in units.DefaultIfEmpty()
                          join t6 in _context.tm_Form on t1.FormID equals t6.ID into forms
                          from form in forms.DefaultIfEmpty()
                          where actionPE.ActionType == "submit"
                          select new
                          {
                              UnitFormID = t1.ID,
                              UnitFormActionID = actionPE.ID,
                              t1.ProjectID,
                              project.ProjectName,
                              t1.UnitID,
                              unit.UnitCode,
                              t1.VendorID,
                              VenderName = vendor.Name,
                              t1.Grade,
                              t1.FormID,
                              FormName = form.Name,
                              t1.StatusID,
                              actionPE.PassConditionID,
                              RoleID_PE = actionPE.RoleID,
                              ActionType_PE = actionPE.ActionType,
                              StatusID_PE = actionPE.StatusID,
                              Remark_PE = actionPE.Remark,
                              ActionDate_PE = actionPE.ActionDate,
                              RoleID_PM = actionPM.RoleID,
                              ActionType_PM = actionPM.ActionType,
                              StatusID_PM = actionPM.StatusID,
                              Remark_PM = actionPM.Remark,
                              ActionDate_PM = actionPM.ActionDate
                          })
                          .OrderBy(item => item.ActionType_PE == "submit" && item.ActionType_PM == null ? 0 : 1)
                          .ThenBy(item => item.UnitFormID)
                          .ToList();

            // Format the date after retrieving the data from the database
            var formattedResult = result.Select(item => new PMApproveModel
            {
                UnitFormID = item.UnitFormID,
                UnitFormActionID = item.UnitFormActionID,
                ProjectID = item.ProjectID,
                ProjectName = item.ProjectName,
                UnitID = item.UnitID,
                UnitCode = item.UnitCode,
                VendorID = item.VendorID,
                VenderName = item.VenderName,
                Grade = item.Grade,
                FormID = item.FormID,
                FormName = item.FormName,
                StatusID = item.StatusID,
                PassConditionID = item.PassConditionID,
                RoleID_PE = item.RoleID_PE,
                ActionType_PE = item.ActionType_PE,
                StatusID_PE = item.StatusID_PE,
                Remark_PE = item.Remark_PE,
                ActionDate_PE = item.ActionDate_PE.HasValue
                                ? FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(item.ActionDate_PE.Value.ToString("dd/MM/yyyy"))
                                : null,
                RoleID_PM = item.RoleID_PM,
                ActionType_PM = item.ActionType_PM,
                StatusID_PM = item.StatusID_PM,
                Remark_PM = item.Remark_PM,
                ActionDate_PM = item.ActionDate_PM
            }).ToList();

            return formattedResult;
        }

        public List<ApproveFormcheckModel> GetApproveFormcheckList(ApproveFormcheckModel model)
        {
            var result = (from t1 in _context.tr_UnitForm
                          join t2 in _context.tm_Vendor on t1.VendorID equals t2.ID into vendors
                          from vendor in vendors.DefaultIfEmpty()
                          join t3 in _context.tr_UnitFormAction on t1.ID equals t3.UnitFormID into unitFormActions
                          from action in unitFormActions.DefaultIfEmpty()
                          join t4 in _context.tm_Project on t1.ProjectID equals t4.ProjectID into projects
                          from project in projects.DefaultIfEmpty()
                          join t5 in _context.tm_Unit on t1.UnitID equals t5.UnitID into units
                          from unit in units.DefaultIfEmpty()
                          join t6 in _context.tm_FormGroup on t1.FormID equals t6.FormID into formGroups
                          from formGroup in formGroups.DefaultIfEmpty()
                          join t7 in _context.tr_UnitFormPassCondition on new { UnitFormID = (Guid?)t1.ID, GroupID = formGroup.ID } equals new { t7.UnitFormID, t7.GroupID } into unitFormPassConditions
                          from passCondition in unitFormPassConditions.DefaultIfEmpty()
                          where t1.UnitID == model.UnitID && t1.FormID == model.FormID
                          select new ApproveFormcheckModel
                          {
                              ID = t1.ID,
                              ProjectID = t1.ProjectID,
                              ProjectName = project.ProjectName,
                              UnitID = t1.UnitID,
                              UnitCode = unit.UnitCode,
                              VendorID = t1.VendorID,
                              VenderName = vendor.Name,
                              VendorResourceID = t1.VendorResourceID,
                              Grade = t1.Grade,
                              FormID = t1.FormID,
                              Group_ID = formGroup.ID,
                              Group_Name = formGroup.Name,
                              Remark = action.Remark,
                              LockStatusID = passCondition.LockStatusID
                          }).ToList();

            return result;
        }
    }
}
