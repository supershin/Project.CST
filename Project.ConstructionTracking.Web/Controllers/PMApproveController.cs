﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services; // Assuming the GetApproveFormcheckList class is in the Services namespace
using static Project.ConstructionTracking.Web.Models.ApproveFormcheckIUDModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class PMApproveController : BaseController
    {
        private readonly IPMApproveService _PMApproveService;
        private readonly IHostEnvironment _hosting;
        public PMApproveController(IPMApproveService PMApproveService, IHostEnvironment hosting)
        {
            _PMApproveService = PMApproveService;
            _hosting = hosting;
        }

        public IActionResult Index(Guid unitId, int formId ,string comeFrom)
        {
            // Use the GetApproveFormcheck method from your service to get the data
            var model = new ApproveFormcheckModel { UnitID = unitId, FormID = formId };
            var resultModel = _PMApproveService.GetApproveFormcheck(model);
            ViewBag.comeFrom = comeFrom;

            if (resultModel != null)
            {
                // Set ViewBag properties based on the result
                ViewBag.ProjectID = resultModel.ProjectID;
                ViewBag.ProjectName = resultModel.ProjectName;
                ViewBag.UnitID = resultModel.UnitID;
                ViewBag.UnitFormID = resultModel.UnitFormID;
                ViewBag.UnitCode = resultModel.UnitCode;
                ViewBag.FormID = resultModel.FormID;
                ViewBag.FormName = resultModel.FormName;
                ViewBag.UnitFormStatusID = resultModel.UnitFormStatusID;
                ViewBag.Actiondate = resultModel.Actiondate?.ToString("dd/MM/yyyy") ?? "";
                ViewBag.ActiondatePm = resultModel.ActiondatePm?.ToString("dd/MM/yyyy") ?? "";
                ViewBag.ActiondatePJm = resultModel.ActiondatePJm?.ToString("dd/MM/yyyy") ?? "";
                ViewBag.Grade = resultModel.Grade;
                ViewBag.LockStatusID = resultModel.PM_getListgroup?.Any(l => l.LockStatusID != null) == true ? "NotNull" : null;
                ViewBag.VenderName = resultModel.VenderName;
                ViewBag.PM_Remarkaction = resultModel.PM_Remarkaction;
                ViewBag.PM_Actiontype = resultModel.PM_Actiontype;
                ViewBag.PJM_Remarkaction = resultModel.PJM_Remarkaction;
                ViewBag.PJM_Actiontype = resultModel.PJM_Actiontype;
            }
            var listpass = resultModel?.PM_getListgroup;
            if (listpass != null)
            {
                int? PCAll = listpass.Count;
                int? PCPass = listpass.Count(item => item.PC_StatusID == 8 || item.PCFlageActive == false || item.PassConditionsID == null);
                ViewBag.PCALLPASS = (PCAll == PCPass) ? "yes" : "no";
            }

            // Pass the resultModel to the view
            return View(resultModel);
        }

        [HttpPost]
        public IActionResult SaveOrSubmit(ApproveFormcheckIUDModel model)
        {
            try
            {
                var param = Request.Form["PassConditionsIUD"];
                if (!string.IsNullOrEmpty(param))
                {
                    model.PassConditionsIUD = JsonConvert.DeserializeObject<List<PassConditions>>(param);
                }
                model.ApplicationPath = _hosting.ContentRootPath;
                model.UserID = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                model.RoleID = int.TryParse(Request.Cookies["CST.Role"], out var tempRoleInt) ? tempRoleInt : -1;
                _PMApproveService.SaveOrUpdateUnitFormAction(model);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetImages(Guid UnitFormID, int GroupID, int FormID, int RoleID)
        {
            // Prepare the model to send to the service
            var model = new UnitFormResourceModel
            {
                UnitFormID = UnitFormID,
                GroupID = GroupID,
                FormID = FormID,    
                RoleID= RoleID
            };

            // Call the service to get the images
            var images = _PMApproveService.GetImage(model);

            // Return the images as JSON to be used in the modal
            return Json(images);
        }

    }
}
