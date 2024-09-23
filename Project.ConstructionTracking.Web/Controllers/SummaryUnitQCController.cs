using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models.QCModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class SummaryUnitQCController : BaseController
    {
        private readonly IQcSummaryService _qcSummaryService;
        public SummaryUnitQCController(IQcSummaryService qcSummaryService)
        {
            _qcSummaryService = qcSummaryService;
        }

        public IActionResult Index(Guid projectId, string projectName, Guid unitId)
        {
            QcSummaryResp qcSummary = _qcSummaryService.GetQcSummary(projectId, unitId);
            qcSummary.ProjectName = projectName;

            return View(qcSummary);  
        }
    }
}
