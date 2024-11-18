using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;
using Project.ConstructionTracking.Web.Commons;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitPaymentController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        private readonly IGetDDLService _getDDLService;
        private readonly IUnitFormPaymentService _UnitFormPaymentService;
        private readonly IHostEnvironment _hosting;

        public UnitPaymentController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService , IUnitFormPaymentService UnitFormPaymentService, IHostEnvironment hosting)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
            _UnitFormPaymentService = UnitFormPaymentService;
            _hosting = hosting;
        }
        public IActionResult Index()
        {
            
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

            var userID = Request.Cookies["CST.ID"];

            var en = new WorkPeriodModel
            {
                act = "Getworkperiodlist",
                project_id = (ListProject != null && ListProject.Count > 0) ? ListProject[0].ValueGuid.ToString() : string.Empty,
                unit_id = "",
                unit_status = "",
                user_id = userID

            };
            List<WorkPeriodModel> WorkPeriodlists = _unitstatusProvider.sp_get_workperiod(en);

            return View(WorkPeriodlists);
        }

        [HttpGet]
        public IActionResult GetDDLUnitPass(Guid ProjectID)
        {
            var ddlModel = new GetDDL { Act = "GetListUnitPass", GuID = ProjectID };
            List<GetDDL> ListUnitPass = _getDDLService.GetDDLList(ddlModel);
            return Json(ListUnitPass);
        }

        [HttpGet]
        public IActionResult GetUnitFormGRDetail(Guid UnitFormID)
        {
            var Model = new UnitFormPaymentModel.getUnitFormGRDetail { UnitFormID = UnitFormID };
            UnitFormPaymentModel.getUnitFormGRDetail UnitFormGRDetail = _UnitFormPaymentService.getUnitFormGRDetail(Model);
            return Json(UnitFormGRDetail);
        }

        [HttpPost]
        public IActionResult SaveUnitFormGRPaymentData(UnitFormPaymentModel.insertGRPayment Model)
        {
            try
            {
                string returnmessage = "";
                int RoleID = int.TryParse(Request.Cookies["CST.Role"], out var tempRoleInt) ? tempRoleInt : -1;
                if (RoleID == SystemConstant.UserRole.ADMIN)
                {
                    Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                    Model.UserID = userid;
                    returnmessage = _UnitFormPaymentService.InsertNewGRPayment(Model);
                    return Json(new { success = true, message = returnmessage });
                }
                else
                {
                    returnmessage = "สิทธิ์บันทึกเบิกงวดไม่ถูกต้อง";
                    return Json(new { success = false, message = returnmessage });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }
    }
}
