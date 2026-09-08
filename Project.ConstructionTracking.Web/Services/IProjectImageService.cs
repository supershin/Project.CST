using Project.ConstructionTracking.Web.Models.ProjectImage;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IProjectImageService
    {
        List<ProjectImageModel.ListProjectImageModel> GetProjectImageList(string? strSearch);
        string? GetProjectImagePath(Guid projectID);
        void SaveProjectImage(ProjectImageModel.SaveProjectImageModel model);
        void RemoveProjectImage(ProjectImageModel.RemoveProjectImageModel model);
    }
}
