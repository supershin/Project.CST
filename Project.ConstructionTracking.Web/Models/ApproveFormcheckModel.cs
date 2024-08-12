namespace Project.ConstructionTracking.Web.Models
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
        public int? Group_ID { get; set; }
        public string? Group_Name { get; set; }
        public string? Remark { get; set; }
        public int? LockStatusID { get; set; }
    }
}
