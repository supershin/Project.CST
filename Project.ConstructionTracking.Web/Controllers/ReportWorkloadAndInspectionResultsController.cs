using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReportWorkloadAndInspectionResultsController : BaseController
    {
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ReportWorkloadAndInspectionResultsProvider;
        private readonly IGetDDLService _getDDLService;
        public ReportWorkloadAndInspectionResultsController(IHostEnvironment hosting, MasterManagementProviderProject ReportWorkloadAndInspectionResultsProvider, IGetDDLService getDDLService)
        {
            _hosting = hosting;
            _ReportWorkloadAndInspectionResultsProvider = ReportWorkloadAndInspectionResultsProvider;
            _getDDLService = getDDLService;
        }


        public IActionResult Index()
        {
            List<ReportWorkloadAndInspectionResultsModel> ReportWorkloadAndInspectionResultslists = new List<ReportWorkloadAndInspectionResultsModel>();

            var en = new ReportWorkloadAndInspectionResultsModel
            {
                act = "ReportWorkloadAndInspectionResults",
                project_id = "0CC60DA9-9AC5-4DF6-871E-B10FB0257B4B",
                unit_id = "",
                unit_status = "",
                build_status = "",
                start_date = "2024"
            };

            ReportWorkloadAndInspectionResultslists = _ReportWorkloadAndInspectionResultsProvider.sp_get_report_workload_inspection_results(en);

            return View(ReportWorkloadAndInspectionResultslists);
        }
    }
}
