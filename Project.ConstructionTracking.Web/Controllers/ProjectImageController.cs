using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models.ProjectImage;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ProjectImageController : BaseController
    {
        private readonly IProjectImageService _ProjectImageService;
        private readonly IHostEnvironment _hosting;

        public ProjectImageController(IProjectImageService projectImageService, IHostEnvironment hosting)
        {
            _ProjectImageService = projectImageService;
            _hosting = hosting;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetProjectImageList(string? strSearch)
        {
            var listProjectImage = _ProjectImageService.GetProjectImageList(strSearch);
            return PartialView("Partial_List_ProjectImage", listProjectImage);
        }

        [HttpPost]
        public IActionResult SaveProjectImage(ProjectImageModel.SaveProjectImageModel model)
        {
            try
            {
                model.UserID = CurrentUserID;
                model.ApplicationPath = _hosting.ContentRootPath;
                _ProjectImageService.SaveProjectImage(model);
                return Json(new { success = true, message = "บันทึกรูปภาพโครงการสำเร็จ" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult RemoveProjectImage(ProjectImageModel.RemoveProjectImageModel model)
        {
            try
            {
                model.UserID = CurrentUserID;
                _ProjectImageService.RemoveProjectImage(model);
                return Json(new { success = true, message = "ลบรูปภาพโครงการสำเร็จ" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }
    }
}
