using System;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IMasterProjectRepo
	{
        dynamic GetProjectList(DTParamModel param);
    }

    public class MasterProjectRepo : IMasterProjectRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public MasterProjectRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public dynamic GetProjectList(DTParamModel param)
        {
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            var query = from tmp in _context.tm_Project
                        where tmp.FlagActive == true
                        select new
                        {
                            tmp.ProjectID,
                            tmp.ProjectCode,
                            tmp.ProjectName,
                            tmp.CreateDate,
                            tmp.UpdateDate
                        };

            var result = query.Page(param.start, param.length, i => i.CreateDate, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;

            return result.AsEnumerable().Select(e => new
            {
                ProjectId = e.ProjectID,
                ProjectCode = e.ProjectCode,
                ProjectName = e.ProjectName,
                CreateDate = e.CreateDate,
                UpdateDate = e.UpdateDate
            }).ToList();
        }
    }
}

