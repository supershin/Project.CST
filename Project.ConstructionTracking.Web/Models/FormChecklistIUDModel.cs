namespace Project.ConstructionTracking.Web.Models
{
    public class FormChecklistIUDModel
    {
        public List<PackageModel> Packages { get; set; }
        public List<ChecklistModel> CheckLists { get; set; }
        public PassConditionCheckModel PcCheck { get; set; }
    }

    public class PackageModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int RoleID { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UnitId { get; set; }
        public Guid UnitFormID { get; set; }
        public int UnitFormActionID { get; set; }
        public int UnitPackageID { get; set; }
        public int FormID { get; set; }
        public int GroupID { get; set; }
        public int PackageID { get; set; }
        public string Remark { get; set; }
    }
    public class ChecklistModel
    {
        public string UserName { get; set; }
        public int FormID { get; set; }
        public int GroupID { get; set; }
        public int PackageID { get; set; }
        public int UnitPackageID { get; set; }
        public int CheckListID { get; set; }
        public int UnitChecklistID { get; set; }
        public int RadioValue { get; set; }
    }
    public class PassConditionCheckModel
    {
        public int GroupID { get; set; }
        public string Remark { get; set; }
        public Guid UnitFormID { get; set; }
    }
}
