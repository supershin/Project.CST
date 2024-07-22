using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitController : Controller
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }
        public IActionResult Formcheck()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }
        public IActionResult Check()
        {
            ViewData["Title"] = "Check";
            return View();
        }
        //public JsonResult GetUnitList(Criteria criteria, DTParamModel param)
        //{
        //    var units = _unitService.GetUnitList(criteria, param);
        //    return Json(
        //                     new
        //                     {
        //                         success = true,
        //                         data = units,
        //                         param.draw,
        //                         iTotalRecords = param.TotalRowCount,
        //                         iTotalDisplayRecords = param.TotalRowCount
        //                     });

        //}

    }
}
