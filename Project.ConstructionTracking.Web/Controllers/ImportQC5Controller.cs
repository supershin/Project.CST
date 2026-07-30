using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ImportQC5Controller : BaseController
    {
        private readonly IImportQC5Service _ImportQC5Service;

        public ImportQC5Controller(IImportQC5Service importQC5Service)
        {
            _ImportQC5Service = importQC5Service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Preview(IFormFile fileExcel)
        {
            try
            {
                if (fileExcel == null || fileExcel.Length == 0)
                {
                    return Json(new { success = false, message = "กรุณาเลือกไฟล์ Excel (.xlsx)" });
                }

                var result = _ImportQC5Service.PreviewExcel(fileExcel);
                return Json(new { success = true, message = "ตรวจสอบไฟล์สำเร็จ", data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Import(IFormFile fileExcel)
        {
            try
            {
                if (fileExcel == null || fileExcel.Length == 0)
                {
                    return Json(new { success = false, message = "กรุณาเลือกไฟล์ Excel (.xlsx)" });
                }

                var result = _ImportQC5Service.ImportExcel(fileExcel, CurrentUserID);
                return Json(new { success = true, message = $"นำเข้าข้อมูลสำเร็จ {result.ImportedRows} รายการ", data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }
    }
}
