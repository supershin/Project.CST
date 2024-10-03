using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitlistController : BaseController
    {
        private readonly IUnitService _unitService;
        private readonly IGetDDLService _getDDLService;
        private readonly MasterManagementProviderProject _unitstatusProvider;

        public UnitlistController(MasterManagementProviderProject unitstatusProvider, IUnitService unitService, IGetDDLService getDDLService)
        {
            _unitService = unitService;
            _getDDLService = getDDLService;
            _unitstatusProvider = unitstatusProvider;
        }

        public IActionResult Index(string projectId, string projectName)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.ProjectName = projectName;

            var en = new UnitListModel
            {
                act = "GetUnitlist",
                project_id = projectId,
                unit_id = "",
                unit_status = "",

            };
            List<UnitListModel> UnitList = _unitstatusProvider.sp_get_UnitList(en);


            var ddlModelUnitCode = new GetDDL { Act = "Unit", ValueGuid = Guid.TryParse(projectId, out var parsedGuid) ? parsedGuid : (Guid?)null };
            List<GetDDL> ListUnit = _getDDLService.GetDDLList(ddlModelUnitCode);

            ViewBag.DDLListUnit = ListUnit;

            var ddlModel = new GetDDL { Act = "Ext", ID = 4 };
            List<GetDDL> ddlList = _getDDLService.GetDDLList(ddlModel);

            ViewBag.DDLList = ddlList;

            return View(UnitList);
        }

        [HttpGet]
        public IActionResult SearchData(Guid? selectedProjectValue, string selectedUnitValue, string selectedUnitFormStatus)
        {
            var userID = Request.Cookies["CST.ID"];

            var en = new UnitListModel
            {
                act = "GetUnitlist",
                project_id = selectedProjectValue?.ToString() ?? "",
                unit_id = selectedUnitValue ?? "",
                unit_status = selectedUnitFormStatus ?? ""
            };

            List<UnitListModel> UnitList = _unitstatusProvider.sp_get_UnitList(en);

            return Json(UnitList);
        }

        [HttpPost]
        public IActionResult GoToByRole(Guid projectId, string projectName, Guid unitId , string UnitCode , string UnitStatusName)
        {
            var RoleID = Request.Cookies["CST.Role"];
            if (RoleID == SystemConstant.UserRole.PE.ToString())
            {
                return RedirectToAction("Index", "SummaryUnitForm", new { unitId, projectId, projectName , UnitCode , UnitStatusName });
            }
            else if (RoleID == SystemConstant.UserRole.PM.ToString())
            {
                return RedirectToAction("Index", "SummaryUnitForm", new { unitId, projectId, projectName, UnitCode, UnitStatusName });
            }
            else if (RoleID == SystemConstant.UserRole.PJM.ToString())
            {
                return RedirectToAction("Index", "SummaryUnitForm", new { unitId, projectId, projectName, UnitCode, UnitStatusName });
            }
            else if (RoleID == SystemConstant.UserRole.QC.ToString())
            {
                int Seq = 1;
                return RedirectToAction("Index", "SummaryUnitQC", new { projectId, projectName, unitId});
            }
            else
            {
                return RedirectToAction("Index", "Unitlist", new { projectId, projectName, unitId });
            }
        }

    }
}
