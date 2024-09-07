using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class MyTaskPEController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;

        public MyTaskPEController(MasterManagementProviderProject unitstatusProvider)
        {
            _unitstatusProvider = unitstatusProvider;
        }

        public IActionResult Index()
        {
            var en2 = new PEMyTaskModel
            {
                act = "listPEtask",
                project_id = "",
                unit_id = "",
                unit_status = "",
                user_id = "D0E92B67-4FF7-4284-892F-25A4BB3722FA"

            };
            List<PEMyTaskModel> unitstatuslists3 = _unitstatusProvider.sp_get_mytask_pe(en2);

            return View(unitstatuslists3);
        }
    }
}
