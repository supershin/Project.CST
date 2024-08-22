using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using static Project.ConstructionTracking.Web.Models.PJMApproveModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class PJMApproveRepo : IPJMApproveRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public PJMApproveRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<PJMApproveModel.GetlistUnitDetail> GetListPJMApprove(PJMApproveModel.filterData filterData)
        {
            // First Query: Get Unit Details
            var unitDetails = (from t1 in _context.tr_UnitForm
                                join t2 in _context.tm_Project on t1.ProjectID equals t2.ProjectID into projects
                                from project in projects.DefaultIfEmpty()
                                join t3 in _context.tm_Unit on t1.UnitID equals t3.UnitID into units
                                from unit in units.DefaultIfEmpty()
                                join t4 in _context.tm_Vendor on t1.VendorID equals t4.ID into vendors
                                from vendor in vendors.DefaultIfEmpty()
                                join t5 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)2 } equals new { t5.UnitFormID, t5.RoleID } into unitFormActions
                                from unitFormAction in unitFormActions.DefaultIfEmpty()
                                join tpjm in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)3 } equals new { tpjm.UnitFormID, tpjm.RoleID } into PJmunitFormActions
                                from PJmunitFormAction in PJmunitFormActions.DefaultIfEmpty()
                                join t7 in _context.tm_Form on t1.FormID equals t7.ID into forms
                                from form in forms.DefaultIfEmpty()
                                where unitFormAction.ActionType == filterData.ActionType && t1.StatusID > filterData.StatusID
                                orderby unit.UnitCode, t1.FormID
                                select new PJMApproveModel.GetlistUnitDetail
                                {
                                    UnitFormID = t1.ID,
                                    UnitFormActionID = unitFormAction.ID,
                                    ProjectName = project.ProjectName,
                                    UnitCode = unit.UnitCode,
                                    VendorID = t1.VendorID,
                                    VendorName = vendor.Name,
                                    FormID = t1.FormID,
                                    FormName = form.Name,
                                    RoleID = unitFormAction.RoleID,
                                    ActionType = unitFormAction.ActionType,
                                    PJMActionType = PJmunitFormAction.ActionType,
                                    ActionDate = unitFormAction.ActionDate.HasValue
                                    ? unitFormAction.ActionDate.Value.ToString("dd/MM/yyyy")
                                    : "", // Convert to string dd/MM/yyyy
                                    StatusID = unitFormAction.StatusID
                                }).ToList();
            return unitDetails;
        }

        public List<PJMApproveModel.GetlistChecklistPC> GetChecklistPJMApprove(PJMApproveModel.GetlistChecklistPC filterData)
        {
            var result = (from t1 in _context.tr_UnitForm
                          join t2 in _context.tm_FormGroup on t1.FormID equals t2.FormID into formGroups
                          from t2 in formGroups.DefaultIfEmpty()
                          join t3 in _context.tr_UnitFormPassCondition on new { UnitFormID = (Guid?)t1.ID, GroupID = (int?)t2.ID } equals new { t3.UnitFormID, t3.GroupID } into passConditions
                          from t3 in passConditions.Where(pc => pc.FlagActive == true).DefaultIfEmpty()
                          join t4 in _context.tm_Project on t1.ProjectID equals t4.ProjectID into projects
                          from t4 in projects.DefaultIfEmpty()
                          join t5 in _context.tm_Unit on t1.UnitID equals t5.UnitID into units
                          from t5 in units.DefaultIfEmpty()
                          join t6 in _context.tm_Form on t1.FormID equals t6.ID into forms
                          from t6 in forms.DefaultIfEmpty()
                          join t8 in _context.tr_UnitFormAction.Where(a => a.RoleID == 3)
                              on t1.ID equals t8.UnitFormID into actions
                          from t8 in actions.DefaultIfEmpty()
                          where t1.ID == filterData.UnitFormID && t3.ID != null && t1.StatusID > 5
                          select new GetlistChecklistPC
                          {
                              UnitFormID = t1.ID,
                              ProjectID = t1.ProjectID,
                              ProjectName = t4.ProjectName,
                              UnitCode = t5.UnitCode, 
                              FormName = t6.Name,
                              Grade = t1.Grade,
                              GroupID = t2.ID,
                              GroupName = t2.Name,
                              PC_ID = t3.ID,
                              LockStatusID = t3.LockStatusID,
                              PC_StatusID = t3.StatusID,
                              PE_Remark = t3.PE_Remark,
                              PM_Remark = t3.PM_Remark,
                              PJM_Remark = t3.PJM_Remark,
                              PJM_StatusID = t8.StatusID,
                              PJMUnitFormRemark = t8.Remark
                          }).ToList();

            return result;
        }


    }
}

