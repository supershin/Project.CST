using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Infras.Services;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;
using Project.ConstructionTracking.Web.Services;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;
using static Project.ConstructionTracking.Web.Models.ApproveFormcheckIUDModel;
using static Project.ConstructionTracking.Web.Models.FormGroupModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class PMApproveController : BaseController
    {
        private readonly IPMApproveService _PMApproveService;
        private readonly IHostEnvironment _hosting;
        private readonly IGetDDLService _getDDLService;
        private readonly IConfiguration _config;
        private readonly string _ConstructionQualityTracking;
        public PMApproveController(IPMApproveService PMApproveService, IHostEnvironment hosting, IGetDDLService getDDLService, IConfiguration config)
        {
            _PMApproveService = PMApproveService;
            _hosting = hosting;
            _getDDLService = getDDLService;
            _config = config;
            _ConstructionQualityTracking = config["ConstructionQualityTracking:Url"];
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
                ViewBag.PCAll = resultModel.PCAllcount;
                ViewBag.ProjectID = resultModel.ProjectID;
                ViewBag.ProjectName = resultModel.ProjectName;
                ViewBag.UnitID = resultModel.UnitID;
                ViewBag.UnitFormID = resultModel.UnitFormID;
                ViewBag.UnitCode = resultModel.UnitCode;
                ViewBag.FormID = resultModel.FormID;
                ViewBag.FormName = resultModel.FormName;             
                ViewBag.UnitFormStatusID = resultModel.UnitFormStatusID;
                ViewBag.Actiondate = FormatExtension.FormatDateToDayMonthNameYearTime(resultModel.Actiondate);
                ViewBag.ActiondatePm = FormatExtension.FormatDateToDayMonthNameYearTime(resultModel.ActiondatePm);
                ViewBag.ActiondatePJm = FormatExtension.FormatDateToDayMonthNameYearTime(resultModel.ActiondatePJm);
                ViewBag.Grade = resultModel.Grade;
                ViewBag.LockStatusID = resultModel.PM_getListgroup?.Any(l => l.LockStatusID != null) == true ? "NotNull" : null;
                ViewBag.VenderName = resultModel.VenderName;
                ViewBag.CompanyName = resultModel.CompanyName;
                ViewBag.PM_Remarkaction = resultModel.PM_Remarkaction;
                ViewBag.PE_Actiontype = resultModel.PE_Actiontype;
                ViewBag.PM_Actiontype = resultModel.PM_Actiontype;
                ViewBag.PJM_Remarkaction = resultModel.PJM_Remarkaction;
                ViewBag.PJM_Actiontype = resultModel.PJM_Actiontype;
                ViewBag.FilePathPDF = resultModel.FilePathPDF;
                var Filter = new GetDDL { Act = "UserName", ValueGuid = resultModel.ActionByPE };
                List<GetDDL> ListUser = _getDDLService.GetDDLList(Filter);
                ViewBag.PEActionBy = ListUser[0].Text;

                var FindvenderSign = new GetDDL { Act = "GetVenderSign", GuID = resultModel.UnitFormID , ID = resultModel.FormID};
                List<GetDDL> venderSign = _getDDLService.GetDDLList(FindvenderSign);
                ViewBag.PathvenderSign = (venderSign != null && venderSign.Count > 0) ? venderSign[0].Text : null;
            }
            var listpass = resultModel?.PM_getListgroup;
            if (listpass != null)
            {
                int? PCAll = listpass.Count;
                int? PCPass = listpass.Count(item => item.PC_StatusID == 8 || item.PCFlageActive == false || item.PassConditionsID == null);
                int? PCUnlock = listpass.Count(item => item.LockStatusID == 8 && item.PCFlageActive == true);
                ViewBag.PCUnlock = PCUnlock;
                ViewBag.PCALLPASS = (PCAll == PCPass) ? "yes" : "no";
            }
            ViewBag.ListQCData = resultModel?.PM_getListQCData;
            var ListQCData = resultModel?.PM_getListQCData;
            if (ListQCData != null)
            {
                int? QCDataAll = ListQCData.Count;
                int? QCStatusID = 0;
                string qcStatusMessage;
                string qcStatusColore;
                if (ListQCData.All(item => item.QCStatusID == 1))
                {
                    qcStatusMessage = "ผ่าน";
                    qcStatusColore = "success";                    
                }
                else if (ListQCData.All(item => item.QCStatusID == null))
                {
                    qcStatusMessage = "ยังไม่ได้ตรวจ";
                    qcStatusColore = "secondary";
                    QCStatusID = 1;
                }
                else
                {
                    qcStatusMessage = "กำลังตรวจ";
                    qcStatusColore = "warning";
                    QCStatusID = 1;
                }
                ViewBag.QCStatusID = QCStatusID;
                ViewBag.QCStatusMessage = qcStatusMessage;
                ViewBag.qcStatusColore = qcStatusColore;

                var concatenatedQCNames = ListQCData.Select((item) => $"{item.QCName}").ToList();
                // Join them into a single string, separated by commas or new lines if needed
                ViewBag.ConcatenatedQCNames = string.Join(", ", concatenatedQCNames);

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
                string returnUrlDoc = _PMApproveService.SaveOrUpdateUnitFormAction(model);
                if (model.ActionType == "submit") 
                {
                    ViewBag.ConstructionQualityTrackingUrl = _ConstructionQualityTracking;

                    PMRespond PMRespondData = _PMApproveService.GetPMRespondSendEmailData(FormatExtension.ConvertStringToGuid(model.UnitFormID));

                    string template = RenderRazorViewtoString(this, "Template_PM_Respond_SendMail", PMRespondData);
                    var email = new EmailModel();
                    email.Host = _config["Email:HOST"];
                    email.From = _config["Email:FROM"];
                    email.Sender = _config["Email:SENDER"];
                    email.Username = _config["Email:USER_NAME"];
                    email.Password = _config["Email:PASSWORD"];
                    email.PORT = Convert.ToInt32(_config["Email:PORT"]);
                    if (!string.IsNullOrEmpty(PMRespondData.PEEmail))
                        email.To = new List<string> { PMRespondData.PEEmail};
                    email.Subject = _config["Email:Subject:HEADER_TEXT"];
                    email.Body = template;

                    (new MailService()).SendMail(email);

                    if (model.UnitFormStatus == SystemConstant.Unit_Form_Status.PM_Sendto_PJM)
                    {
                        List<PMRequestModel> listPMRequesData = _PMApproveService.GetListPMRequesSendEmailData(FormatExtension.ConvertStringToGuid(model.UnitFormID));

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

                        foreach (var request in listPMRequesData)
                        {
                            if (!string.IsNullOrEmpty(request.PJMEmail))
                            {
                                string template2 = RenderRazorViewtoString(this, "Template_PM_Request_SendMail", request);
                                emailConfig.To = new List<string> { request.PJMEmail };
                                emailConfig.Body = template2;
                                (new MailService()).SendMail(emailConfig);
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

        [HttpGet]
        public JsonResult GetDetailCommentPmpjm(Guid UnitFormID, int FormID, int RoleID)
        {
            var model = new UnitFormResourceModel
            {
                UnitFormID = UnitFormID,
                FormID = FormID,
                RoleID = RoleID
            };

            var images = _PMApproveService.GetImage(model);

            var ddlModel = new GetDDL { Act = "GetDetailCommentPMPJM", GuID = UnitFormID, ID = RoleID };
            List<GetDDL> detailCommentPmpjm = _getDDLService.GetDDLList(ddlModel);

            // Ensure data is handled safely
            var response = new
            {
                images = images, // Return empty list if no images
                Date = detailCommentPmpjm?.FirstOrDefault()?.Text ?? string.Empty,
                Username = detailCommentPmpjm?.FirstOrDefault()?.Text2 ?? string.Empty,
                Remark = detailCommentPmpjm?.FirstOrDefault()?.Text3 ?? string.Empty
            };

            return Json(response);
        }

    }
}
