using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

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
                        select new { t1, t2Group };

            var data = query.Select(e => new TrackingUnitModel
            {
                ID = e.t1.ID,
                FormName = e.t1.Name,
                Duration = e.t1.Duration,
                Progress = e.t1.Progress,
                FormRef = (e.t2Group != null) ? new TrackingUnitRef
                {
                    ID = e.t2Group.ID,
                    FormName = e.t2Group.Name,
                    Duration = e.t2Group.Duration,
                    Progress = e.t2Group.Progress,
                } : new TrackingUnitRef()
            }).ToList();


            return data;
        }
        public UnitModel GetUnit(Guid unitID)
        {
            var query = from u in _context.tm_Unit.Where(e=>e.UnitID == unitID)
                        join ut in _context.tm_UnitType
                            on u.UnitTypeID equals ut.ID
                        select new { u, ut };

            return query.AsEnumerable().Select(e => new UnitModel
            {
                ProjectID = e.u.ProjectID,
                UnitID = e.u.UnitID,
                UnitCode = e.u.UnitCode,
                UnitTypeID = e.u.UnitTypeID,
                UnitTypeName = e.ut.Name,
                StartDate = e.u.StartDate
            }).FirstOrDefault()??new UnitModel();
        }
    }
}
