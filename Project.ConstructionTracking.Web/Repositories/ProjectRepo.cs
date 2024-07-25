using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class ProjectRepo : IProjectRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public ProjectRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }
        public dynamic GetProjectList()
        {
            try
            {
                var query = from u in _context.tm_Project.Where(e => e.FlagActive == true)
                            select new
                            {
                                u.ProjectID,
                                u.ProjectCode,
                                u.ProjectName
                            };

                var data = query.AsEnumerable().Select(e => new
                {
                    e.ProjectID,
                    e.ProjectCode,
                    e.ProjectName

                }).ToList();

                return data;
            }
            catch (Exception ex)
            {
                return new { ex.Message, error = "Catch ใน Repo" };
            }
        }

        public dynamic SearchProjects(string term)
        {
            try
            {
                var query = from u in _context.tm_Project
                            where u.FlagActive == true && u.ProjectName.Contains(term)
                            select new
                            {
                                u.ProjectID,
                                u.ProjectCode,
                                u.ProjectName
                            };

                var data = query.AsEnumerable().Select(e => new
                {
                    e.ProjectID,
                    e.ProjectCode,
                    e.ProjectName
                }).ToList();

                return data;
            }
            catch (Exception ex)
            {
                return new { ex.Message , error = "Catch ใน Repo" };
            }
        }

    }
}
