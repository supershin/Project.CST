using System;
using System.Text.RegularExpressions;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IMasterCommonRepo
    {
        dynamic GetProjectList(Guid? userId, int RoleID);
    }
	public class MasterCommonRepo : IMasterCommonRepo
	{
        private readonly ContructionTrackingDbContext _context;

        public MasterCommonRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public dynamic GetProjectList(Guid? userId, int RoleID)
        {
            var query = from p in _context.tm_Project
                        where p.FlagActive == true
                        select new
                        {
                            ProjectID = p.ProjectID,
                            ProjectName = p.ProjectName
                        };

            if (userId != Guid.Empty)
            {
                query = from p in _context.tm_Project
                        join pp in _context.tr_ProjectPermission on p.ProjectID equals pp.ProjectID into pGroup
                        from pp in pGroup.DefaultIfEmpty()
                        where p.FlagActive == true && (pp == null || pp.UserID == userId)
                        select new
                        {
                            ProjectID = p.ProjectID,
                            ProjectName = p.ProjectName
                        };
            }

            return query.ToList();
        }
    }
}

