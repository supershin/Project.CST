using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReporQC14FailController : BaseController
    {
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ReportReportQC14FailProvider;
        private readonly IGetDDLService _getDDLService;

        public ReporQC14FailController(IHostEnvironment hosting, MasterManagementProviderProject ReportReportQC14FailProvider, IGetDDLService getDDLService)
        {
            _hosting = hosting;
            _ReportReportQC14FailProvider = ReportReportQC14FailProvider;
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

            string projectId = Request.Cookies.ContainsKey("ReporQC14FailselectedProjectId") ? Request.Cookies["ReporQC14FailselectedProjectId"] : null;

            List<ReportQC14FailModel> ReportQC14Fail = new List<ReportQC14FailModel>();

            var en = new ReportQC14FailModel
            {
                act = "ReporQC1-4Fail",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = "",
                qctype_id = "",
                start_date = "",
                end_date = ""
            };

            ReportQC14Fail = _ReportReportQC14FailProvider.sp_get_report_qc1_4fail(en);
            ViewBag.SelectedProjectId = projectId;

            return View(ReportQC14Fail);
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
