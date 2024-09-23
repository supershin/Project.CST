using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;
namespace Project.ConstructionTracking.Web.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepo _ProjectRepo;

        public ProjectService(IProjectRepo ProjectRepo)
        {
            _ProjectRepo = ProjectRepo;
        }

        public dynamic GetProjectList(Guid? userID)
        {
            var Projectlist = _ProjectRepo.GetProjectList(userID);
            return Projectlist;
        }

        public dynamic SearchProjects(string term, Guid? userID)
        {
            var projects = _ProjectRepo.SearchProjects(term , userID);
            return projects;
        }
    }
}
