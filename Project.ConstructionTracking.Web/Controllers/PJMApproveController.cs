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
        public IActionResult Index(Guid UnitFormID)
        {
            var filterData = new PJMApproveModel.GetlistChecklistPC { UnitFormID = UnitFormID };
            List<PJMApproveModel.GetlistChecklistPC> ListChecklistPJMApprove = _PJMApproveService.GetChecklistPJMApprove(filterData);
            if (ListChecklistPJMApprove != null && ListChecklistPJMApprove.Count > 0)
            {
                var listPJMApprove = ListChecklistPJMApprove[0]; // Assuming there is only one row in listStatus
                ViewBag.ProjectName = listPJMApprove.ProjectName;
                ViewBag.UnitCode = listPJMApprove.UnitCode;
                ViewBag.FormName = listPJMApprove.FormName;
            }
            ViewBag.ListChecklistPJMApprove = ListChecklistPJMApprove;
            return View(ListChecklistPJMApprove);
        }
    }
}
