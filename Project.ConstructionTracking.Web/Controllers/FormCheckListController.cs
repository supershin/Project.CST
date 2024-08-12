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

            ViewBag.ProjectId = projectId;
            ViewBag.ProjectName = projectName;
            ViewBag.FormID = FormID;
            ViewBag.UnitFormID = UnitFormID;
            ViewBag.UnitFormName = UnitFormName;
            ViewBag.UnitId = unitId;
            ViewBag.UnitCode = UnitCode;
            ViewBag.UnitStatusName = UnitStatusName;
            ViewBag.GroupName = GroupName;
            ViewBag.GroupID = GroupID;
            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;

            var filterData = new FormCheckListModel.Form_getFilterData { GroupID = GroupID , UnitFormID = UnitFormID };
            List<FormCheckListModel.Form_getListPackages> listChecklist = _FormChecklistService.GetFormCheckList(filterData);

            var statusFilterData = new FormCheckListModel.Form_getFilterData
            {
                ProjectID = projectId,
                UnitID = unitId,
                GroupID = GroupID,
                FormID = FormID
            };

            List<FormCheckListModel.Form_getListStatus> listStatus = _FormChecklistService.GetFormCheckListStatus(statusFilterData);

            if (listStatus != null && listStatus.Count > 0)
            {
                var status = listStatus[0]; // Assuming there is only one row in listStatus
                //ViewBag.StatusID = status.ID;
                //ViewBag.StatusProjectID = status.ProjectID;
                //ViewBag.StatusUnitID = status.UnitID;
                //ViewBag.StatusFormID = status.FormID;
                //ViewBag.StatusRoleID = status.RoleID;
                //ViewBag.UnitFormID = status.ID;

                ViewBag.LockStatusID = status.LockStatusID;
                ViewBag.RemarkPassCondition = status.RemarkPassCondition;
                ViewBag.UnitFormActionID = status.UnitFormActionID;
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
