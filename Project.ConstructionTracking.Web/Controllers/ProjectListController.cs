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
        public IActionResult GoToUnitlist(string projectId, string projectName)
        {
            return RedirectToAction("Index", "Unitlist", new { projectId, projectName });
            //var userName = Request.Cookies["UserName"];
            //if (userName == "PE")
            //{
                
            //}
            //else if (userName == "PM")
            //{
            //    return RedirectToAction("Index", "PMFormcheck", new { projectId, projectName });
            //}
            //else if (userName == "QC")
            //{
            //    return RedirectToAction("Index", "SummaryUnitQC", new { projectId, projectName });
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Unitlist", new { projectId, projectName });
            //}
        }

    }
}
