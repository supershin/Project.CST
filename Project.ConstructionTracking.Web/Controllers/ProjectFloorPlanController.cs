using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.ProjectBluePrint;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ProjectFloorPlanController : BaseController
    {
        private readonly IGetDDLService _getDDLService;
        private readonly IProjectBluePrintService _ProjectBluePrintService;
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ReportProjectFloorPlanProvider;

        public ProjectFloorPlanController(IGetDDLService getDDLService, IProjectBluePrintService ProjectBluePrintService, IHostEnvironment hosting, MasterManagementProviderProject ReportProjectFloorPlanProvider)
        {
            _getDDLService = getDDLService;
            _ProjectBluePrintService = ProjectBluePrintService;
            _hosting = hosting;
            _ReportProjectFloorPlanProvider = ReportProjectFloorPlanProvider;
        }

        public IActionResult Index()
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.ListDDLProject = ListProject;

            return View();
        }

        [HttpGet]
        public IActionResult GetListImageProjectFloorPlan(Guid ProjectID)
        {
            if (ProjectID == Guid.Empty)
            {
                return BadRequest("Invalid Project ID.");
            }

            var listImageProjectFloorPlan = _ProjectBluePrintService.GetListImageProjectFloorPlan(ProjectID);
            return PartialView("PartialTableListImageProjectBluePrint", listImageProjectFloorPlan);
        }

        public IActionResult GetBlueprintElements(Guid ProjectFloorPlanID)
        {

            List<ReportProjectFloorPlanModel> ListdataReportProjectFloorPlan = new List<ReportProjectFloorPlanModel>();

            var EN = new ReportProjectFloorPlanModel
            {
                act = "ReportProjectFloorPlan",
                project_floor_plan_id = Commons.FormatExtension.NullToString(ProjectFloorPlanID)
            };

            ListdataReportProjectFloorPlan = _ReportProjectFloorPlanProvider.sp_get_report_ProjectFloorPlan(EN);

            return Json(ListdataReportProjectFloorPlan);
        }

        public IActionResult GetUnitDetail(Guid UnitID)
        {

           ReportProjectFloorPlanByUnitModel getdataReportProjectFloorPlan = new ReportProjectFloorPlanByUnitModel();

            var EN = new ReportProjectFloorPlanByUnitModel
            {
                act = "ReportProjectFloorPlanByUnit",
                unit_id = Commons.FormatExtension.NullToString(UnitID)
            };

            getdataReportProjectFloorPlan = _ReportProjectFloorPlanProvider.sp_get_report_ProjectFloorPlanByUnit(EN);

            return Json(getdataReportProjectFloorPlan);
        }
    }
}
