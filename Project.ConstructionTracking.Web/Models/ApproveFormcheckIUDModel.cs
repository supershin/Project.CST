namespace Project.ConstructionTracking.Web.Models
{
    public class ApproveFormcheckIUDModel
    {
        public Guid? UnitFormID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public Guid? UserID { get; set; }
        public string? UnitCode { get; set; }
        public int? RoleID { get; set; }
        public int? FormID { get; set; }
        public int? Group_ID { get; set; }
        public string? ActionType { get; set; }
        public int? UnitFormStatus { get; set; }
        public string? Remark { get; set; }
        public List<IFormFile>? Images { get; set; }
        public string? ApplicationPath { get; set; }
        public List<PassConditions>? PassConditionsIUD { get; set; }
        public class PassConditions
        {
            public int? PassConditionsID { get; set; }
            public int? PassConditionsvalue { get; set; }
            public int? Group_ID { get; set; }
            public string? Remark { get; set; }
        }
    }
}
