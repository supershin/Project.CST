using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class SummaryUnitFormController : Controller
    {
        private readonly ISummeryUnitFormService _SummeryUnitFormService;

        public SummaryUnitFormController(ISummeryUnitFormService SummaryUnitFormService)
        {
            _SummeryUnitFormService = SummaryUnitFormService;
        }

        public IActionResult Index(Guid unitId , Guid projectId , string projectName , string UnitCode , string UnitStatusName)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.ProjectName = projectName;
            ViewBag.UnitCode = UnitCode;
            ViewBag.UnitStatusName = UnitStatusName;

            var Model = new SummeryUnitForm { UnitID = unitId };
            List<SummeryUnitForm> listSummeryUnitForm = _SummeryUnitFormService.GetSummeryUnitFormList(Model);
            return View(listSummeryUnitForm);
        }

        [HttpPost]
        public IActionResult GoToFormGroup(Guid projectId, string projectName, int FormID, Guid UnitFormID, string UnitFormName, Guid unitId, string UnitCode, string UnitStatusName)
        {
            return RedirectToAction("Index", "FormGroup", new { projectId, projectName, FormID, UnitFormID, UnitFormName, unitId, UnitCode, UnitStatusName });
        }
    }
}
