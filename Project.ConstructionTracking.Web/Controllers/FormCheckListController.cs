using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class FormCheckListController : BaseController
    {

        private readonly IFormChecklistService _FormChecklistService;

        public FormCheckListController(IFormChecklistService FormChecklistService)
        {
            _FormChecklistService = FormChecklistService;
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
        public IActionResult UpdateStatus(FormChecklistIUDModel model)
        {
            try
            {
                _FormChecklistService.InsertOrUpdate(model);
                return Ok(new { success = true, message = "บันทึกข้อมูลสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "บันทึกข้อมูลไม่สำเร็จ: " + ex.Message });
            }
        }

    }
}
