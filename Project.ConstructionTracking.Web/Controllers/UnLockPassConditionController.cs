using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnLockPassConditionController : Controller
    {
        private readonly IUnLockPassConditionService _UnLockPassConditionService;
        private readonly IHostEnvironment _hosting;

        public UnLockPassConditionController(IUnLockPassConditionService UnLockPassConditionService, IHostEnvironment hosting)
        {
            _UnLockPassConditionService = UnLockPassConditionService;
            _hosting = hosting;
        }

        public IActionResult Index(Guid UnitFormID, int GroupID)
        {
            //int _GroupID = Int32.Parse(GroupID);
            var model = new UnLockPassConditionModel.GetDataUnlockPC { UnitFormID = UnitFormID, GroupID = GroupID };
            var resultModel = _UnLockPassConditionService.GetListUnlockPC(model);
            if (resultModel != null && resultModel.Count > 0)
            {
                ViewBag.ProjectName = resultModel[0].ProjectName != null ? resultModel[0].ProjectName : string.Empty;
                ViewBag.UnitCode = resultModel[0].UnitCode != null ? resultModel[0].UnitCode : string.Empty;
                ViewBag.FormName = resultModel[0].FormName != null ? resultModel[0].FormName : string.Empty;
                ViewBag.ListGroupPC = resultModel;
            }

            return View(resultModel);
        }
    }
}
