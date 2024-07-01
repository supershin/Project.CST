using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    [Route("FormCheckList")]
    public class FormCheckListController : Controller
    {
        private readonly IProjectFormService _ProjectFormService;

        public FormCheckListController(IProjectFormService ProjectFormService)
        {
            _ProjectFormService = ProjectFormService;
        }
        [HttpPost("Index")]
        public IActionResult Index(TrackingUnitModel model)
        {
            //Guid unitID = model.UnitID;
            int ID = model.ID;
            int test = 1;
            int test2 = 1;
            FormCheckListUnitView viewModel = _ProjectFormService.GetFormCheckListUnit(test, test2);
            return View(viewModel);
        }
        //[HttpPost("Indextest")]
        //public IActionResult Indextest(UnitModel model)
        //{
        //    // Access the ID property from the model
        //    int test = 1; // assuming the ID property is in your UnitModel
        //    int test2 = 1; // This can be any other parameter you need

        //    // Call your service method with the parameters
        //    FormCheckListUnitView viewModel = _ProjectFormService.GetFormCheckListUnit(test, test2);

        //    // Return the view with the viewModel
        //    return View(viewModel);
        //}

        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus(UnitForm model)
        {
            _ProjectFormService.InsertFormCheckListUnit(model);
            return RedirectToAction("Success"); // Redirect to a success page or wherever appropriate
            //return Ok(new { success = true });
        }

    }
}
