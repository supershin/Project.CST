using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class QCChecklistController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
