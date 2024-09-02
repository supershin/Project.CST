using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ApproveUnLockPassConditionController : BaseController
    {
        private readonly IUnLockPassConditionService _UnLockPassConditionService;
        private readonly IHostEnvironment _hosting;

        public ApproveUnLockPassConditionController(IUnLockPassConditionService UnLockPassConditionService, IHostEnvironment hosting)
        {
            _UnLockPassConditionService = UnLockPassConditionService;
            _hosting = hosting;
        }

        public IActionResult Index(Guid UnitFormID, int GroupID)
        {

            var model = new UnLockPassConditionModel.GetDataUnlockPC { UnitFormID = UnitFormID, GroupID = GroupID };
            var resultModel = _UnLockPassConditionService.GetListUnlockPC(model);
            if (resultModel != null && resultModel.Count > 0)
            {
                ViewBag.ProjectID = resultModel[0].ProjectID.ToString() != null ? resultModel[0].ProjectID.ToString() : string.Empty;
                ViewBag.ProjectName = resultModel[0].ProjectName != null ? resultModel[0].ProjectName : string.Empty;
                ViewBag.UnitID = resultModel[0].UnitID.ToString() != null ? resultModel[0].UnitID.ToString() : string.Empty;
                ViewBag.UnitCode = resultModel[0].UnitCode != null ? resultModel[0].UnitCode : string.Empty;
                ViewBag.FormName = resultModel[0].FormName != null ? resultModel[0].FormName : string.Empty;
                ViewBag.ListGroupPC = resultModel;
            }
            return View(resultModel);
        }

        [HttpPost]
        public IActionResult RequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.UserID = Guid.Parse(userID);
                model.RoleID = int.Parse(RoleID);
                _UnLockPassConditionService.RequestUnlock(model);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "บันทึกข้อมูลไม่สำเร็จ: " + ex.Message });
            }
        }
    }
}
