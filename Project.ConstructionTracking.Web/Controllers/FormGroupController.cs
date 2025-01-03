using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Infras.Services;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;
using Project.ConstructionTracking.Web.Services;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class FormGroupController : BaseController
    {
        private readonly IFormGroupService _FormGroupService;
        private readonly IFormChecklistService _FormChecklistService;
        private readonly IGetDDLService _getDDLService;
        private readonly IHostEnvironment _hosting;
        private readonly IPMApproveService _PMApproveService;
        private readonly IConfiguration _config;
        public FormGroupController(IFormGroupService FormGroupService, IGetDDLService getDDLService, IHostEnvironment hosting , IFormChecklistService formChecklistService , IPMApproveService PMApproveService , IConfiguration configuration)
        {
            _FormGroupService = FormGroupService;
            _getDDLService = getDDLService;
            _hosting = hosting;
            _FormChecklistService = formChecklistService;
            _PMApproveService = PMApproveService;
            _config = configuration;
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
            ViewBag.UnitFormMainStatusID = UnitFormData.UnitFormStatusID;
            ViewBag.UnitFormID = UnitFormData.UnitFormID;
            ViewBag.CompanyvenderName = UnitFormData.CompanyvenderName;

            var Model = new FormGroupModel { FormID = FormID , UnitID = unitId , UnitFormID = UnitFormData.UnitFormID };
            List<FormGroupModel> listFormGroup = _FormGroupService.GetFormGroupList(Model);

            FormGroupModel.FormGroupDetail FormGroupDetail = _FormGroupService.GetFormGroupDetail(UnitFormData.UnitFormID);
            if (FormGroupDetail != null)
            {
                ViewBag.FormGroupDetail = FormGroupDetail;
                ViewBag.Signaldate = FormatExtension.FormatDateToDayMonthNameYearTime(FormGroupDetail?.FileDate);
                ViewBag.PEActionDate = FormatExtension.FormatDateToDayMonthNameYearTime(FormGroupDetail?.PE_ActionDate);
                ViewBag.FilePathPDF = FormGroupDetail?.FilePathPDF;
                var Filter = new GetDDL { Act = "UserName", ValueGuid = FormGroupDetail.PE_ActionBy };
                List<GetDDL> ListUser = _getDDLService.GetDDLList(Filter);
                ViewBag.PEActionBy = ListUser[0].Text;

            }

            var ddlModel = new GetDDL { Act = "Vender", ID = UnitFormData.CompanyvenderID };
            List<GetDDL> ListVender = _getDDLService.GetDDLList(ddlModel);
            ViewBag.ListVender = ListVender;

            var userID = Request.Cookies["CST.ID"];
            var userIDuse = Guid.Parse(userID);

            bool ResultPermissionSubmit = _FormGroupService.ValidateUserSubmit(userIDuse , UnitFormData.UnitID);
            ViewBag.PermissionSubmit = ResultPermissionSubmit;

            var FilterPC = new GetDDL { Act = "GetUnitFormPassCondition", GuID = UnitFormData.UnitFormID };
            List<GetDDL> ListPC = _getDDLService.GetDDLList(FilterPC);
            ViewBag.CntPC = (ListPC?.Count > 0) ? ListPC.Count : 0;

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

                if (model.Act == "submit")
                {
                    // Retrieve the data list
                    List<PERequesModel> listPERequesData = _FormGroupService.GetListPERequesSendEmailData(FormatExtension.ConvertStringToGuid(model.UnitFormID));

                    // Configure email settings
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

                    foreach (var request in listPERequesData)
                    {
                        if (!string.IsNullOrEmpty(request.PMEmail))
                        {
                            // Render template for the current PM
                            string template = RenderRazorViewtoString(this, "Template_PE_Request_SendMail", request);

                            // Send the email
                            emailConfig.To = new List<string> { request.PMEmail };
                            emailConfig.Body = template;
                            (new MailService()).SendMail(emailConfig);
                        }
                    }

                    return Ok(new { success = true, message = model.FormGrade });
                }

                return Ok(new { success = true, message = model.FormGrade });
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                return StatusCode(500, new { success = false, message = ex.Message });
            }

        }

        [HttpPost]
        public IActionResult GoToFormChecklist(int FormID,Guid unitId,int GroupID ,string GobackTo)
        {
            return RedirectToAction("Index", "FormCheckList", new { FormID,unitId,GroupID,GobackTo });
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
