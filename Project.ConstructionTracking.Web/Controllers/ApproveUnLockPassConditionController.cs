using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Infras.Services;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ApproveUnLockPassConditionController : BaseController
    {
        private readonly IUnLockPassConditionService _UnLockPassConditionService;
        private readonly IHostEnvironment _hosting;
        private readonly IConfiguration _config;
        private readonly string _ConstructionQualityTracking;

        public ApproveUnLockPassConditionController(IUnLockPassConditionService UnLockPassConditionService, IHostEnvironment hosting, IConfiguration config)
        {
            _UnLockPassConditionService = UnLockPassConditionService;
            _hosting = hosting;
            _config = config;
            _ConstructionQualityTracking = config["ConstructionQualityTracking:Url"];
        }

        public IActionResult Index(Guid UnitFormID, int GroupID, string ComFrom)
        {
            ViewBag.ComFrom = ComFrom;
            var model = new UnLockPassConditionModel.GetDataUnlockPC { UnitFormID = UnitFormID, GroupID = GroupID };
            var resultModel = _UnLockPassConditionService.GetListUnlockPC(model);
            if (resultModel != null && resultModel.Count > 0)
            {
                ViewBag.ProjectID = resultModel[0].ProjectID.ToString() != null ? resultModel[0].ProjectID.ToString() : string.Empty;
                ViewBag.ProjectName = resultModel[0].ProjectName != null ? resultModel[0].ProjectName : string.Empty;
                ViewBag.UnitFormID = resultModel[0].UnitFormID != null ? resultModel[0].UnitFormID : Guid.Empty;
                ViewBag.UnitID = resultModel[0].UnitID.ToString() != null ? resultModel[0].UnitID.ToString() : string.Empty;
                ViewBag.UnitCode = resultModel[0].UnitCode != null ? resultModel[0].UnitCode : string.Empty;
                ViewBag.FormID = resultModel[0].FormID != null ? resultModel[0].FormID : 0;
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

                ViewBag.ConstructionQualityTrackingUrl = _ConstructionQualityTracking;

                PMRespondUnlockModel PMRespondUnlockData = _UnLockPassConditionService.PMRespondUnlockSendMail(model.PC_ID, FormatExtension.ConvertStringToGuid(model.UnitFormID));

                string template = RenderRazorViewtoString(this, "Template_PM_Respond_Unlock_SendMail", PMRespondUnlockData);
                var email = new EmailModel();
                email.Host = _config["Email:HOST"];
                email.From = _config["Email:FROM"];
                email.Sender = _config["Email:SENDER"];
                email.Username = _config["Email:USER_NAME"];
                email.Password = _config["Email:PASSWORD"];
                email.PORT = Convert.ToInt32(_config["Email:PORT"]);
                if (!string.IsNullOrEmpty(PMRespondUnlockData.Email))
                    email.To = new List<string> { PMRespondUnlockData.Email };
                email.Subject = _config["Email:Subject:HEADER_TEXT"];
                email.Body = template;

                (new MailService()).SendMail(email);


                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "บันทึกข้อมูลไม่สำเร็จ: " + ex.Message });
            }
        }
    }
}
