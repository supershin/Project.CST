using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitPaymentController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        private readonly IGetDDLService _getDDLService;

        public UnitPaymentController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
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
            var ddlModel = new GetDDL { Act = "GetListUnitPass", ValueGuid = ProjectID };
            List<GetDDL> ListUnitPass = _getDDLService.GetDDLList(ddlModel);
            return Json(ListUnitPass);
        }
    }
}
