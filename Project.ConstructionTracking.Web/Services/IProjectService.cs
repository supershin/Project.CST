using Project.ConstructionTracking.Web.Models;
namespace Project.ConstructionTracking.Web.Services
{
    public interface IProjectService
    {
        dynamic GetProjectList(Guid? userID);
        dynamic SearchProjects(string term, Guid? userID);
    }
}
