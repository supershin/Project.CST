namespace Project.ConstructionTracking.Web.Models
{
    public class ProjectModel
    {
        public int index { get; set; }
        public string ProjectID { get; set; }
        public int BUID { get; set; }
        public int ProjectTypeID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int FlagActive { get; set; }
        public string CreateDate { get; set; }
        public int CreateBy { get; set; }
        public string UpdateDate { get; set; }
        public int UpdateBy { get; set; }
        public string act { get; set; }  // Ensure this property is present
    }
}
