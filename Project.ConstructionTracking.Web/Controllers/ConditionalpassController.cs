using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ConditionalpassController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
