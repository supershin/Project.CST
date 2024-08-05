using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class FormGroupController : Controller
    {
        private readonly IFormGroupService _FormGroupService;

        public FormGroupController(IFormGroupService FormGroupService)
        {
            _FormGroupService = FormGroupService;
        }

        public IActionResult Index(Guid projectId, string projectName, int FormID, Guid UnitFormID, string UnitFormName, Guid unitId , string UnitCode , string UnitStatusName)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.ProjectName = projectName;
            ViewBag.FormID = FormID;
            ViewBag.UnitFormID = UnitFormID;
            ViewBag.UnitFormName = UnitFormName;
            ViewBag.unitId = unitId;
            ViewBag.UnitCode = UnitCode;
            ViewBag.UnitStatusName = UnitStatusName;

            var Model = new FormGroupModel { FormID = FormID , UnitID = unitId , UnitFormID = UnitFormID };
            List<FormGroupModel> listFormGroup = _FormGroupService.GetFormGroupList(Model);
            return View(listFormGroup);
        }
    }
}
