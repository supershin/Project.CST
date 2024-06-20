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

        public dynamic GetProjectList()
        {
            var Projectlist = _ProjectRepo.GetProjectList();
            return Projectlist;
        }
    }
}
