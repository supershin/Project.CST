using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitStatusByUnitController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        public UnitStatusByUnitController(MasterManagementProviderProject unitstatusProvider)
        {
            _unitstatusProvider = unitstatusProvider;
        }
        public IActionResult Index(string ProjectID, string UnitID)
        {

            var en = new UnitFormStatusModel
            {
                act = "UnitFormStatusByUnit",
                project_id = ProjectID,
                unit_id = UnitID,
                unit_status = "",
                build_status = ""

            };

            List<UnitFormStatusModel> unitstatuslists = _unitstatusProvider.sp_get_UnitFormStatusByUnit(en);
            ViewBag.UnitCode = unitstatuslists[0]?.UnitCode;
            return View(unitstatuslists);
        }
    }
}
