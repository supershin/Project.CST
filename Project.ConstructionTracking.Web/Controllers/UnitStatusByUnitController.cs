using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitStatusByUnitController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        private readonly IGetDDLService _getDDLService;

        public UnitStatusByUnitController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
        }
        public IActionResult Index(string ProjectID, string UnitID)
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" , GuID = FormatExtension.ConvertStringToGuid(ProjectID) };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.ProjectName = ListProject?[0]?.Text ?? "";

            var en = new UnitFormStatusModel
            {
                act = "UnitFormStatusByUnit",
                project_id = ProjectID,
                unit_id = UnitID,
                unit_status = "",
                build_status = ""

            };

            List<UnitFormStatusModel> unitstatuslists = _unitstatusProvider.sp_get_UnitFormStatusByUnit(en);

            if (unitstatuslists != null && unitstatuslists.Count > 0 && unitstatuslists[0] != null)
            {
                ViewBag.UnitCode = unitstatuslists[0].UnitCode;
            }

            return View(unitstatuslists);
        }
    }
}
