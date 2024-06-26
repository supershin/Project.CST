using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
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
            FormCheckListUnitView viewModel = _ProjectFormService.GetFormCheckListUnit(test);
            return View(viewModel);
        }
    }
}
