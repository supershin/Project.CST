using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReportWorkloadAndInspectionResultsController : BaseController
    {
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ReportWorkloadAndInspectionResultsProvider;
        private readonly IGetDDLService _getDDLService;

        public ReportWorkloadAndInspectionResultsController(IHostEnvironment hosting, MasterManagementProviderProject ReportWorkloadAndInspectionResultsProvider, IGetDDLService getDDLService)
        {
            _hosting = hosting;
            _ReportWorkloadAndInspectionResultsProvider = ReportWorkloadAndInspectionResultsProvider;
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

            string projectId = Request.Cookies.ContainsKey("ReportWorkloadAndInspectionResultsselectedProjectId") ? Request.Cookies["ReportWorkloadAndInspectionResultsselectedProjectId"] : null;

            var currentYear = DateTime.Now.Year;
            var yearList = Enumerable.Range(currentYear - 3, 4).OrderByDescending(y => y).ToList();
            ViewBag.DDLyearList = yearList;

            List<ReportWorkloadAndInspectionResultsModel> ReportWorkloadAndInspectionResultslists = new List<ReportWorkloadAndInspectionResultsModel>();

            var en = new ReportWorkloadAndInspectionResultsModel
            {
                act = "ReportWorkloadAndInspectionResults",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = "",
                start_date = currentYear.ToString()
            };

            ReportWorkloadAndInspectionResultslists = _ReportWorkloadAndInspectionResultsProvider.sp_get_report_workload_inspection_results(en);
            ViewBag.SelectedProjectId = projectId;
            return View(ReportWorkloadAndInspectionResultslists);
        }

        [HttpGet]
        public IActionResult GetDDLVenderByProject(Guid ProjectId)
        {
            var ddlModel = new GetDDL { Act = "GetListDDLCompanyVenderInProject", GuID = ProjectId };
            List<GetDDL> ListDDLCompanyVenderInProject = _getDDLService.GetDDLList(ddlModel);
            return Json(ListDDLCompanyVenderInProject);
        }

        [HttpPost]
        public IActionResult OnclickSearch(string projectId, string CompanyvenderID, string year)
        {

            // Save the projectId in a cookie
            Response.Cookies.Append("ReportWorkloadAndInspectionResultsselectedProjectId", projectId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7) // Set cookie to expire in 7 days
            });

            var en = new ReportWorkloadAndInspectionResultsModel
            {
                act = "ReportWorkloadAndInspectionResults",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = Commons.FormatExtension.NullToString(CompanyvenderID),
                start_date = year
            };

            List<ReportWorkloadAndInspectionResultsModel> Datatable = _ReportWorkloadAndInspectionResultsProvider.sp_get_report_workload_inspection_results(en);

            return PartialView("PartialTable", Datatable);
        }

        [HttpGet]
        public IActionResult ExportToExcel(Guid projectId, string companyVendorId, string year)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("รายงานปริมาณงานและผลการตรวจ");

                DateTime Datenow = DateTime.Now;

                var ddlModel = new GetDDL { Act = "GetListDDLCompanyVenderInProject", GuID = projectId ,ID = Commons.FormatExtension.Nulltoint(companyVendorId) };
                List<GetDDL> CompanyVenderName = _getDDLService.GetDDLList(ddlModel);

                var ddlModel2 = new GetDDL { Act = "ProjectAdmin" , GuID = projectId };
                List<GetDDL> ProjectName = _getDDLService.GetDDLList(ddlModel2);

                // Add filter values
                worksheet.Cell(1, 1).Value = "ตัวเลือกการค้นหา";
                worksheet.Cell(2, 1).Value = "โครงการ :";
                worksheet.Cell(2, 2).Value = ProjectName != null && ProjectName.Count > 0 ? ProjectName[0].Text : "ไม่พบชื่อโครงการนี้" ;
                worksheet.Cell(3, 1).Value = "ผู้รับเหมา :";
                worksheet.Cell(3, 2).Value = companyVendorId != null ? (CompanyVenderName != null && CompanyVenderName.Count > 0 ? CompanyVenderName[0].Text : "ไม่พบชื่อผู้รับเหมา") : "-- ทั้งหมด --";
                worksheet.Cell(4, 1).Value = "ปีที่ค้นหา :";
                worksheet.Cell(4, 2).Value = year;
                worksheet.Cell(5, 1).Value = "Exprort วันที่ :";
                worksheet.Cell(5, 2).Value = Commons.FormatExtension.FormatDateToDayMonthNameYearTime(Datenow); ;
                worksheet.Range(1, 1, 1, 4).Merge();
                worksheet.Range(2, 2, 2, 6).Merge();
                worksheet.Range(3, 2, 3, 6).Merge();
                worksheet.Range(4, 2, 4, 6).Merge();
                worksheet.Range(5, 2, 5, 6).Merge();

                // Row 1: Main headers
                worksheet.Cell(6, 1).Value = "เดือน";
                worksheet.Cell(6, 2).Value = "QC1";
                worksheet.Cell(6, 7).Value = "QC2";
                worksheet.Cell(6, 12).Value = "QC3";
                worksheet.Cell(6, 17).Value = "QC4";
                worksheet.Cell(6, 22).Value = "QC5";
                worksheet.Cell(6, 27).Value = "QC1-QC5";

                // Merge main headers
                worksheet.Range(6, 2, 6, 6).Merge();
                worksheet.Range(6, 7, 6, 11).Merge();
                worksheet.Range(6, 12, 6, 16).Merge();
                worksheet.Range(6, 17, 6, 21).Merge();
                worksheet.Range(6, 22, 6, 26).Merge();
                worksheet.Range(6, 27, 6, 31).Merge();

                // Row 2: Sub-headers
                worksheet.Cell(7, 2).Value = "ผ่าน";
                worksheet.Cell(7, 3).Value = "ไม่ผ่าน";
                worksheet.Cell(7, 4).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(7, 5).Value = "% ผ่าน";
                worksheet.Cell(7, 6).Value = "Total";

                worksheet.Cell(7, 7).Value = "ผ่าน";
                worksheet.Cell(7, 8).Value = "ไม่ผ่าน";
                worksheet.Cell(7, 9).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(7, 10).Value = "% ผ่าน";
                worksheet.Cell(7, 11).Value = "Total";

                worksheet.Cell(7, 12).Value = "ผ่าน";
                worksheet.Cell(7, 13).Value = "ไม่ผ่าน";
                worksheet.Cell(7, 14).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(7, 15).Value = "% ผ่าน";
                worksheet.Cell(7, 16).Value = "Total";

                worksheet.Cell(7, 17).Value = "ผ่าน";
                worksheet.Cell(7, 18).Value = "ไม่ผ่าน";
                worksheet.Cell(7, 19).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(7, 20).Value = "% ผ่าน";
                worksheet.Cell(7, 21).Value = "Total";

                worksheet.Cell(7, 22).Value = "ผ่าน";
                worksheet.Cell(7, 23).Value = "ไม่ผ่าน";
                worksheet.Cell(7, 24).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(7, 25).Value = "% ผ่าน";
                worksheet.Cell(7, 26).Value = "Total";

                worksheet.Cell(7, 27).Value = "ผ่าน";
                worksheet.Cell(7, 28).Value = "ไม่ผ่าน";
                worksheet.Cell(7, 29).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(7, 30).Value = "% ผ่าน";
                worksheet.Cell(7, 31).Value = "Total";


                // Style header rows (Rows 6 and 7)
                var headerRange = worksheet.Range(6, 1, 7, 31); // Assuming 31 columns
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                headerRange.Style.Border.OutsideBorderColor = XLColor.Green;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

                // Fetch data based on filters
                var reportData = _ReportWorkloadAndInspectionResultsProvider.sp_get_report_workload_inspection_results(
                    new ReportWorkloadAndInspectionResultsModel
                    {
                        act = "ReportWorkloadAndInspectionResults",
                        project_id = Commons.FormatExtension.NullToString(projectId),
                        vender_id = Commons.FormatExtension.NullToString(companyVendorId),
                        start_date = year
                    }
                );

                if (reportData == null || reportData.Count == 0)
                {
                    // Handle the case with no data
                    worksheet.Cell(8, 1).Value = "ไม่มีข้อมูล";
                    worksheet.Range("A3:AE3").Merge(); // Adjusted for 31 columns (A to AE)
                    worksheet.Cell(8, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(8, 1).Style.Font.Bold = true;
                }
                else
                {
                    // Populate data rows
                    int row = 8; // Starting row for data
                    foreach (var item in reportData)
                    {
                        worksheet.Cell(row, 1).Value = item.MonthName;        // Month Name
                        worksheet.Cell(row, 2).Value = item.QC1Pass;          // QC1 Passed
                        worksheet.Cell(row, 3).Value = item.QC1NotPass;       // QC1 Failed
                        worksheet.Cell(row, 4).Value = item.QC1NotReady;      // QC1 Not Ready
                        worksheet.Cell(row, 5).Value = item.QC1PercentPass;   // QC1 Percent Passed
                        worksheet.Cell(row, 6).Value = item.TotalQC1;         // QC1 Total

                        worksheet.Cell(row, 7).Value = item.QC2Pass;          // QC2 Passed
                        worksheet.Cell(row, 8).Value = item.QC2NotPass;       // QC2 Failed
                        worksheet.Cell(row, 9).Value = item.QC2NotReady;      // QC2 Not Ready
                        worksheet.Cell(row, 10).Value = item.QC2PercentPass;  // QC2 Percent Passed
                        worksheet.Cell(row, 11).Value = item.TotalQC2;        // QC2 Total

                        worksheet.Cell(row, 12).Value = item.QC3Pass;         // QC3 Passed
                        worksheet.Cell(row, 13).Value = item.QC3NotPass;      // QC3 Failed
                        worksheet.Cell(row, 14).Value = item.QC3NotReady;     // QC3 Not Ready
                        worksheet.Cell(row, 15).Value = item.QC3PercentPass;  // QC3 Percent Passed
                        worksheet.Cell(row, 16).Value = item.TotalQC3;        // QC3 Total

                        worksheet.Cell(row, 17).Value = item.QC4Pass;         // QC4 Passed
                        worksheet.Cell(row, 18).Value = item.QC4NotPass;      // QC4 Failed
                        worksheet.Cell(row, 19).Value = item.QC4NotReady;     // QC4 Not Ready
                        worksheet.Cell(row, 20).Value = item.QC4PercentPass;  // QC4 Percent Passed
                        worksheet.Cell(row, 21).Value = item.TotalQC4;        // QC4 Total

                        worksheet.Cell(row, 22).Value = item.QC5Pass;         // QC5 Passed
                        worksheet.Cell(row, 23).Value = item.QC5NotPass;      // QC5 Failed
                        worksheet.Cell(row, 24).Value = item.QC5NotReady;     // QC5 Not Ready
                        worksheet.Cell(row, 25).Value = item.QC5PercentPass;  // QC5 Percent Passed
                        worksheet.Cell(row, 26).Value = item.TotalQC5;        // QC5 Total

                        worksheet.Cell(row, 27).Value = item.QCALLPass;       // QC1-QC5 Passed
                        worksheet.Cell(row, 28).Value = item.QCALLNotPass;    // QC1-QC5 Failed
                        worksheet.Cell(row, 29).Value = item.QCALLNotReady;   // QC1-QC5 Not Ready
                        worksheet.Cell(row, 30).Value = item.QCALLPercentPass;// QC1-QC5 Percent Passed
                        worksheet.Cell(row, 31).Value = item.TotalQCALL;      // QC1-QC5 Total


                        // Style the first column to align left while vertically centered
                        var firstColumnCell = worksheet.Cell(row, 1);
                        firstColumnCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        firstColumnCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        firstColumnCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        // Style the rest of the row to align center
                        var detailRange = worksheet.Range(row, 2, row, 31); // From column 2 to 25
                        detailRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        detailRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        //detailRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        var BorderQC1 = worksheet.Range(row, 2, row, 6);
                        BorderQC1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        var BorderQC2 = worksheet.Range(row, 7, row, 11);
                        BorderQC2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        var BorderQC3 = worksheet.Range(row, 12, row, 16);
                        BorderQC3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        var BorderQC4 = worksheet.Range(row, 17, row, 21);
                        BorderQC4.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        var BorderQC5 = worksheet.Range(row, 22, row, 26);
                        BorderQC5.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        var BorderQCALL = worksheet.Range(row, 27, row, 31);
                        BorderQCALL.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row++;
                    }
                }

                // Style rows for better readability
                worksheet.Columns().AdjustToContents(); // Adjust column widths to fit content

                // Return the file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"รายงานปริมาณงานและผลการตรวจ.xlsx"
                    );
                }
            }
        }


    }
}
