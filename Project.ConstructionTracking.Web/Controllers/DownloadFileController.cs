using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class DownloadFileController : BaseController
    {

        private readonly IHostEnvironment _hosting;

        public DownloadFileController(IHostEnvironment hosting)
        {
            _hosting = hosting;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult DownloadFile(string act)
        {
            try
            {
                // Initialize variables for file name and path
                string fileName = string.Empty;
                string relativePath = string.Empty;

                // Determine the file to download based on the `act` parameter
                switch (act)
                {
                    case "Adminusermanual":
                        fileName = "คู่มือ_Admin_การ_set_up_ระบบ_CQT.pdf";
                        relativePath = Path.Combine("wwwroot", "FileUserManual", fileName);
                        break;

                    case "AdminOnlineBilling":
                        fileName = "คู่มือ_Admin_การส่งข้อมูล_QCT _ไป_Online_Billing.pdf";
                        relativePath = Path.Combine("wwwroot", "FileUserManual", fileName);
                        break;

                    case "PESEPMusermanual":
                        fileName = "คู่มือ_PE_SE_PM_ระบบ_CQT.pdf";
                        relativePath = Path.Combine("wwwroot", "FileUserManual", fileName);
                        break;

                    case "QCusermanual":
                        fileName = "คู่มือ_QC_ระบบ_CQT.pdf";
                        relativePath = Path.Combine("wwwroot", "FileUserManual", fileName);
                        break;

                    default:
                        return BadRequest("Invalid action specified.");
                }

                // Construct the full file path
                var filePath = Path.Combine(_hosting.ContentRootPath, relativePath);

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("ไม่พบไฟล์");
                }

                // Return the file as a download
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, GetMimeType(fileName), fileName);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, "เกิดข้อผิดพลาดขณะประมวลผลคำขอของคุณ");
            }
        }

        // Helper method to determine MIME type based on file extension
        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream", // Default binary type
            };
        }


    }
}
