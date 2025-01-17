using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Models.ProjectBluePrint;
using Project.ConstructionTracking.Web.Repositories;
using Project.ConstructionTracking.Web.Services;
using static Project.ConstructionTracking.Web.Models.PJMApproveModel;
using static Project.ConstructionTracking.Web.Models.ProjectBluePrint.ProjectBluePrintModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ProjectBluePrintController : BaseController
    {
        private readonly IGetDDLService _getDDLService;
        private readonly IProjectBluePrintService _ProjectBluePrintService;
        public ProjectBluePrintController(IGetDDLService getDDLService, IProjectBluePrintService ProjectBluePrintService)
        {
            _getDDLService = getDDLService;
            _ProjectBluePrintService = ProjectBluePrintService;
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
            var ddlModel = new GetDDL { Act = "Unit", ValueGuid = projectId };
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

        //[HttpPost]
        //public IActionResult GetListImageProjectFloorPlan([FromBody] Guid ProjectID)
        //{

        //    List<ProjectBluePrintModel.GetListImageProjectFloorPlanModel> ListImageProjectFloorPlan = _ProjectBluePrintService.GetListImageProjectFloorPlan(ProjectID);

        //    return PartialView("PartialTableListImageProjectBluePrint", ListImageProjectFloorPlan);
        //}
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



    }
}
