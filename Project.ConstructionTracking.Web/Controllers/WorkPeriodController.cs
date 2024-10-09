using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class WorkPeriodController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        private readonly IGetDDLService _getDDLService;

        public WorkPeriodController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {

            var userID = Request.Cookies["CST.ID"];
            var ddlModel = new GetDDL { Act = "Project", UserID = Guid.Parse(userID) };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

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
    }
}
