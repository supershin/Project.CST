using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface ITrackingRepo
    {
        List<TrackingUnitModel> GetTrackingUnitList(UnitModel unit);
        UnitModel GetUnit(Guid unitID);
    }
}
