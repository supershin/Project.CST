using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Infras.Services;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;
using Project.ConstructionTracking.Web.Services;
using System.Text.RegularExpressions;
using static Project.ConstructionTracking.Web.Models.ApproveFormcheckIUDModel;
using static Project.ConstructionTracking.Web.Models.PJMApproveModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class PJMApproveController : BaseController
    {
        private readonly IPJMApproveService _PJMApproveService;
        private readonly IPMApproveService _PMApproveService;
        private readonly IHostEnvironment _hosting;
        private readonly IGetDDLService _getDDLService;
        private readonly IConfiguration _config;
        private readonly string _VendorPortal;
        private readonly string _ConstructionQualityTracking;
        public PJMApproveController(IPJMApproveService PJMApproveService, IHostEnvironment hosting, IPMApproveService pMApproveService, IGetDDLService getDDLService, IConfiguration config)
        {
            _PJMApproveService = PJMApproveService;
            _hosting = hosting;
            _PMApproveService = pMApproveService;
            _getDDLService = getDDLService;
            _config = config;
            _VendorPortal = config["VendorPortal:Url"];
            _ConstructionQualityTracking = config["ConstructionQualityTracking:Url"];
        }
        public IActionResult Index(Guid UnitFormID)
        {
            var filterData = new PJMApproveModel.GetlistChecklistPC { UnitFormID = UnitFormID };
            List<PJMApproveModel.GetlistChecklistPC> ListChecklistPJMApprove = _PJMApproveService.GetChecklistPJMApprove(filterData);

            int? _FormID = 0;

            if (ListChecklistPJMApprove != null && ListChecklistPJMApprove.Count > 0)
            {
                var listPJMApprove = ListChecklistPJMApprove[0]; // Assuming there is only one row in listStatus
                ViewBag.ProjectID = listPJMApprove.ProjectID;
                ViewBag.ProjectName = listPJMApprove.ProjectName;
                ViewBag.UnitID = listPJMApprove.UnitID;
                ViewBag.UnitCode = listPJMApprove.UnitCode;
                ViewBag.UnitFormID = listPJMApprove.UnitFormID;
                ViewBag.UnitFormStatus = listPJMApprove?.UnitFormStatus?.ToString() ?? "";
                ViewBag.FormID = listPJMApprove?.FormID;
                ViewBag.PJM_ActionBy = listPJMApprove?.PJM_ActionBy;
                ViewBag.FormName = listPJMApprove?.FormName;
                ViewBag.PJM_Actiontype = listPJMApprove?.PJM_Actiontype ?? string.Empty;
                ViewBag.PJM_ActionDate = FormatExtension.FormatDateToDayMonthNameYearTime(listPJMApprove?.PJM_ActionDate);
                ViewBag.PJM_StatusID = listPJMApprove?.PJM_StatusID ?? (int?)null;
                ViewBag.PJMUnitFormRemark = listPJMApprove?.PJMUnitFormRemark ?? string.Empty;
                _FormID = listPJMApprove?.FormID;
            }
            ViewBag.ListChecklistPJMApprove = ListChecklistPJMApprove;


            var model = new UnitFormResourceModel
            {
                UnitFormID = UnitFormID,
                FormID = _FormID,
                RoleID = 3
            };

            // Call the service to get the images
            var ListPJMImage = _PMApproveService.GetImage(model);

            ViewBag.ListPJMImage = ListPJMImage;

            var FilepatchPDF = new GetDDL { Act = "GetUnitFornPDF", GuID = UnitFormID };
            List<GetDDL> patchPDF = _getDDLService.GetDDLList(FilepatchPDF);
            ViewBag.FilePathPDF = (patchPDF != null && patchPDF.Count > 0 && !string.IsNullOrEmpty(patchPDF[0].Text)) ? patchPDF[0].Text : string.Empty;

            return View(ListChecklistPJMApprove);
        }


        [HttpPost]
        public IActionResult SaveOrSubmit(PJMApproveModel.PJMApproveIU model)
        {
            try
            {
                var param = Request.Form["PassConditionsIUD"];
                if (!string.IsNullOrEmpty(param))
                {
                    model.ListPCIC = JsonConvert.DeserializeObject<List<PJMIUPC>>(param);
                }
                model.UserID = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                model.ApplicationPath = _hosting.ContentRootPath;

                // Get the document URL if available
                string returnUrlDoc = _PJMApproveService.SaveOrUpdateUnitFormAction(model);

                if (model.ActionType == "submit")
                {
                    ViewBag.ConstructionQualityTrackingUrl = _ConstructionQualityTracking;

                    List<PJMRespondModel> listPJMRespondData = _PJMApproveService.GetPJMRespondSendEmailData(FormatExtension.ConvertStringToGuid(model.UnitFormID));

                    var emailConfig = new EmailModel
                    {
                        Host = _config["Email:HOST"],
                        From = _config["Email:FROM"],
                        Sender = _config["Email:SENDER"],
                        Username = _config["Email:USER_NAME"],
                        Password = _config["Email:PASSWORD"],
                        PORT = Convert.ToInt32(_config["Email:PORT"]),
                        Subject = _config["Email:Subject:HEADER_TEXT"]
                    };

                    foreach (var request in listPJMRespondData)
                    {
                        if (!string.IsNullOrEmpty(request.Email))
                        {
                            // Render template for the current PM
                            string template = RenderRazorViewtoString(this, "Template_PJM_Respond_SendMail", request);

                            // Send the email
                            emailConfig.To = new List<string> { request.Email };
                            emailConfig.Body = template;
                            (new MailService()).SendMail(emailConfig);
                        }
                    }
                    int? statusToUpdate = model.ListPCIC != null && model.ListPCIC.Any(pc => pc.StatusID == 9 && pc.PC_FlagActive != 0) ? 9 : 8;
                    int? statusForm = (statusToUpdate == 8) ? 7 : 8;
                    if (statusForm == 7)
                    {
                        AdminRespond listPJMRequesData = _PJMApproveService.GetAdminPJMRespond(model.UnitFormID.AsGuid());

                        var adminEmailConfig = new EmailModel 
                        {
                            Host = _config["Email:HOST"],
                            From = _config["Email:FROM"],
                            Sender = _config["Email:SENDER"],
                            Username = _config["Email:USER_NAME"],
                            Password = _config["Email:PASSWORD"],
                            PORT = Convert.ToInt32(_config["Email:PORT"]),
                            Subject = _config["Email:Subject:HEADER_TEXT"]
                        };

                        if (listPJMRequesData.ListSendEmailAdmin != null)
                        {
                            foreach (var request in listPJMRequesData.ListSendEmailAdmin)
                            {
                                if (!string.IsNullOrEmpty(request.AdminEmail))
                                {
                                    string templateAdmin = RenderRazorViewtoString(this, "Template_Noti_Admin_Respond_With_Pass_Condition_SendMail", listPJMRequesData);
                                    adminEmailConfig.To = new List<string> { request.AdminEmail };
                                    adminEmailConfig.Body = templateAdmin;
                                    (new MailService()).SendMail(adminEmailConfig);
                                }
                            }
                        }
                    }
                }

                return Ok(new { success = true, pdfPath = returnUrlDoc });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public JsonResult GetImagesUnlock(Guid UnitFormID, int PassConditionID)
        {
            // Prepare the model to send to the service
            var model = new PJMApproveModel.GetImageUnlock
            {
                UnitFormID = UnitFormID,
                PassConditionID = PassConditionID
            };

            // Call the service to get the images
            var listimages = _PJMApproveService.GetImageUnlock(model);

            // Return the images as JSON to be used in the modal
            return Json(listimages);
        }
    }
}
