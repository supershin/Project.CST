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
            public int? StatusID { get; set; }
            public List<GetlistChecklistPC> ?ChecklistPC { get; set; }
        }
        public class GetlistChecklistPC
        {
            public int? FormID { get; set; }
            public int? GroupID { get; set; }
            public string? GroupName { get; set; }
            public int? PassConditionID { get; set; }
            public string? PE_Remark { get; set; }
            public string? PM_Remark { get; set; }
        }
    }
}
