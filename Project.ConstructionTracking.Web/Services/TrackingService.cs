using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ITrackingRepo _TrackingRepo;

        public TrackingService(ITrackingRepo TrackingRepo)
        {
            _TrackingRepo = TrackingRepo;
        }
        public TrackingUnitView GetTrackingUnit(Guid unitID)
        {
            var trackingUnitView = new TrackingUnitView();
            trackingUnitView.Unit = _TrackingRepo.GetUnit(unitID);
            trackingUnitView.TrackingUnitList = _TrackingRepo.GetTrackingUnitList(trackingUnitView.Unit);
            return trackingUnitView;
        }
    }
}
