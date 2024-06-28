namespace Project.ConstructionTracking.Web.Models
{
    public class ProjectForm
    {
        public int ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? FormID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? StatusID { get; set; }
    }

    public class ProjectFormGroup
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string Name { get; set; }
    }

    public class ProjectFormPackage
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public string Name { get; set; }
    }

    public class ProjectFormCheckList
    {
        public int ID { get; set; }
        public int PackageID { get; set; }
        public string Name { get; set; }
        public int? CheckListID { get; set; }
        public int? StatusID { get; set; }
        public string Remark { get; set; }
    }

    public class UnitFormDetail
    {
        public int ID { get; set; }
        public int UnitFormID { get; set; }
        public int CheckListID { get; set; }
        public int? CheckListStatusID { get; set; }
        public string Remark { get; set; }
    }

}
