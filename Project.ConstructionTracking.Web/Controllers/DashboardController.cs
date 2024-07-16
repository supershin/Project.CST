using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
            var userName = Request.Cookies["UserName"];
            ViewData["UserName"] = userName;
            return View();
		}
	}
}
