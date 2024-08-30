using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class PMFormcheckController : BaseController
    {
        private readonly IPMApproveService _PMApproveService;

        public PMFormcheckController(IPMApproveService PMApproveService)
        {
            _PMApproveService = PMApproveService;
        }

        public IActionResult Index()
        {
            List<PMApproveModel> listPMApproveForm = _PMApproveService.GetPMApproveFormList();
            return View(listPMApproveForm);
        }
    }
}
