﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Models.SummeryUnitModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class SummaryUnitFormController : BaseController
    {
        private readonly ISummeryUnitFormService _SummeryUnitFormService;
        private readonly MasterManagementProviderProject _unitstatusProvider;

        public SummaryUnitFormController(ISummeryUnitFormService SummaryUnitFormService , MasterManagementProviderProject unitstatusProvider)
        {
            _SummeryUnitFormService = SummaryUnitFormService;
            _unitstatusProvider = unitstatusProvider;
        }

        public IActionResult Index(Guid unitId , Guid projectId , string projectName , string UnitCode , string UnitStatusName)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.ProjectName = projectName;
            ViewBag.UnitCode = UnitCode;
            ViewBag.UnitStatusName = UnitStatusName;

            //var Model = new SummeryUnitForm { UnitID = unitId };
            //List<SummeryUnitForm> listSummeryUnitForm = _SummeryUnitFormService.GetSummeryUnitFormList(Model);

            var en = new SummeryUnitFormModel
            {
                act = "unitsummary",
                project_id = projectId.ToString(),
                unit_id = unitId.ToString(),
                unit_status = "",
                user_id = "",

            };
            List<SummeryUnitFormModel> SummeryUnitFormList = _unitstatusProvider.sp_get_summaryunitform(en);

            return View(SummeryUnitFormList);
        }

        [HttpPost]
        public IActionResult GoToFormGroup(Guid projectId, string projectName, int FormID, Guid UnitFormID, string UnitFormName, Guid unitId, string UnitCode, string UnitStatusName)
        {
            string comeFrom = "SummaryUnitForm";
            return RedirectToAction("Index", "FormGroup", new {FormID, unitId, comeFrom });
        }

        [HttpPost]
        public IActionResult GoToPC(Guid UnitFormID, int GroupID , string ComFrom)
        {
            var RoleID = Request.Cookies["CST.Role"];
            GroupID = (GroupID == 0) ? -1 : GroupID;

            if (RoleID == "1")
            {
                return RedirectToAction("Index", "UnLockPassCondition", new { UnitFormID, GroupID , ComFrom });
            }
            else
            {
                return RedirectToAction("Index", "ApproveUnLockPassCondition", new { UnitFormID, GroupID , ComFrom });
            }
            
        }
    }
}
