using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitdetailController : Controller
    {
        private readonly ITrackingService _TrackingService;

        public UnitdetailController(ITrackingService trackingService)
        {
            _TrackingService = trackingService;
        }

        public IActionResult index(Guid ID)
        {
            TrackingUnitView viewModel = _TrackingService.GetTrackingUnit(ID);
            return View(viewModel);
        }
    }
}
