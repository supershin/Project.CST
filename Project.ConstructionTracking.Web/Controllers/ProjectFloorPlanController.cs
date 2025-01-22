using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ProjectFloorPlanController : BaseController
    {
        private readonly IGetDDLService _getDDLService;
        private readonly IProjectBluePrintService _ProjectBluePrintService;
        private readonly IHostEnvironment _hosting;
        public ProjectFloorPlanController(IGetDDLService getDDLService, IProjectBluePrintService ProjectBluePrintService, IHostEnvironment hosting)
        {
            _getDDLService = getDDLService;
            _ProjectBluePrintService = ProjectBluePrintService;
            _hosting = hosting;
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
    }
}
