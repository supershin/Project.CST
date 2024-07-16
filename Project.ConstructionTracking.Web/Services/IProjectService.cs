using Project.ConstructionTracking.Web.Models;
namespace Project.ConstructionTracking.Web.Services
{
    public interface IProjectService
    {
        dynamic GetProjectList();
        dynamic SearchProjects(string term);
    }
}
