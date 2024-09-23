using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitStatusByProjectController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        private readonly IGetDDLService _getDDLService;

        public UnitStatusByProjectController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
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
                act = "GetlistUnitStatustest",
                project_id = (ListProject != null && ListProject.Count > 0) ? ListProject[0].ValueGuid.ToString() : string.Empty,
                unit_id = "",
                unit_status = "",
                build_status = ""

            };
            List<UnitStatusModel> unitstatuslists = _unitstatusProvider.sp_get_unitstatus(en);

            return View(unitstatuslists);
        }

        [HttpPost]
        public IActionResult SearchUnitStatusByProject(string projectId, string unitStatus, string buildStatus)
        {
            // If any of the parameters are null or empty, handle them accordingly
            projectId = string.IsNullOrEmpty(projectId) ? "" : projectId;
            unitStatus = string.IsNullOrEmpty(unitStatus) ? "" : unitStatus;
            buildStatus = string.IsNullOrEmpty(buildStatus) ? "" : buildStatus;

            var en = new UnitStatusModel
            {
                act = "GetlistUnitStatustest",
                project_id = projectId,
                unit_status = unitStatus,
                build_status = buildStatus
            };

            // Call the provider to get the filtered data
            List<UnitStatusModel> unitstatuslists = _unitstatusProvider.sp_get_unitstatus(en);

            // Return the partial view with the updated model
            return PartialView("PartialTable", unitstatuslists);
        }


    }
}
