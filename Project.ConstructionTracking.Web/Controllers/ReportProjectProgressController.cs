using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReportProjectProgressController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
