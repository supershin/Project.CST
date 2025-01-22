using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.ProjectBluePrint;
using Project.ConstructionTracking.Web.Services;
using static Project.ConstructionTracking.Web.Models.ProjectBluePrint.ProjectBluePrintModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ProjectBluePrintController : BaseController
    {
        private readonly IGetDDLService _getDDLService;
        private readonly IProjectBluePrintService _ProjectBluePrintService;
        private readonly IHostEnvironment _hosting;
        public ProjectBluePrintController(IGetDDLService getDDLService, IProjectBluePrintService ProjectBluePrintService, IHostEnvironment hosting)
        {
            _getDDLService = getDDLService;
            _ProjectBluePrintService = ProjectBluePrintService;
            _hosting = hosting;
        }

        public IActionResult Index()
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.ListDDLProject = ListProject;

            return View();
        }

        [HttpPost]
        public IActionResult UploadBlueprint(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return Json(new { success = false, message = "Please upload a valid image." });
            }

            string uploadsPath = Path.Combine("wwwroot/images");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            string inputPath = Path.Combine(uploadsPath, imageFile.FileName);
            using (var stream = new FileStream(inputPath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            string imagePath = "/images/" + imageFile.FileName;
            return Json(new { success = true, imagePath });
        }

        [HttpGet]
        public IActionResult GetDDLUnitList(Guid projectId)
        {
            var ddlModel = new GetDDL { Act = "UnitForInsertBluePrint", ValueGuid = projectId };
            List<GetDDL> ListUnit = _getDDLService.GetDDLList(ddlModel);
            return Json(ListUnit);
        }

        [HttpPost]
        public IActionResult SaveBlueprintElements([FromBody] List<ProjectBluePrintModel.BlueprintElementModel> elements)
        {
            if (elements == null || !elements.Any())
            {
                return Json(new { success = false, message = "No elements to save." });
            }

            try
            {
                _ProjectBluePrintService.SaveBlueprintElements(elements);

                return Json(new { success = true, message = "Blueprint elements saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult GetBlueprintElements(Guid projectId)
        {
            List<ProjectBluePrintModel.BlueprintElementModel> listBlueprintElement = _ProjectBluePrintService.GetListProjectBlueprintElements(projectId);

            return Json(listBlueprintElement);
        }

        [HttpGet]
        public IActionResult GetListImageProjectFloorPlan(Guid ProjectID)
        {
            if (ProjectID == Guid.Empty)
            {
                return BadRequest("Invalid Project ID.");
            }

            var listImageProjectFloorPlan = _ProjectBluePrintService.GetListImageProjectFloorPlan(ProjectID);
            return PartialView("PartialTableListImageProjectBluePrint", listImageProjectFloorPlan);
        }

        [HttpPost]
        public IActionResult InsertImageProjectFloorPlan(InsertImageProjectFloorPlanModel model)
        {
            try
            {
                model.UserID = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                model.ApplicationPath = _hosting.ContentRootPath;
                _ProjectBluePrintService.InsertImageProjectFloorPlan(model);
                return Json(new { success = true, message = "บันทึกข้อมูลสำเร็จ" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult RemoveImageProjectFloorPlan(RemoveImageProjectFloorPlanModel model)
        {
            try
            {
                _ProjectBluePrintService.RemoveImageProjectFloorPlan(model);
                return Ok(new { success = true, message = "ลบรูปภาพสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult RemoveMarkerProjectBluePrint(RemoveMarkerProjectBluePrintModel model)
        {
            try
            {
                _ProjectBluePrintService.RemoveMarkerProjectBluePrint(model);
                return Ok(new { success = true, message = "ลบรูปภาพสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
    }
}
