using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitmobileController : Controller
    {
        private readonly IUnitService _unitService;

        public UnitmobileController(IUnitService unitService)
        {
            _unitService = unitService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
