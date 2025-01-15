using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ProjectBluePrintController : BaseController
    {
        private readonly IGetDDLService _getDDLService;

        public ProjectBluePrintController(IGetDDLService getDDLService)
        {
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {
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

    }
}
