using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Models.WebAPIRest;
using Project.ConstructionTracking.Web.Services;
using static Project.ConstructionTracking.Web.Infras.Services.WebAPIRestService;
using static Project.ConstructionTracking.Web.Models.WebAPIRest.RequestPostModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitStatusByProjectController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        private readonly IGetDDLService _getDDLService;
        private readonly IWebAPIRestService _WebAPIRestService;

        public UnitStatusByProjectController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService, IWebAPIRestService WebAPIRestService)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
            _WebAPIRestService = WebAPIRestService;
        }

        public IActionResult Index()
        {

            var userID = Request.Cookies["CST.ID"];
            var ddlModel = new GetDDL { Act = "Project", UserID = Guid.Parse(userID) };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.ListProject = ListProject;

            var ddlStatus = new GetDDL { Act = "Ext", ID = 4 };
            List<GetDDL> ddlList = _getDDLService.GetDDLList(ddlStatus);
            ViewBag.DDLList = ddlList;

            var en = new UnitStatusModel
            {
                act = "GetlistUnitStatusByProjectNEW",
                project_id = (ListProject != null && ListProject.Count > 0) ? ListProject[0].ValueGuid.ToString() : string.Empty,
                //project_id = "19AD1044-63CC-43D0-9CA2-548AACE6A935",
                unit_id = "",
                unit_status = "",
                build_status = "-1"

            };
            List<UnitStatusModel> unitstatuslists = _unitstatusProvider.sp_get_unitstatus(en);

            return View(unitstatuslists);
        }

        [HttpPost]
        public IActionResult SearchUnitStatusByProject(string projectId, string unitStatus, string buildStatus)
        {
            // If any of the parameters are null or empty, handle them accordingly
            projectId = string.IsNullOrEmpty(projectId) ? "" : projectId;
            unitStatus = string.IsNullOrEmpty(unitStatus) ? "" : unitStatus;
            buildStatus = string.IsNullOrEmpty(buildStatus) ? "" : buildStatus;

            var en = new UnitStatusModel
            {
                act = "GetlistUnitStatusByProjectNEW",
                project_id = projectId,
                unit_status = unitStatus,
                build_status = buildStatus
            };

            // Call the provider to get the filtered data
            List<UnitStatusModel> unitstatuslists = _unitstatusProvider.sp_get_unitstatus(en);

            // Return the partial view with the updated model
            return PartialView("PartialTable", unitstatuslists);
        }

        [HttpPost]
        public IActionResult SubmitSyncQC5([FromForm] QC_Status_Update_QC5.Sends request)
        {
            var response = new QC_Status_Update_QC5.Responds
            {
                Status = 0,
                message = "Unknown"
            };

            try
            {
                // Validate required fields
                //if (string.IsNullOrEmpty(request.project_id) ||
                //    string.IsNullOrEmpty(request.unit_number) ||
                //    string.IsNullOrEmpty(request.contractor_appointment_date) ||
                //    string.IsNullOrEmpty(request.contractor_appointment_timeStart) ||
                //    string.IsNullOrEmpty(request.qc_response_user_id) ||
                //    string.IsNullOrEmpty(request.qc_response_date) 
                //)
                //{
                //    response.message = "Missing required fields";
                //    return Json(response);
                //}

                var requestCrmUser = new RequestPostModel.Get_User_CRM.Sends
                {
                    email = FormatExtension.NullToString(Request.Cookies["CST.Email"])
                    //email = "aukkaraded@assetwise.co.th"
                };

                var apiResponse = _WebAPIRestService.CentralizeGetUserCRM(requestCrmUser).GetAwaiter().GetResult();


                if (apiResponse.status == null || apiResponse.status == 0)
                {
                    response.message = "User not found in CRM";
                    return Json(response);
                }

                request.qc_response_user_id = apiResponse.UserID;
                var apiQcStatusUpdateQc5Response = _WebAPIRestService.QcStatusUpdateQc5(request).GetAwaiter().GetResult();

                if (apiQcStatusUpdateQc5Response.Status == null || apiQcStatusUpdateQc5Response.Status == 0)
                {
                    response.message = "Failed to update QC status: " + apiQcStatusUpdateQc5Response.message;
                    return Json(response);
                }

                response.Status = 1;
                response.message = "Saved successfully";
            }
            catch (Exception ex)
            {
                response.Status = 0;
                response.message = "Error: " + ex.Message;
            }

            return Json(response);
        }

    }
}
