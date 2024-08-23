namespace Project.ConstructionTracking.Web.Models
{
    public class UnitFormResourceModel
    {
        public int UnitFormResourceID { get; set; }
        public Guid? ResourceID { get; set; }
        public Guid MasterResourceID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public int RoleID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? GroupID { get; set; }
        public int? FormID { get; set; }
    }
}
