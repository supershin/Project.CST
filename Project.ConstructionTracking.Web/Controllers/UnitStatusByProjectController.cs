using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitStatusByProjectController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
