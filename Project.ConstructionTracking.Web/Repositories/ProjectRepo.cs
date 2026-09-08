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
        public dynamic GetProjectList(Guid? userID)
        {
            try
            {
                var query = from u in _context.tm_Project.Where(e => e.FlagActive == true)
                            join u2 in _context.tr_ProjectPermission.Where(p => p.FlagActive == true) on u.ProjectID equals u2.ProjectID into u2Group
                            from u2 in u2Group.DefaultIfEmpty()
                            join pi in _context.tr_ProjectImage.Where(p => p.FlagActive == true) on u.ProjectID equals pi.ProjectID into piGroup
                            from pi in piGroup.DefaultIfEmpty()
                            join r in _context.tm_Resource.Where(e => e.FlagActive == true) on pi.ResourceID equals r.ID into rGroup
                            from r in rGroup.DefaultIfEmpty()
                            where u2.UserID == userID
                            select new
                            {
                                u.ProjectID,
                                u.ProjectCode,
                                u.ProjectName,
                                ImagePath = r != null ? r.FilePath : null
                            };

                var data = query.AsEnumerable().Select(e => new
                {
                    e.ProjectID,
                    e.ProjectCode,
                    e.ProjectName,
                    e.ImagePath

                }).ToList();

                return data;
            }
            catch (Exception ex)
            {
                return new { ex.Message, error = "Catch ใน Repo" };
            }
        }

        public dynamic SearchProjects(string term, Guid? userID)
        {
            try
            {
                var query = from u in _context.tm_Project
                            join u2 in _context.tr_ProjectPermission.Where(p => p.FlagActive == true) on u.ProjectID equals u2.ProjectID into u2Group
                            from u2 in u2Group.DefaultIfEmpty()
                            join pi in _context.tr_ProjectImage.Where(p => p.FlagActive == true) on u.ProjectID equals pi.ProjectID into piGroup
                            from pi in piGroup.DefaultIfEmpty()
                            join r in _context.tm_Resource.Where(e => e.FlagActive == true) on pi.ResourceID equals r.ID into rGroup
                            from r in rGroup.DefaultIfEmpty()
                            where u.FlagActive == true
                               && u2.UserID == userID
                               && u.ProjectName.Contains(term)
                            select new
                            {
                                u.ProjectID,
                                u.ProjectCode,
                                u.ProjectName,
                                ImagePath = r != null ? r.FilePath : null
                            };

                var data = query.AsEnumerable().Select(e => new
                {
                    e.ProjectID,
                    e.ProjectCode,
                    e.ProjectName,
                    e.ImagePath
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
