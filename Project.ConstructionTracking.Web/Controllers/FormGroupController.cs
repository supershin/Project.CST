﻿using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class FormGroupController : Controller
    {
        private readonly IFormGroupService _FormGroupService;
        private readonly IGetDDLService _getDDLService;
        private readonly IHostEnvironment _hosting;

        public FormGroupController(IFormGroupService FormGroupService, IGetDDLService getDDLService, IHostEnvironment hosting)
        {
            _FormGroupService = FormGroupService;
            _getDDLService = getDDLService;
            _hosting = hosting;
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

            FormGroupModel.FormGroupDetail FormGroupDetail = _FormGroupService.GetFormGroupDetail(UnitFormID);
            ViewBag.FormGroupDetail = FormGroupDetail;

            var ddlModel = new GetDDL { Act = "Vender", ID = 0 };
            List<GetDDL> ListVender = _getDDLService.GetDDLList(ddlModel);

            ViewBag.ListVender = ListVender;

            return View(listFormGroup);
        }

        [HttpPost]
        public IActionResult UpdateSaveGrade(FormGroupModel.FormGroupIUDModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.userID = Guid.Parse(userID);
                model.RoleID = int.Parse(RoleID);
                model.ApplicationPath = _hosting.ContentRootPath;
                _FormGroupService.SubmitSaveFormGroup(model);
                return Ok(new { success = true, message = model.FormGrade });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "บันทึกข้อมูลไม่สำเร็จ: " + ex.Message });
            }
        }
    }
}
