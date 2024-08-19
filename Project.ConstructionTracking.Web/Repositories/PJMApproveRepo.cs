using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

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
                                join t5 in _context.tr_UnitFormAction on t1.ID equals t5.UnitFormID into unitFormActions
                                from unitFormAction in unitFormActions.DefaultIfEmpty()
                                join t7 in _context.tm_Form on t1.FormID equals t7.ID into forms
                                from form in forms.DefaultIfEmpty()
                                where unitFormAction.ActionType == filterData.ActionType
                                        && unitFormAction.StatusID == filterData.StatusID
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
                                    StatusID = unitFormAction.StatusID
                                }).ToList();

            // Second Query: Get Checklist PC
            var checklistPCs = (from t1 in _context.tr_UnitForm
                                join t5 in _context.tr_UnitFormAction on t1.ID equals t5.UnitFormID into unitFormActions
                                from unitFormAction in unitFormActions.DefaultIfEmpty()
                                join t6 in _context.tm_FormGroup on t1.FormID equals t6.FormID into formGroups
                                from formGroup in formGroups.DefaultIfEmpty()
                                join t8 in _context.tr_UnitFormPassCondition on new { unitFormAction.UnitFormID, GroupID = (int?)formGroup.ID } equals new { t8.UnitFormID, t8.GroupID } into passConditions
                                from passCondition in passConditions.DefaultIfEmpty()
                                where unitFormAction.ActionType == filterData.ActionType && unitFormAction.StatusID == filterData.StatusID && passCondition.ID != null
                                orderby t1.UnitID, t1.FormID, formGroup.ID
                                select new PJMApproveModel.GetlistChecklistPC
                                {
                                    FormID = t1.FormID,
                                    GroupID = formGroup.ID,
                                    GroupName = formGroup.Name,
                                    PassConditionID = passCondition.ID,
                                    PE_Remark = passCondition.PE_Remark,
                                    PM_Remark = passCondition.PM_Remark
                                }).ToList();

            // Group checklistPCs by FormID
            var checklistGroupedByFormID = checklistPCs.GroupBy(c => c.FormID).ToDictionary(g => g.Key, g => g.ToList());

            // Map ChecklistPC to the corresponding Unit Details
            foreach (var unitDetail in unitDetails)
            {
                if (unitDetail.FormID.HasValue && checklistGroupedByFormID.TryGetValue(unitDetail.FormID.Value, out var checklistPC))
                {
                    unitDetail.ChecklistPC = checklistPC;
                }
            }

            return unitDetails;
        }



        //public List<PJMApproveModel.GetlistUnitDetail> GetListPJMApprove(PJMApproveModel.filterData filterData)
        //{
        //    //// First query to get UnitForm details
        //    //var unitDetails = (from t1 in _context.tr_UnitForm
        //    //                   join t2 in _context.tm_Project on t1.ProjectID equals t2.ProjectID into projects
        //    //                   from project in projects.DefaultIfEmpty()
        //    //                   join t3 in _context.tm_Unit on t1.UnitID equals t3.UnitID into units
        //    //                   from unit in units.DefaultIfEmpty()
        //    //                   join t4 in _context.tm_Vendor on t1.VendorID equals t4.ID into vendors
        //    //                   from vendor in vendors.DefaultIfEmpty()
        //    //                   join t5 in _context.tr_UnitFormAction on t1.ID equals t5.UnitFormID into unitFormActions
        //    //                   from unitFormAction in unitFormActions.DefaultIfEmpty()
        //    //                   join t7 in _context.tm_Form on t1.FormID equals t7.ID into forms
        //    //                   from form in forms.DefaultIfEmpty()
        //    //                   where unitFormAction.ActionType == filterData.ActionType && unitFormAction.StatusID == filterData.StatusID
        //    //                   orderby unit.UnitCode, t1.FormID
        //    //                   select new PJMApproveModel.GetlistUnitDetail
        //    //                   {
        //    //                       UnitFormID = t1.ID,
        //    //                       UnitFormActionID = unitFormAction.ID,
        //    //                       UnitCode = unit.UnitCode,
        //    //                       ProjectName = project.ProjectName,
        //    //                       VendorID = t1.VendorID,
        //    //                       VendorName = vendor.Name,
        //    //                       FormID = t1.FormID,
        //    //                       FormName = form.Name,
        //    //                       RoleID = unitFormAction.RoleID,
        //    //                       ActionType = unitFormAction.ActionType,
        //    //                       StatusID = unitFormAction.StatusID
        //    //                   }).ToList();

        //    //// Second query to get ChecklistPC details for each UnitForm
        //    //var checklistPCs = (from t1 in _context.tr_UnitForm
        //    //                    join t6 in _context.tm_FormGroup on t1.FormID equals t6.FormID into formGroups
        //    //                    from formGroup in formGroups.DefaultIfEmpty()
        //    //                    join t8 in _context.tr_UnitFormPassCondition on new { UnitFormID = (Guid?)t1.ID, GroupID = formGroup.ID } equals new { t8.UnitFormID, GroupID = t8.ID } into passConditions
        //    //                    from passCondition in passConditions.DefaultIfEmpty()
        //    //                    where passCondition != null
        //    //                    orderby t1.ID, formGroup.ID
        //    //                    select new
        //    //                    {
        //    //                        t1.FormID,
        //    //                        Checklist = new PJMApproveModel.GetlistChecklistPC
        //    //                        {
        //    //                            FormID = t1.FormID,
        //    //                            GroupID = formGroup.ID,
        //    //                            GroupName = formGroup.Name,
        //    //                            PassConditionID = passCondition.ID,
        //    //                            PE_Remark = passCondition.PE_Remark,
        //    //                            PM_Remark = passCondition.PM_Remark
        //    //                        }
        //    //                    }).ToList();

        //    //// Group the checklist items by FormID
        //    //var checklistGroupedByFormID = checklistPCs.GroupBy(x => x.FormID).ToDictionary(g => g.Key, g => g.Select(x => x.Checklist).ToList());

        //    //// Map the grouped ChecklistPC data to the corresponding UnitForm details
        //    //foreach (var unitDetail in unitDetails)
        //    //{
        //    //    if (unitDetail.FormID.HasValue && checklistGroupedByFormID.ContainsKey(unitDetail.FormID.Value))
        //    //    {
        //    //        unitDetail.ChecklistPC = checklistGroupedByFormID[unitDetail.FormID.Value];
        //    //    }
        //    //    else
        //    //    {
        //    //        unitDetail.ChecklistPC = new List<PJMApproveModel.GetlistChecklistPC>(); // Ensure it's initialized as an empty list
        //    //    }
        //    //}

        //    //return unitDetails;

        //    var unitDetails = (from t1 in _context.tr_UnitForm
        //                       join t2 in _context.tm_Project on t1.ProjectID equals t2.ProjectID into projects
        //                       from project in projects.DefaultIfEmpty()
        //                       join t3 in _context.tm_Unit on t1.UnitID equals t3.UnitID into units
        //                       from unit in units.DefaultIfEmpty()
        //                       join t4 in _context.tm_Vendor on t1.VendorID equals t4.ID into vendors
        //                       from vendor in vendors.DefaultIfEmpty()
        //                       join t5 in _context.tr_UnitFormAction on t1.ID equals t5.UnitFormID into unitFormActions
        //                       from unitFormAction in unitFormActions.DefaultIfEmpty()
        //                       join t7 in _context.tm_Form on t1.FormID equals t7.ID into forms
        //                       from form in forms.DefaultIfEmpty()
        //                       where unitFormAction.ActionType == filterData.ActionType
        //                          && unitFormAction.StatusID == filterData.StatusID
        //                       orderby unit.UnitCode, t1.FormID
        //                       select new PJMApproveModel.GetlistUnitDetail
        //                       {
        //                           UnitFormID = t1.ID,
        //                           UnitFormActionID = unitFormAction.ID,
        //                           UnitCode = unit.UnitCode,
        //                           ProjectName = project.ProjectName,
        //                           VendorID = t1.VendorID,
        //                           VendorName = vendor.Name,
        //                           FormID = t1.FormID,
        //                           FormName = form.Name,
        //                           RoleID = unitFormAction.RoleID,
        //                           ActionType = unitFormAction.ActionType,
        //                           StatusID = unitFormAction.StatusID
        //                       }).ToList();

        //    // Second query to get ChecklistPC details for each UnitForm
        //    foreach (var unitDetail in unitDetails)
        //    {
        //        unitDetail.ChecklistPC = (from t1 in _context.tr_UnitForm
        //                                  join t6 in _context.tm_FormGroup on t1.FormID equals t6.FormID into formGroups
        //                                  from formGroup in formGroups.DefaultIfEmpty()
        //                                  join t8 in _context.tr_UnitFormPassCondition on new { UnitFormID = (Guid?)t1.ID, GroupID = formGroup.ID } equals new { UnitFormID = unitDetail.UnitFormID, GroupID = t8.ID } into passConditions
        //                                  from passCondition in passConditions.DefaultIfEmpty()
        //                                  where passCondition != null
        //                                  orderby formGroup.ID
        //                                  select new PJMApproveModel.GetlistChecklistPC
        //                                  {
        //                                      FormID = t1.FormID,
        //                                      GroupID = formGroup.ID,
        //                                      GroupName = formGroup.Name,
        //                                      PassConditionID = passCondition.ID,
        //                                      PE_Remark = passCondition.PE_Remark,
        //                                      PM_Remark = passCondition.PM_Remark
        //                                  }).ToList();
        //    }

        //    return unitDetails;

        //}
    }
}

