using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitcheckController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Check()
        {
            return View();
        }
    }
}
