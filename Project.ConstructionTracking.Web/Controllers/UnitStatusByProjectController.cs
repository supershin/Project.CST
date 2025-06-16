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
        private readonly IQC5CheckService _QC5CheckService;

        public UnitStatusByProjectController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService, IWebAPIRestService WebAPIRestService, IQC5CheckService qC5CheckService)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
            _WebAPIRestService = WebAPIRestService;
            _QC5CheckService = qC5CheckService;
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
                build_status = "-1",
                sync_qc_status = "-1"

            };
            List<UnitStatusModel> unitstatuslists = _unitstatusProvider.sp_get_unitstatus(en);

            return View(unitstatuslists);
        }

        [HttpPost]
        public IActionResult SearchUnitStatusByProject(string projectId, string unitStatus, string buildStatus, string syncQCStatus)
        {
            // If any of the parameters are null or empty, handle them accordingly
            projectId = string.IsNullOrEmpty(projectId) ? "" : projectId;
            unitStatus = string.IsNullOrEmpty(unitStatus) ? "" : unitStatus;
            buildStatus = string.IsNullOrEmpty(buildStatus) ? "" : buildStatus;
            syncQCStatus = string.IsNullOrEmpty(syncQCStatus) ? "-1" : syncQCStatus;

            var en = new UnitStatusModel
            {
                act = "GetlistUnitStatusByProjectNEW",
                project_id = projectId,
                unit_status = unitStatus,
                build_status = buildStatus,
                sync_qc_status= syncQCStatus
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
                if (string.IsNullOrEmpty(request.contractor_appointment_date))
                {
                    response.message = "กรุณาระบุวันที่นัดตรวจ";
                    return Json(response);
                }
                if (string.IsNullOrEmpty(request.contractor_appointment_timeStart))
                {
                    response.message = "กรุณาระบุเวลาที่นัดตรวจ";
                    return Json(response);
                }
                if (string.IsNullOrEmpty(request.qc_response_date))
                {
                    response.message = "กรุณาระบุวันที่ QC5";
                    return Json(response);
                }


                var requestCrmUser = new RequestPostModel.Get_User_CRM.Sends
                {
                    email = FormatExtension.NullToString(Request.Cookies["CST.Email"])
                    //email = "aukkaraded@assetwise.co.th"
                };

                var apiResponse = _WebAPIRestService.CentralizeGetUserCRM(requestCrmUser).GetAwaiter().GetResult();
                if (apiResponse.status != 1)
                {
                    response.message = "ไม่พบ User ใน CRM";
                    return Json(response);
                }

                Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                request.qc_response_user_id = apiResponse.UserID;
                request.qc_type = "qc5_pass";
                request.CQTUserID = userid;

                var apiQcStatusUpdateQc5Response = _WebAPIRestService.QcStatusUpdateQc5(request).GetAwaiter().GetResult();
                if (apiQcStatusUpdateQc5Response.Status != 1)
                {
                    response.message = "Sync CRM ไม่สำเร็จ: " + apiQcStatusUpdateQc5Response.message;
                    return Json(response);
                }

                bool Results = _QC5CheckService.InsertQCSync(request);
                if (!Results)
                {
                    response.message = "Sync CRM สำเร็จแต่บันทึกข้อมูลลงฐานข้อมูลไม่สำเร็จกรุณาติดต่อทีม IT";
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

        [HttpPost]
        public IActionResult GetSyncQCDetail([FromForm] QC_Status_Update_QC5.Getdetail request)
        {
            var data = _QC5CheckService.GetQCSyncDetail(request);

            if (data != null)
            {
                return Json(new
                {
                    success = true,
                    data = data
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "ไม่พบข้อมูล"
                });
            }
        }


    }
}
