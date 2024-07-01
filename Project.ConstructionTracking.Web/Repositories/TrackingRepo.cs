using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using System.Text.RegularExpressions;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class TrackingRepo : ITrackingRepo
    {
        private readonly ContructionTrackingDbContext _context;
        public TrackingRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }
   
        public List<TrackingUnitModel> GetTrackingUnitList(UnitModel unit)
        {
            var query = from t1 in _context.tr_ProjectForm
                        where t1.ProjectID == unit.ProjectID && t1.UnitTypeID == unit.UnitTypeID
                              && t1.FormTypeID == SystemConstant.Ext.FORM_TYPE_PE && t1.FormRefID == null
                        join t2 in _context.tr_ProjectForm on t1.ID equals t2.FormRefID into _t2Group
                        from t2Group in _t2Group.DefaultIfEmpty()
                        select new
                        {
                            t1,
                            t2Group
                        };

            // Materialize the first query results into memory
            var queryResult = query.ToList();

            var query2 = from t1 in _context.tr_UnitForm
                         where t1.UnitID == unit.UnitID
                         join t2 in _context.tm_Unit on t1.UnitID equals t2.UnitID into _t2Group
                         from t2Group in _t2Group.DefaultIfEmpty()
                         select new
                         {
                             t1,
                             t2Group
                         };

            var queryResult2 = query2.ToList();

            List<TrackingUnitModel> results = new();
            DateTime? previousEndDate = null;

            foreach (var item in queryResult)
            {
                var t4Group = queryResult2.FirstOrDefault(x => x.t1.FormID == item.t1.ID);
                var t3Group = t4Group?.t2Group;

                DateTime? startDate;
                if (previousEndDate == null)
                {
                    startDate = t3Group?.StartDate;
                }
                else
                {
                    startDate = previousEndDate;
                }

                DateTime? endDate = startDate.HasValue ? startDate.Value.AddDays((double)item.t1.Duration) : (DateTime?)null;

                results.Add(new TrackingUnitModel
                {
                    ID = item.t1.ID,
                    FormName = item.t1.Name,
                    Duration = item.t1.Duration,
                    Progress = item.t1.Progress,
                    StatusID = (t4Group?.t1.StatusID == null) ? 13 : t4Group.t1.StatusID,
                    StartDate = startDate.ToStringDate(), // Handle null case
                    EndDate = endDate.ToStringDate(), // Handle null case   
                    FormRef = (item.t2Group != null) ? new TrackingUnitRef
                    {
                        ID = item.t2Group.ID,
                        FormName = item.t2Group.Name,
                        Duration = item.t2Group.Duration,
                        Progress = item.t2Group.Progress,
                    } : new TrackingUnitRef()
                });

                previousEndDate = endDate;
            }

            return results;
        }


        public UnitModel GetUnit(Guid unitID)
        {
            var query = from u in _context.tm_Unit.Where(e=>e.UnitID == unitID)
                        join up in _context.tm_Project on u.ProjectID equals up.ProjectID
                        join ut in _context.tm_UnitType on u.UnitTypeID equals ut.ID
                        select new { u, up , ut };

            return query.AsEnumerable().Select(e => new UnitModel
            {
                ProjectID = e.u.ProjectID,
                ProjectName = e.up.ProjectName,
                UnitID = e.u.UnitID,
                UnitCode = e.u.UnitCode,
                UnitTypeID = e.u.UnitTypeID,
                UnitTypeName = e.ut.Name,
                Area = e.u.Area,
                StartDate = e.u.StartDate.ToStringDate(),
                EndDate = e.u.EndDate.ToStringDate(),

            }).FirstOrDefault()??new UnitModel();
        }
    }
}
