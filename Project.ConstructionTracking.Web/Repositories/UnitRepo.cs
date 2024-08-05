using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using System.Linq;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class UnitRepo : IUnitRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public UnitRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<UnitModel> GetUnitList(string Search, UnitModel Model)
        {
            var maxFormIds = _context.tr_UnitForm
                .Where(f => f.FlagActive == true)
                .GroupBy(f => f.UnitID)
                .Select(g => new
                {
                    UnitID = g.Key,
                    FormID = g.Max(f => f.FormID)
                });

            //var query = from t1 in _context.tm_Unit
            //            join t2 in _context.tm_Ext on t1.UnitStatusID equals t2.ID into gj
            //            from subT2 in gj.DefaultIfEmpty()
            //            join t3 in maxFormIds on t1.UnitID equals t3.UnitID into gMaxFormIds
            //            from subT3 in gMaxFormIds.DefaultIfEmpty()
            //            join t4 in _context.tm_Form on subT3.FormID equals t4.ID into gForm
            //            from subT4 in gForm.DefaultIfEmpty()
            //            where t1.ProjectID == Model.ProjectID &&
            //                  (string.IsNullOrEmpty(Search) || t1.UnitCode.Contains(Search)) &&
            //                  (!Model.UnitStatusID.HasValue || t1.UnitStatusID == Model.UnitStatusID)
            //            select new UnitModel
            //            {
            //                UnitID = t1.UnitID,
            //                ProjectID = t1.ProjectID,
            //                UnitCode = t1.UnitCode,
            //                UnitStatusID = t1.UnitStatusID,
            //                UnitStatusName = subT2.Name,
            //                FormID = subT3.FormID,
            //                FormName = subT4.Name
            //            };

            var query = from t1 in _context.tm_Unit
                        where t1.FlagActive == true
                        join t2 in _context.tm_Ext on t1.UnitStatusID equals t2.ID into gj
                        from subT2 in gj.DefaultIfEmpty()
                        where subT2.FlagActive == true
                        join t3 in maxFormIds on t1.UnitID equals t3.UnitID into gMaxFormIds
                        from subT3 in gMaxFormIds.DefaultIfEmpty()
                        join t4 in _context.tm_Form on subT3.FormID equals t4.ID into gForm
                        from subT4 in gForm.DefaultIfEmpty()
                        //where subT4.FlagActive == true
                        where t1.ProjectID == Model.ProjectID &&
                              (string.IsNullOrEmpty(Search) || t1.UnitCode.Contains(Search)) &&
                              (!Model.UnitStatusID.HasValue || t1.UnitStatusID == Model.UnitStatusID)
                        orderby t1.UnitCode descending
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

            return query.ToList();
        }

        public dynamic GetUnitTypeList()
        {
            var query = from u in _context.tm_UnitType.Where(e => e.FlagActive == true)
                        select new
                        {
                            u.ID,
                            u.Name
                        };

            return query.AsEnumerable().Select(e => new
            {
                e.ID,
                e.Name
            }).ToList();
        }
    }
}
