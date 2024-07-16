using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ProjectListController : Controller
    {
        private readonly IProjectService _ProjectService;

        public ProjectListController(IProjectService projectService)
        {
            _ProjectService = projectService;
        }

        public IActionResult Index()
        {
            var projects = _ProjectService.GetProjectList();
            return View(projects);
        }

        [HttpPost]
        public IActionResult Search(string Search_Project)
        {
            var projects = string.IsNullOrWhiteSpace(Search_Project) ?
                _ProjectService.GetProjectList() :
                _ProjectService.SearchProjects(Search_Project);
            return View("Index", projects);
        }

        [HttpGet]
        public JsonResult SearchProjects(string term)
        {
            var projects = _ProjectService.SearchProjects(term);
            return Json(projects);
        }

        [HttpPost]
        public IActionResult GoToUnitlist(int projectId)
        {
            var userName = Request.Cookies["CST.UserName"];
            if (userName == "PE")
            {
                return RedirectToAction("Index", "Unitmobile");
            }
            else if (userName == "PM")
            {
                return RedirectToAction("Index", "PMFormcheck");
            }
            else
            {
                return RedirectToAction("Index", "Unitmobile");
            }
        }
    }
}
