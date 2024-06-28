using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormCheckListController : Controller
    {
        private readonly IProjectFormService _ProjectFormService;

        public FormCheckListController(IProjectFormService ProjectFormService)
        {
            _ProjectFormService = ProjectFormService;
        }

        public IActionResult Index(Guid ID)
        {
            int test = 1;
            int test2 = 1;
            FormCheckListUnitView viewModel = _ProjectFormService.GetFormCheckListUnit(test, test2);
            return View(viewModel);
        }

        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            // Perform your update logic here using request parameters
            // For example, update the status of a checklist item
            // Return a success response
            return Ok(new { success = true });
        }

        public class UpdateStatusRequest
        {
            public int ChecklistID { get; set; }
            public int StatusID { get; set; }
            public string Remark { get; set; }
        }

    }
}
