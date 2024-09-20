using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IProjectRepo
    {
        dynamic GetProjectList(Guid? userID);
        dynamic SearchProjects(string term , Guid? userID);
    }

}
