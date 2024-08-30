using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ChatInBoxController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
