namespace Project.ConstructionTracking.Web.Models.ProjectImage
{
    public class ProjectImageModel
    {
        public class ListProjectImageModel
        {
            public Guid ProjectID { get; set; }
            public string? ProjectCode { get; set; }
            public string? ProjectName { get; set; }
            public Guid? ProjectImageID { get; set; }
            public Guid? ResourceID { get; set; }
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public DateTime? UpdateDate { get; set; }
        }

        public class SaveProjectImageModel
        {
            public Guid ProjectID { get; set; }
            public Guid UserID { get; set; }
            public string? ApplicationPath { get; set; }
            public IFormFile? Image { get; set; }
        }

        public class RemoveProjectImageModel
        {
            public Guid ProjectID { get; set; }
            public Guid UserID { get; set; }
        }
    }
}
