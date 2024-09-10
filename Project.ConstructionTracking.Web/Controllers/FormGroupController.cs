using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;
using System.Text.RegularExpressions;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class FormGroupController : BaseController
    {
        private readonly IFormGroupService _FormGroupService;
        private readonly IFormChecklistService _FormChecklistService;
        private readonly IGetDDLService _getDDLService;
        private readonly IHostEnvironment _hosting;

        public FormGroupController(IFormGroupService FormGroupService, IGetDDLService getDDLService, IHostEnvironment hosting , IFormChecklistService formChecklistService)
        {
            _FormGroupService = FormGroupService;
            _getDDLService = getDDLService;
            _hosting = hosting;
            _FormChecklistService = formChecklistService;
        }

        public IActionResult Index(int FormID, Guid unitId , string comeFrom)
        {

            ViewBag.GobackTo = "Forgroup";

            var userRole = Request.Cookies["CST.Role"];
            ViewBag.RoleID = userRole;
            ViewBag.comeFrom = comeFrom;

            var filterunitData = new FormCheckListModel.Form_getUnitFormData { UnitID = unitId, FormID = FormID};

            FormCheckListModel.Form_getUnitFormData UnitFormData = _FormChecklistService.GetUnitFormData(filterunitData);

            ViewBag.ProjectId = UnitFormData.ProjectID;
            ViewBag.ProjectName = UnitFormData.ProjectName;
            ViewBag.FormID = FormID;
            ViewBag.UnitFormName = UnitFormData.FormName;
            ViewBag.unitId = unitId;
            ViewBag.UnitCode = UnitFormData.UnitCode;
            ViewBag.UnitStatusName = UnitFormData.UnitStatusName;

            ViewBag.UnitFormID = UnitFormData.UnitFormID;

            var Model = new FormGroupModel { FormID = FormID , UnitID = unitId , UnitFormID = UnitFormData.UnitFormID };
            List<FormGroupModel> listFormGroup = _FormGroupService.GetFormGroupList(Model);

            FormGroupModel.FormGroupDetail FormGroupDetail = _FormGroupService.GetFormGroupDetail(UnitFormData.UnitFormID);
            if (FormGroupDetail != null)
            {
                ViewBag.FormGroupDetail = FormGroupDetail;
                ViewBag.Signaldate = FormatExtension.FormatDateToDayMonthNameYearTime(FormGroupDetail.FileDate);
            }

            var ddlModel = new GetDDL { Act = "Vender", ID = UnitFormData.CompanyvenderID };
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

        [HttpPost]
        public IActionResult GoToFormChecklist(int FormID,Guid unitId,int GroupID ,string GobackTo)
        {
            return RedirectToAction("Index", "FormCheckList", new { FormID,unitId,GroupID,GobackTo });
        }
    }
}
