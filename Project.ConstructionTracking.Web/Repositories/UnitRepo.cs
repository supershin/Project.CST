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

        public List<UnitModel> GetUnitList(string Search, UnitModel Model)
        {
            var maxFormIds = _context.tr_UnitForm
                .GroupBy(f => f.UnitID)
                .Select(g => new
                {
                    UnitID = g.Key,
                    FormID = g.Max(f => f.FormID)
                });

            var query = from t1 in _context.tm_Unit
                        join t2 in _context.tm_Ext on t1.UnitStatusID equals t2.ID into gj
                        from subT2 in gj.DefaultIfEmpty()
                        join t3 in maxFormIds on t1.UnitID equals t3.UnitID into gMaxFormIds
                        from subT3 in gMaxFormIds.DefaultIfEmpty()
                        join t4 in _context.tm_Form on subT3.FormID equals t4.ID into gForm
                        from subT4 in gForm.DefaultIfEmpty()
                        where t1.ProjectID == Model.ProjectID
                        select new UnitModel
                        {
                            UnitID = t1.UnitID,
                            ProjectID = t1.ProjectID,
                            UnitCode = t1.UnitCode,
                            UnitStatusID = t1.UnitStatusID,
                            UnitStatusName = subT2.Name,
                            FormID = subT3.FormID,
                            FormName = subT4.Name
                        };

            var result = query.ToList();

            return result;
        }




        public dynamic GetUnitTypeList()
        {
            var query = from u in _context.tm_UnitType.Where(e => e.FlagActive == true)
                        select new
                        {
                            u.ID,
                            u.Name
                        };

            var data = query.AsEnumerable().Select(e => new
            {
                e.ID,
                e.Name

            }).ToList();

            return data;
        }
    }
}
