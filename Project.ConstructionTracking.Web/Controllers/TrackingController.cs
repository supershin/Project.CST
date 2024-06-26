using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;


namespace Project.ConstructionTracking.Web.Controllers
{
    public class TrackingController : Controller
    {
        private readonly ITrackingService _TrackingService;

        public TrackingController(ITrackingService trackingService)
        {
            _TrackingService = trackingService;
        }

        public IActionResult Index(Guid ID)
        {
            TrackingUnitView viewModel = _TrackingService.GetTrackingUnit(ID);
            return View(viewModel);

        }

    }
}
