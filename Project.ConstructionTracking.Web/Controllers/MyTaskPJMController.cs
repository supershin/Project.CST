using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class MyTaskPJMController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        private readonly IGetDDLService _getDDLService;

        public MyTaskPJMController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {
            var userID = Request.Cookies["CST.ID"];
            var ddlModel = new GetDDL { Act = "Project", UserID = Guid.Parse(userID) };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);

            ViewBag.ListProject = ListProject;

            var ddlfilter = new GetDDL { Act = "UnitFormStatus" };
            List<GetDDL> listUnitFormStatus = _getDDLService.GetDDLList(ddlfilter);

            ViewBag.listUnitFormStatus = listUnitFormStatus;

            var en2 = new PJMMyTaskModel
            {
                act = "listPJMtask",
                project_id = "",
                unit_id = "",
                unit_status = "6",
                user_id = userID

            };
            List<PJMMyTaskModel> MyTasklists = _unitstatusProvider.sp_get_mytask_pjm(en2);

            return View(MyTasklists);
        }


        [HttpGet]
        public IActionResult SearchData(Guid? selectedProjectValue, string selectedUnitValue, string selectedUnitFormStatus)
        {
            var userID = Request.Cookies["CST.ID"];

            var en2 = new PJMMyTaskModel
            {
                act = "listPJMtask",
                project_id = selectedProjectValue?.ToString() ?? "",
                unit_id = selectedUnitValue ?? "",
                user_id = userID,
                unit_status = selectedUnitFormStatus ?? ""
            };

            List<PJMMyTaskModel> MyTasklists = _unitstatusProvider.sp_get_mytask_pjm(en2);

            return Json(MyTasklists);
        }


        [HttpGet]
        public IActionResult GetUnitsByProject(Guid projectId)
        {
            var ddlModel = new GetDDL { Act = "Unit", ValueGuid = projectId };
            List<GetDDL> ListUnit = _getDDLService.GetDDLList(ddlModel);

            return Json(ListUnit);
        }
    }
}

