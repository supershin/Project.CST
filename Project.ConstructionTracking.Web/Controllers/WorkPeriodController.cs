using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class WorkPeriodController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
