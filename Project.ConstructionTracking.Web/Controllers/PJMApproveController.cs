using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
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
        public PJMApproveController(IPJMApproveService PJMApproveService, IHostEnvironment hosting, IPMApproveService pMApproveService)
        {
            _PJMApproveService = PJMApproveService;
            _hosting = hosting;
            _PMApproveService = pMApproveService;
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
                _PJMApproveService.SaveOrUpdateUnitFormAction(model);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
