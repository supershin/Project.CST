using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class UnitRepo : IUnitRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public UnitRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }
        public dynamic GetUnitList(Criteria criteria, DTParamModel param)
        {
            var query = from u in _context.tm_Unit.Where(e => e.FlagActive == true)
                        join ut in _context.tm_UnitType
                            on u.UnitTypeID equals ut.ID
                        where u.UnitCode.Contains(criteria.Search) || criteria.Search == null
                           && u.ProjectID == criteria.ProjectID && criteria.ProjectID == null
                        select new
                        {
                            u.UnitID,
                            u.ProjectID,
                            u.UnitTypeID,
                            UnitTypeName = ut.Name,
                            u.UnitCode,
                            u.Build,
                            u.Floor,
                            u.Block,
                            u.Area,
                            u.StartDate,
                            u.EndDate,                           
                            u.FlagActive,
                            u.CreateDate,
                            u.CreateBy,
                            u.UpdateDate,
                            u.UpdateBy
                        };

            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");
            criteria.Search = criteria.Search.ToStringNullable() ?? string.Empty;

            var result = query.Page(param.start, param.length, i => i.UpdateDate, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;

            var data = result.AsEnumerable().Select(e => new
            {
                e.UnitID,
                e.ProjectID,
                e.UnitTypeID,
                e.UnitTypeName,
                e.UnitCode,
                e.Build,
                e.Floor,
                e.Block,
                e.Area,
                StartDate = e.StartDate.ToStringDate(),
                EndDate = e.EndDate.ToStringDate(),               
                e.FlagActive,
                e.CreateDate,
                e.CreateBy,
                UpdateDate = e.UpdateDate.ToStringDateTime(),
                e.UpdateBy
            }).ToList();
         
            return data;
        }
    }
}
