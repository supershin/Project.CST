using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitlistController : BaseController
    {
        private readonly IUnitService _unitService;
        private readonly IGetDDLService _getDDLService;

        public UnitlistController(IUnitService unitService, IGetDDLService getDDLService)
        {
            _unitService = unitService;
            _getDDLService = getDDLService;
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

            // Fetch dropdown data
            var ddlModel = new GetDDL { Act = "Ext", ID = 4 };
            List<GetDDL> ddlList = _getDDLService.GetDDLList(ddlModel);

            ViewBag.DDLList = ddlList;

            return View(units);
        }

        [HttpPost]
        public IActionResult SearchUnits(string search, string projectId, string projectName, int? unitStatusId)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.ProjectName = projectName;

            var model = new UnitModel
            {
                ProjectID = Guid.TryParse(projectId, out var parsedProjectId) ? parsedProjectId : (Guid?)null,
                UnitStatusID = unitStatusId
            };

            List<UnitModel> units = _unitService.GetUnitList(search, model);

            // Fetch dropdown data
            var ddlModel = new GetDDL { Act = "Ext", ID = 4 };
            List<GetDDL> ddlList = _getDDLService.GetDDLList(ddlModel);

            ViewBag.DDLList = ddlList;

            return View("Index", units);
        }

        [HttpPost]
        public IActionResult GoToByRole(Guid projectId, string projectName, Guid unitId , string UnitCode , string UnitStatusName)
        {
            var userName = Request.Cookies["CST.UserName"];
            if (userName == "PE")
            {
                return RedirectToAction("Index", "SummaryUnitForm", new { unitId, projectId, projectName , UnitCode , UnitStatusName });
            }
            else if (userName == "PM")
            {
                return RedirectToAction("Index", "SummaryUnitForm", new { unitId, projectId, projectName, UnitCode, UnitStatusName });
            }
            else if (userName == "PJM")
            {
                return RedirectToAction("Index", "SummaryUnitForm", new { unitId, projectId, projectName, UnitCode, UnitStatusName });
            }
            else if (userName == "QC")
            {
                return RedirectToAction("Index", "SummaryUnitQC", new { projectId, projectName, unitId });
            }
            else
            {
                return RedirectToAction("Index", "Unitlist", new { projectId, projectName, unitId });
            }
        }

    }
}
