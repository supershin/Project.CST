namespace Project.ConstructionTracking.Web.Models
{
    public class ApproveFormcheckModel
    {
        public Guid? ID { get; set; }
        public Guid? ProjectID { get; set; }
        public string? ProjectName { get; set; }
        public Guid? UnitID { get; set; }
        public Guid? UnitFormID { get; set; }
        public string? UnitCode { get; set; }
        public int? VendorID { get; set; }
        public string? VenderName { get; set; }
        public Guid? VendorResourceID { get; set; }
        public string? Grade { get; set; }
        public int? FormID { get; set; }
        public int? UnitFormStatusID { get; set; }
        public string? FormName { get; set; }
        public DateTime? Actiondate { get; set; }
        public DateTime? ActiondatePm { get; set; }
        public DateTime? ActiondatePJm { get; set; }
        public int? PM_StatusID { get; set; }
        public string? PM_Remarkaction { get; set; }
        public string? PM_Actiontype { get; set; }
        public string? PM_ActionBy { get; set; }
        public int? PJM_StatusID { get; set; }
        public string? PJM_Remarkaction { get; set; }
        public string? PJM_Actiontype { get; set; }
        public int? PCAllcount { get; set; }
        public int? PCPassCount { get; set; }
        public List<PM_getListgroup>? PM_getListgroup { get; set; }
        public List<PM_getListImage>? PM_getListImage { get; set; }
    }

    public class PM_getListgroup
    {
        public int? Group_ID { get; set; }
        public string? Group_Name { get; set; }
        public int? PassConditionsID { get; set; }
        public int? PC_StatusID { get; set; }
        public int? LockStatusID { get; set; }
        public string? PE_RemarkPC { get; set; }
        public string? PM_RemarkPC { get; set; }
        public string? PJM_RemarkPC { get; set; }
        public int? PCAllcount { get; set; }
        public int? PCPassCount { get; set; }
        public bool? PCFlageActive { get; set; }
        public Guid? ResourceID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public List<PM_getListpackage>? PM_getListpackage { get; set; }
    }

    public class PM_getListpackage
    {
        public int? Package_ID { get; set; }
        public string? Package_Name { get; set; }
        public string? Package_Remark { get; set; }
    }
    public class PM_getListImage
    {
        public Guid? ResourceID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

    }
}
