using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class PJMApproveController : Controller
    {
        private readonly IPJMApproveService _PJMApproveService;
        public PJMApproveController(IPJMApproveService PJMApproveService)
        {
            _PJMApproveService = PJMApproveService;
        }
        public IActionResult Index()
        {
            var filterData = new PJMApproveModel.filterData { ActionType = "submit", StatusID = 6 };
            var ListPJMApprove = _PJMApproveService.GetListPJMApprove(filterData);
            return View(ListPJMApprove);
        }
    }
}
