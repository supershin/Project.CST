namespace Project.ConstructionTracking.Web.Models
{
    public class PJMApproveModel
    {
        public class filterData
        {
            public string? ActionType { get; set; }
            public int? StatusID { get; set; }
        }
        public class GetlistUnitDetail
        {
            public Guid? UnitFormID { get; set; }
            public int UnitFormActionID { get; set; }
            public string? UnitCode { get; set; }
            public string? ProjectName { get; set; }
            public int? VendorID { get; set; }
            public string? VendorName { get; set; }
            public int? FormID { get; set; }
            public string? FormName { get; set; }
            public int? RoleID { get; set; }
            public string? ActionType { get; set; }
            public string? PJMActionType { get; set; }
            public string? ActionDate { get; set; }
            public int? StatusID { get; set; }
        }
        public class GetlistChecklistPC
        {
            public Guid? UnitFormID { get; set; }
            public Guid? ProjectID { get; set; }
            public string? ProjectName { get; set; }
            public string? UnitCode { get; set; }
            public string? FormName { get; set; }
            public string? Grade { get; set; }
            public int? GroupID { get; set; }
            public string? GroupName { get; set; }
            public int? PC_ID { get; set; }
            public int? LockStatusID { get; set; }
            public int? PC_StatusID { get; set; }
            public string? PE_Remark { get; set; }
            public string? PM_Remark { get; set; }
            public string? PJM_Remark { get; set; }
            public int? PJM_StatusID { get; set; }
            public string? PJMUnitFormRemark { get; set; }
        }
    }
}
