using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class PJMFormcheckController : BaseController
    {
        private readonly IPJMApproveService _PJMApproveService;
        public PJMFormcheckController(IPJMApproveService PJMApproveService)
        {
            _PJMApproveService = PJMApproveService;
        }
        public IActionResult Index()
        {
            var filterData = new PJMApproveModel.filterData { ActionType = "submit", StatusID = 5 };
            List<PJMApproveModel.GetlistUnitDetail> ListPJMApprove = _PJMApproveService.GetListPJMApprove(filterData);

            if (ListPJMApprove != null)
            {
                // Set ViewBag properties based on the result
                ViewBag.ListPJMApprove = ListPJMApprove;
            }

            return View(ListPJMApprove);
        }
    }
}
