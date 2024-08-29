using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class SummaryUnitQCController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
