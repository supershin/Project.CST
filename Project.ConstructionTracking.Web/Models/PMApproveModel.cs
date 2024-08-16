namespace Project.ConstructionTracking.Web.Models
{
    public class PMApproveModel
    {
        public Guid? UnitFormID { get; set; }
        public int? UnitFormActionID { get; set; }
        public Guid? ProjectID { get; set; }
        public string? ProjectName { get; set; }
        public Guid? UnitID { get; set; }
        public string? UnitCode { get; set; }
        public int? VendorID { get; set; }
        public string? VenderName { get; set; }
        public string? Grade { get; set; }
        public int? FormID { get; set; }
        public string? FormName { get; set; }
        public int? StatusID { get; set; }
        public int FlagActive { get; set; }
        public int? PassConditionID { get; set; }
        public int? RoleID_PE { get; set; }
        public string? ActionType_PE { get; set; }
        public int? StatusID_PE { get; set; }
        public string? Remark_PE { get; set; }
        public string? ActionDate_PE { get; set; }
        public int? RoleID_PM { get; set; }
        public string? ActionType_PM { get; set; }
        public int? StatusID_PM { get; set; }
        public string? Remark_PM { get; set; }
        public DateTime? ActionDate_PM { get; set; }
    }
}
