using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ProjectListController : BaseController
    {
        private readonly IProjectService _ProjectService;

        public ProjectListController(IProjectService projectService)
        {
            _ProjectService = projectService;
        }
           
        public IActionResult Index()
        {
            var userID = Request.Cookies["CST.ID"];
            var userIDuse = Guid.Parse(userID);
            var projects = _ProjectService.GetProjectList(userIDuse);
            return View(projects);
        }

        [HttpPost]
        public IActionResult Search(string Search_Project)
        {
            var userID = Request.Cookies["CST.ID"];
            var userIDuse = Guid.Parse(userID);
            var projects = string.IsNullOrWhiteSpace(Search_Project) ? _ProjectService.GetProjectList(userIDuse) 
                                                                     : _ProjectService.SearchProjects(Search_Project, userIDuse);
            return View("Index", projects);
        }

        [HttpGet]
        public JsonResult SearchProjects(string term)
        {
            var userID = Request.Cookies["CST.ID"];
            var userIDuse = Guid.Parse(userID);
            var projects = _ProjectService.SearchProjects(term , userIDuse);
            return Json(projects);
        }

        [HttpPost]
        public IActionResult GoToUnitlist(string projectId, string projectName)
        {
            return RedirectToAction("Index", "Unitlist", new { projectId, projectName });
        }

    }
}
