using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class FormCheckListController : BaseController
    {

        private readonly IFormChecklistService _FormChecklistService;
        private readonly IHostEnvironment _hosting;

        public FormCheckListController(IFormChecklistService FormChecklistService, IHostEnvironment hosting)
        {
            _FormChecklistService = FormChecklistService;
            _hosting = hosting;
        }

        public IActionResult Index(Guid projectId, string projectName, int FormID, Guid UnitFormID, string UnitFormName, Guid unitId, string UnitCode, string UnitStatusName , string GroupName , int GroupID)
        {
            var userName = Request.Cookies["CST.UserName"];
            var userRole = Request.Cookies["CST.Role"];

            var filterunitData = new FormCheckListModel.Form_getUnitFormData { UnitID = unitId , FormID = FormID, GroupID = GroupID };

            FormCheckListModel.Form_getUnitFormData UnitFormData = _FormChecklistService.GetUnitFormData(filterunitData);


            ViewBag.ProjectId = UnitFormData.ProjectID;
            ViewBag.ProjectName = UnitFormData.ProjectName;
            ViewBag.FormID = UnitFormData.FormID;
            ViewBag.UnitFormID = UnitFormData?.UnitFormID;
            ViewBag.UnitFormName = UnitFormData?.FormName;
            ViewBag.UnitId = UnitFormData?.UnitID;
            ViewBag.UnitFormStatusID = UnitFormData?.UnitFormStatusID;
            ViewBag.UnitCode = UnitFormData?.UnitCode;
            ViewBag.UnitStatusName = UnitStatusName;
            ViewBag.GroupName = UnitFormData?.GroupName;
            ViewBag.GroupID = UnitFormData?.GroupID;
            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;

            var filterData = new FormCheckListModel.Form_getFilterData { GroupID = GroupID , UnitFormID = UnitFormData?.UnitFormID };
            List<FormCheckListModel.Form_getListPackages> listChecklist = _FormChecklistService.GetFormCheckList(filterData);

            var statusFilterData = new FormCheckListModel.Form_getFilterData
            {
                ProjectID = UnitFormData.ProjectID,
                UnitID = UnitFormData.UnitID,
                GroupID = UnitFormData.GroupID,
                FormID = UnitFormData.FormID
            };

            List<FormCheckListModel.Form_getListStatus> listStatus = _FormChecklistService.GetFormCheckListStatus(statusFilterData);

            if (listStatus != null && listStatus.Count > 0)
            {
                var status = listStatus[0]; // Assuming there is only one row in listStatus
                ViewBag.LockStatusID = status.LockStatusID;
                ViewBag.PCStatusID = status.PCStatusID;
                ViewBag.RemarkPassCondition = status.RemarkPassCondition;
                ViewBag.UnitFormActionID = status.UnitFormActionID;
                ViewBag.PM_StatusID = status.PM_StatusID;
                ViewBag.PM_ActionType = status.PM_ActionType;
                ViewBag.PJM_StatusID = status.PJM_StatusID;
                ViewBag.PJM_ActionType = status.PJM_ActionType;
                ViewBag.StatusActionType = status.ActionType;
                ViewBag.StatusUpdateDate = status.UpdateDate;
            }

            var viewModel = new FormChecklistViewModel
            {
                ListPackages = listChecklist,
                ListStatus = listStatus
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(FormChecklistIUDModel model,IFormFileCollection files)
        {
            try
            {
                //await _FormChecklistService.InsertOrUpdate(model, files);
                return Ok(new { success = true, message = "บันทึกข้อมูลสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "บันทึกข้อมูลไม่สำเร็จ: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateStatusV1(FormChecklistIUDModel model)
        {
            try
            {
                // Convert userID to Guid
                Guid userGuid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                // Convert roleID to int
                int roleInt = int.TryParse(Request.Cookies["CST.Role"], out var tempRoleInt) ? tempRoleInt : -1;

                model.ApplicationPath = _hosting.ContentRootPath;

                _FormChecklistService.InsertOrUpdate(model, userGuid, roleInt);
                return Ok(new { success = true, message = "บันทึกข้อมูลสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "บันทึกข้อมูลไม่สำเร็จ: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteImage(Guid resourceId)
        {
            try
            {
                var ApplicationPath = _hosting.ContentRootPath;
                var result = _FormChecklistService.DeleteImage(resourceId , ApplicationPath);
                if (result)
                {
                    return Ok(new { success = true, message = "Image deleted successfully." });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to delete image." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


    }
}
