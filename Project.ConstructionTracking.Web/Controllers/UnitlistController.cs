using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitlistController : Controller
    {
        private readonly IUnitService _unitService;

        public UnitlistController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        public IActionResult Index(string projectId, string projectName)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.ProjectName = projectName;

            var model = new UnitModel
            {
                ProjectID = Guid.TryParse(projectId, out var parsedProjectId) ? parsedProjectId : (Guid?)null,
            };

            List<UnitModel> units = _unitService.GetUnitList("", model);
            return View(units);
        }

        [HttpPost]
        public IActionResult SearchUnits(string search, string projectId, string projectName)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.ProjectName = projectName;

            var model = new UnitModel
            {
                ProjectID = Guid.TryParse(projectId, out var parsedProjectId) ? parsedProjectId : (Guid?)null,
            };

            List<UnitModel> units = _unitService.GetUnitList(search, model);
            return View("Index", units);
        }

        [HttpPost]
        public IActionResult GoToByRole(string projectId, string projectName)
        {
            var userName = Request.Cookies["UserName"];
            if (userName == "PE")
            {
                return RedirectToAction("Index", "Tracking", new { projectId, projectName });
            }
            else if (userName == "PM")
            {
                return RedirectToAction("Index", "PMApprove", new { projectId, projectName });
            }
            else if (userName == "QC")
            {
                return RedirectToAction("Index", "SummaryUnitQC", new { projectId, projectName });
            }
            else
            {
                return RedirectToAction("Index", "Unitlist", new { projectId, projectName });
            }
        }

    }
}

