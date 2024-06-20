using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface ITrackingService
    {
        TrackingUnitView GetTrackingUnit(Guid unitID);

    }
}
