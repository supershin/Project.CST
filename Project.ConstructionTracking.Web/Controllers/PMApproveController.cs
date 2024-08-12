using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services; // Assuming the GetApproveFormcheckList class is in the Services namespace

namespace Project.ConstructionTracking.Web.Controllers
{
    public class PMApproveController : Controller
    {
        private readonly IPMApproveService _PMApproveService;

        public PMApproveController(IPMApproveService PMApproveService)
        {
            _PMApproveService = PMApproveService;
        }

        public IActionResult Index(Guid unitId, int formId)
        {
            // Use the GetApproveFormcheckList method from your service to get the data
            var Model = new ApproveFormcheckModel { UnitID = unitId, FormID = formId };
            List<ApproveFormcheckModel> listmodel = _PMApproveService.GetApproveFormcheckList(Model);

            // Pass the model to the view
            return View(listmodel);
        }
    }
}
