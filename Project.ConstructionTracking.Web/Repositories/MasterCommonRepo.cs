using System;
using System.Text.RegularExpressions;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IMasterCommonRepo
    {
        dynamic GetProjectList(Guid? userId, int RoleID);

        dynamic GetProjectAndUnit(Guid projectId, Guid unitId);
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

        public dynamic GetProjectAndUnit(Guid projectId, Guid unitId)
        {
            var query = (from u in _context.tm_Unit
                         join p in _context.tm_Project on u.ProjectID equals p.ProjectID
                         join e in _context.tm_Ext on u.UnitStatusID equals e.ID
                         where p.ProjectID == projectId && p.FlagActive == true
                         && u.UnitID == unitId && u.FlagActive == true
                         select new
                         {
                             p.ProjectID,
                             p.ProjectName,
                             u.UnitID,
                             e.Name,
                             u.UnitCode
                         }).FirstOrDefault();

            return query;
        }
    }
}

