using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitStatusByProjectController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;

        public UnitStatusByProjectController(MasterManagementProviderProject unitstatusProvider)
        {
            _unitstatusProvider = unitstatusProvider;
        }
        public IActionResult Index()
        {
            var en = new UnitStatusModel
            {
                act = "GetlistUnitStatustest",
                project_id = "",
                unit_id = "",
                unit_status = -1

            };
            List<UnitStatusModel> unitstatuslists = _unitstatusProvider.sp_get_unitstatus(en);
            return View(unitstatuslists);
        }
    }
}
