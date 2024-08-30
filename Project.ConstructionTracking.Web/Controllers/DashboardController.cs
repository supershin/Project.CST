using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
	public class DashboardController : BaseController
	{
		public IActionResult Index()
		{
            var userName = Request.Cookies["CST.UserName"];
            ViewData["UserName"] = userName;
            return View();
		}
	}
}
