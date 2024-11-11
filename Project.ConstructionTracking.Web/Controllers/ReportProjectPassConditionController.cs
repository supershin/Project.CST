using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;
using System.Globalization;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReportProjectPassConditionController : BaseController
    {
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ProjectProgressReportProvider;
        private readonly IGetDDLService _getDDLService;

        public ReportProjectPassConditionController(IHostEnvironment hosting, MasterManagementProviderProject ProjectProgressReportProvider, IGetDDLService getDDLService)
        {
            _hosting = hosting;
            _ProjectProgressReportProvider = ProjectProgressReportProvider;
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {
            // Retrieve the projectId from the cookie, if it exists
            string projectId = Request.Cookies.ContainsKey("Re2CkselectedProjectId") ? Request.Cookies["Re2CkselectedProjectId"] : null;

            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

            // Check if projectId exists before fetching data
            List<ReportProjectPassConditionModel> ProjectProgressReportlists = new List<ReportProjectPassConditionModel>();
            if (!string.IsNullOrEmpty(projectId))
            {
                var en = new ReportProjectPassConditionModel
                {
                    act = "ReportProjectPassCondition",
                    project_id = "0CC60DA9-9AC5-4DF6-871E-B10FB0257B4B",
                    unit_id = "",
                    unit_status = "",
                    build_status = "",
                    start_date = "",
                    end_date = ""
                };

                ProjectProgressReportlists = _ProjectProgressReportProvider.sp_get_report_Project_PassCondition(en);
            }
            ViewBag.SelectedProjectId = projectId;
            return View(ProjectProgressReportlists);
        }

        [HttpGet]
        public IActionResult ExportToExcel(string projectId, string projectName, string startdate, string enddate)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Report Project PassCondition");

                // Add filter values
                worksheet.Cell(1, 1).Value = "ตัวเลือกการค้นหา :";
                worksheet.Cell(2, 1).Value = "โครงการ :";
                worksheet.Cell(2, 2).Value = projectName;
                worksheet.Cell(3, 1).Value = "Start Date :";
                worksheet.Cell(3, 2).Value = startdate;
                worksheet.Cell(4, 1).Value = "End Date :";
                worksheet.Cell(4, 2).Value = enddate;

                // Style for filter labels
                worksheet.Range("A1:A4").Style.Font.Bold = true;

                // Add headers for the table
                int headerRowIndex = 6; // Adjusted to allow room for filter values
                worksheet.Cell(headerRowIndex, 1).Value = "No.";
                worksheet.Cell(headerRowIndex, 2).Value = "แปลงที่";
                worksheet.Cell(headerRowIndex, 3).Value = "งวดที่";
                worksheet.Cell(headerRowIndex, 4).Value = "รายการตรวจ";
                worksheet.Cell(headerRowIndex, 5).Value = "วันที่ PE ส่งเรื่อง";
                worksheet.Cell(headerRowIndex, 6).Value = "เหตุผลจากวิศวกร\nผู้ควบคุมงาน";
                worksheet.Cell(headerRowIndex, 7).Value = "วันที่ PM ยืนยัน";
                worksheet.Cell(headerRowIndex, 8).Value = "สถานะ PM ยืนยัน";
                worksheet.Cell(headerRowIndex, 9).Value = "ความคิดเห็นจาก PM";
                worksheet.Cell(headerRowIndex, 10).Value = "วันที่ PJM ยืนยัน";
                worksheet.Cell(headerRowIndex, 11).Value = "สถานะ PJM ยืนยัน";
                worksheet.Cell(headerRowIndex, 12).Value = "ความคิดเห็นจาก \nPJM Head";
                worksheet.Cell(headerRowIndex, 13).Value = "วันที่ขอปลดล๊อค";
                worksheet.Cell(headerRowIndex, 14).Value = "วันที่อนุมัติปลดล๊อค";
                worksheet.Cell(headerRowIndex, 15).Value = "สถานะปัจจุบัน";

                // Prepare date filtering values by parsing dates from dd/mm/yyyy to DateTime
                DateTime? parsedStartDate = !string.IsNullOrEmpty(startdate) ? DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
                DateTime? parsedEndDate = !string.IsNullOrEmpty(enddate) ? DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;

                // Data retrieval based on the selected projectId
                var en = new ReportProjectPassConditionModel
                {
                    act = "ReportProjectPassCondition",
                    project_id = projectId,
                    unit_id = "",
                    unit_status = "",
                    build_status = "",
                    start_date = parsedStartDate.HasValue ? parsedStartDate.Value.ToString("yyyy-MM-dd") : "",
                    end_date = parsedEndDate.HasValue ? parsedEndDate.Value.ToString("yyyy-MM-dd") : ""
                };

                List<ReportProjectPassConditionModel> reportData = _ProjectProgressReportProvider.sp_get_report_Project_PassCondition(en);

                if (reportData == null || reportData.Count == 0)
                {
                    worksheet.Cell(headerRowIndex + 1, 1).Value = "ไม่มีข้อมูล";
                    worksheet.Range($"A{headerRowIndex + 1}:O{headerRowIndex + 1}").Merge(); // Adjust the merge range for 15 columns
                    worksheet.Cell(headerRowIndex + 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(headerRowIndex + 1, 1).Style.Font.Bold = true;
                }
                else
                {
                    // Add data rows
                    int rowIndex = headerRowIndex + 1;
                    foreach (var item in reportData)
                    {
                        worksheet.Cell(rowIndex, 1).Value = item.index; // No.
                        worksheet.Cell(rowIndex, 2).Value = item.UnitCode; // แปลงที่
                        worksheet.Cell(rowIndex, 3).Value = item.FormName; // งวดที่
                        worksheet.Cell(rowIndex, 4).Value = item.FormGroupName; // รายการตรวจ
                        worksheet.Cell(rowIndex, 5).Value = $"{item.PEActionDate} {item.PEActionName}"; // วันที่ PE ส่งเรื่อง
                        worksheet.Cell(rowIndex, 6).Value = item.PEPCRemark; // เหตุผลจากวิศวกรผู้ควบคุมงาน
                        worksheet.Cell(rowIndex, 7).Value = $"{item.PMActionDate} {item.PMActionName}"; // วันที่ PM ยืนยัน
                        worksheet.Cell(rowIndex, 8).Value = item.PMStatusIDName; // สถานะ PM ยืนยัน
                        worksheet.Cell(rowIndex, 9).Value = item.PMPCRemark; // ความคิดเห็นจาก PM
                        worksheet.Cell(rowIndex, 10).Value = $"{item.PJMActionDate} {item.PJMActionName}"; // วันที่ PJM ยืนยัน
                        worksheet.Cell(rowIndex, 11).Value = item.PJMStatusIDName; // สถานะ PJM ยืนยัน
                        worksheet.Cell(rowIndex, 12).Value = item.PJMPCRemark; // ความคิดเห็นจาก PJM Head
                        worksheet.Cell(rowIndex, 13).Value = item.PERequestUnlock; // วันที่ขอปลดล๊อค
                        worksheet.Cell(rowIndex, 14).Value = item.PMUnlock; // วันที่อนุมัติปลดล๊อค
                        worksheet.Cell(rowIndex, 15).Value = item.PCStatusName; // สถานะปัจจุบัน

                        rowIndex++;
                    }
                }

                // Save to memory stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0; // Reset the stream position to the beginning

                    // Return the file directly to the client
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ProjectProgressReport.xlsx"
                    );
                }
            }
        }



        [HttpPost]
        public IActionResult SearchProject(string projectId, string startdate, string enddate)
        {
            // Save the projectId in a cookie
            Response.Cookies.Append("Re2CkselectedProjectId", projectId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7) // Set cookie to expire in 7 days
            });

            // Prepare date filtering values by parsing dates from dd/mm/yyyy to DateTime
            DateTime? parsedStartDate = !string.IsNullOrEmpty(startdate) ? DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
            DateTime? parsedEndDate = !string.IsNullOrEmpty(enddate) ? DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;

            var en = new ReportProjectPassConditionModel
            {
                act = "ReportProjectPassCondition",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                start_date = parsedStartDate.HasValue ? parsedStartDate.Value.ToString("yyyy-MM-dd") : "",
                end_date = parsedEndDate.HasValue ? parsedEndDate.Value.ToString("yyyy-MM-dd") : ""
            };

            List<ReportProjectPassConditionModel> filteredProjects = _ProjectProgressReportProvider.sp_get_report_Project_PassCondition(en);

            // Return the filtered data to the partial view
            return PartialView("PartialTable", filteredProjects);
        }


    }
}
