using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReportAverageNumberofPassedInspectionsController : BaseController
    {
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ReportWorkloadAndInspectionResultsProvider;
        private readonly IGetDDLService _getDDLService;

        public ReportAverageNumberofPassedInspectionsController(IHostEnvironment hosting, MasterManagementProviderProject ReportWorkloadAndInspectionResultsProvider, IGetDDLService getDDLService)
        {
            _hosting = hosting;
            _ReportWorkloadAndInspectionResultsProvider = ReportWorkloadAndInspectionResultsProvider;
            _getDDLService = getDDLService;
        }


        public IActionResult Index()
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

            string projectId = Request.Cookies.ContainsKey("ReportWorkloadAndInspectionResultsselectedProjectId") ? Request.Cookies["ReportWorkloadAndInspectionResultsselectedProjectId"] : null;

            List<ReportAverageNumberofPassedInspectionsModel> ReportAverageNumberofPassedInspections = new List<ReportAverageNumberofPassedInspectionsModel>();

            var en = new ReportAverageNumberofPassedInspectionsModel
            {
                act = "ReportAverageNumberofPassedInspections",
                project_id = "0CC60DA9-9AC5-4DF6-871E-B10FB0257B4B",
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = "",
                start_date = "",
                end_date = ""
            };

            ReportAverageNumberofPassedInspections = _ReportWorkloadAndInspectionResultsProvider.sp_get_report_average_numberof_passed_inspections(en);
            ViewBag.SelectedProjectId = projectId;

            return View(ReportAverageNumberofPassedInspections);
        }



        [HttpGet]
        public IActionResult GetDDLVenderByProject(Guid ProjectId)
        {
            var ddlModel = new GetDDL { Act = "GetListDDLCompanyVenderInProject", GuID = ProjectId };
            List<GetDDL> ListDDLCompanyVenderInProject = _getDDLService.GetDDLList(ddlModel);
            return Json(ListDDLCompanyVenderInProject);
        }


    }
}
