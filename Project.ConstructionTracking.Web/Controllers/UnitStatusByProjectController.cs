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

            var en2 = new PEMyTaskModel
            {
                act = "listPEtask",
                project_id = "",
                unit_id = "",
                unit_status = "",
                user_id = "D0E92B67-4FF7-4284-892F-25A4BB3722FA"

            };
            List<PEMyTaskModel> unitstatuslists3 = _unitstatusProvider.sp_get_mytask_pe(en2);

            //ViewBag.PEMyTaskModel = unitstatuslists3;

            return View(unitstatuslists);
        }
    }
}
