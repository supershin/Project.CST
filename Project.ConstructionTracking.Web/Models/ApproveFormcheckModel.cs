﻿namespace Project.ConstructionTracking.Web.Models
{
    public class ApproveFormcheckModel
    {
        public Guid? ID { get; set; }
        public Guid? ProjectID { get; set; }
        public string? ProjectName { get; set; }
        public Guid? UnitID { get; set; }
        public string? UnitCode { get; set; }
        public int? VendorID { get; set; }
        public string? VenderName { get; set; }
        public Guid? VendorResourceID { get; set; }
        public string? Grade { get; set; }
        public int? FormID { get; set; }
        public string? FormName { get; set; }
        public DateTime? Actiondate { get; set; }
        public DateTime? ActiondatePm { get; set; }
        public int? PM_StatusID { get; set; }
        public string? PM_Remarkaction { get; set; }
        public string? PM_Actiontype { get; set; }
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
        public string? PE_Remark { get; set; }
        public string? PM_Remark { get; set; }
        public Guid? ResourceID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

    }

    public class PM_getListImage
    {
        public Guid? ResourceID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

    }
}