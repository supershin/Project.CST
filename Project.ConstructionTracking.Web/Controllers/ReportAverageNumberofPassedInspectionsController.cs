using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReportAverageNumberofPassedInspectionsController : BaseController
    {
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ReportAverageNumberofPassedInspectionsProvider;
        private readonly IGetDDLService _getDDLService;

        public ReportAverageNumberofPassedInspectionsController(IHostEnvironment hosting, MasterManagementProviderProject ReportWorkloadAndInspectionResultsProvider, IGetDDLService getDDLService)
        {
            _hosting = hosting;
            _ReportAverageNumberofPassedInspectionsProvider = ReportWorkloadAndInspectionResultsProvider;
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

            string projectId = Request.Cookies.ContainsKey("ReportAverageNumberofPassedInspectionsselectedProjectId") ? Request.Cookies["ReportAverageNumberofPassedInspectionsselectedProjectId"] : null;

            List<ReportAverageNumberofPassedInspectionsModel> ReportAverageNumberofPassedInspections = new List<ReportAverageNumberofPassedInspectionsModel>();

            var en = new ReportAverageNumberofPassedInspectionsModel
            {
                act = "ReportAverageNumberofPassedInspections",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = "",
                start_date = "",
                end_date = ""
            };

            ReportAverageNumberofPassedInspections = _ReportAverageNumberofPassedInspectionsProvider.sp_get_report_average_numberof_passed_inspections(en);
            ViewBag.SelectedProjectId = projectId;

            return View(ReportAverageNumberofPassedInspections);
        }

        [HttpGet]
        public IActionResult GetDDLVenderByProject(Guid ProjectId)
        {
            var ddlModel = new GetDDL { Act = "GetListDDLCompanyVenderInProject", GuID = ProjectId };
            List<GetDDL> ListDDLCompanyVenderInProject = _getDDLService.GetDDLList(ddlModel);
            return Json(ListDDLCompanyVenderInProject);
        }

        [HttpPost]
        public IActionResult OnclickSearch(string projectId, string CompanyvenderID, string startdate, string enddate)
        {

            // Save the projectId in a cookie
            Response.Cookies.Append("ReportAverageNumberofPassedInspectionsselectedProjectId", projectId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7) // Set cookie to expire in 7 days
            });

            // Prepare date filtering values by parsing dates from dd/mm/yyyy to DateTime
            string? parsedStartDate = Commons.FormatExtension.ToDateString(startdate);
            string? parsedEndDate = Commons.FormatExtension.ToDateString(enddate);

            List<ReportAverageNumberofPassedInspectionsModel> ReportAverageNumberofPassedInspections = new List<ReportAverageNumberofPassedInspectionsModel>();

            var en = new ReportAverageNumberofPassedInspectionsModel
            {
                act = "ReportAverageNumberofPassedInspections",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = Commons.FormatExtension.NullToString(CompanyvenderID),
                start_date = parsedStartDate,
                end_date = parsedEndDate
            };

            List<ReportAverageNumberofPassedInspectionsModel> Datatable = _ReportAverageNumberofPassedInspectionsProvider.sp_get_report_average_numberof_passed_inspections(en);

            return PartialView("PartialTable", Datatable);
        }

        [HttpGet]
        public IActionResult ExportToExcel(Guid projectId, string companyVendorId, string startdate, string enddate)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ค่าเฉลี่ยจำนวนครั้งที่ผ่าน");

                DateTime Datenow = DateTime.Now;

                var ddlModel = new GetDDL { Act = "GetListDDLCompanyVenderInProject", GuID = projectId, ID = Commons.FormatExtension.Nulltoint(companyVendorId) };
                List<GetDDL> CompanyVenderName = _getDDLService.GetDDLList(ddlModel);

                var ddlModel2 = new GetDDL { Act = "ProjectAdmin", GuID = projectId };
                List<GetDDL> ProjectName = _getDDLService.GetDDLList(ddlModel2);

                // Add filter values
                worksheet.Cell(1, 1).Value = "ตัวเลือกการค้นหา";
                worksheet.Cell(2, 1).Value = "โครงการ :";
                worksheet.Cell(2, 2).Value = ProjectName != null && ProjectName.Count > 0 ? ProjectName[0].Text : "ไม่พบชื่อโครงการนี้";
                worksheet.Cell(3, 1).Value = "ผู้รับเหมา :";
                worksheet.Cell(3, 2).Value = companyVendorId != null ? (CompanyVenderName != null && CompanyVenderName.Count > 0 ? CompanyVenderName[0].Text : "ไม่พบชื่อผู้รับเหมา") : "-- ทั้งหมด --";
                worksheet.Cell(4, 1).Value = "วันที่ค้นหา :";
                worksheet.Cell(4, 2).Value = startdate + " ถึง " + enddate;
                worksheet.Cell(5, 1).Value = "Exprort วันที่ :";
                worksheet.Cell(5, 2).Value = Commons.FormatExtension.FormatDateToDayMonthNameYearTime(Datenow); ;
                worksheet.Range(1, 1, 1, 3).Merge();
                worksheet.Range(2, 2, 2, 3).Merge();
                worksheet.Range(3, 2, 3, 3).Merge();
                worksheet.Range(4, 2, 4, 3).Merge();
                worksheet.Range(5, 2, 5, 3).Merge();

                // Row 1: Main headers
                worksheet.Cell(6, 1).Value = "ลำดับ";
                worksheet.Cell(6, 2).Value = "โครงการ";
                worksheet.Cell(6, 3).Value = "ผู้รับเหมา";
                worksheet.Cell(6, 4).Value = "ค่าเฉลี่ยจำนวนครั้งที่ผ่าน";
                worksheet.Cell(6, 9).Value = "จำนวนหลังที่ตรวจผ่าน";

                // Merge main headers
                worksheet.Range(6, 1, 6, 1).Merge();
                worksheet.Range(6, 2, 6, 2).Merge();
                worksheet.Range(6, 3, 6, 3).Merge();
                worksheet.Range(6, 4, 6, 8).Merge();
                worksheet.Range(6, 9, 6, 13).Merge();

                // Row 2: Sub-headers
                worksheet.Cell(7, 4).Value = "QC1";
                worksheet.Cell(7, 5).Value = "QC2";
                worksheet.Cell(7, 6).Value = "QC3";
                worksheet.Cell(7, 7).Value = "QC4";
                worksheet.Cell(7, 8).Value = "QC5";

                worksheet.Cell(7, 9).Value = "QC1";
                worksheet.Cell(7, 10).Value = "QC2";
                worksheet.Cell(7, 11).Value = "QC3";
                worksheet.Cell(7, 12).Value = "QC4";
                worksheet.Cell(7, 13).Value = "QC5";


                //Style header rows (Rows 6 and 7)
                var headerRange = worksheet.Range(6, 1, 7, 13); // Assuming 13 columns
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                var headerRangeColor = worksheet.Range(6, 1, 7, 8); // Assuming 13 columns
                headerRangeColor.Style.Border.OutsideBorderColor = XLColor.Green;
                headerRangeColor.Style.Fill.BackgroundColor = XLColor.LightGreen;

                var headerRangeColor2 = worksheet.Range(6, 9, 7, 13); // Assuming 13 columns
                headerRangeColor2.Style.Border.OutsideBorderColor = XLColor.Green;
                headerRangeColor2.Style.Fill.BackgroundColor = XLColor.LightYellow;

                // Fetch data based on filters
                string? parsedStartDate = Commons.FormatExtension.ToDateString(startdate);
                string? parsedEndDate = Commons.FormatExtension.ToDateString(enddate);

                List<ReportAverageNumberofPassedInspectionsModel> ReportAverageNumberofPassedInspections = new List<ReportAverageNumberofPassedInspectionsModel>();

                var en = new ReportAverageNumberofPassedInspectionsModel
                {
                    act = "ReportAverageNumberofPassedInspections",
                    project_id = Commons.FormatExtension.NullToString(projectId),
                    unit_id = "",
                    unit_status = "",
                    build_status = "",
                    vender_id = Commons.FormatExtension.NullToString(companyVendorId),
                    start_date = parsedStartDate,
                    end_date = parsedEndDate
                };

                List<ReportAverageNumberofPassedInspectionsModel> reportData = _ReportAverageNumberofPassedInspectionsProvider.sp_get_report_average_numberof_passed_inspections(en);

                if (reportData == null || reportData.Count == 0)
                {
                    // Handle the case with no data
                    worksheet.Cell(8, 1).Value = "ไม่มีข้อมูล";
                    worksheet.Range("A8:M8").Merge(); // Adjusted for 13 columns
                    worksheet.Cell(8, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(8, 1).Style.Font.Bold = true;
                }
                else
                {
                    // Populate data rows
                    int row = 8; // Starting row for data
                    double[] totals = new double[10]; // To store totals for numeric columns
                    foreach (var item in reportData)
                    {
                        worksheet.Cell(row, 1).Value = item.index;
                        worksheet.Cell(row, 2).Value = item.ProjectName;
                        worksheet.Cell(row, 3).Value = item.CompanyVendorName;

                        // Convert string values to double and handle 0 as empty string
                        double.TryParse(item.AverageQC1Pass, out double averageQC1Pass);
                        double.TryParse(item.AverageQC2Pass, out double averageQC2Pass);
                        double.TryParse(item.AverageQC3Pass, out double averageQC3Pass);
                        double.TryParse(item.AverageQC4Pass, out double averageQC4Pass);
                        double.TryParse(item.AverageQC5Pass, out double averageQC5Pass);
                        double.TryParse(item.CntUnitQC1Pass, out double cntUnitQC1Pass);
                        double.TryParse(item.CntUnitQC2Pass, out double cntUnitQC2Pass);
                        double.TryParse(item.CntUnitQC3Pass, out double cntUnitQC3Pass);
                        double.TryParse(item.CntUnitQC4Pass, out double cntUnitQC4Pass);
                        double.TryParse(item.CntUnitQC5Pass, out double cntUnitQC5Pass);

                        worksheet.Cell(row, 4).Value = averageQC1Pass == 0 ? "" : averageQC1Pass;
                        worksheet.Cell(row, 5).Value = averageQC2Pass == 0 ? "" : averageQC2Pass;
                        worksheet.Cell(row, 6).Value = averageQC3Pass == 0 ? "" : averageQC3Pass;
                        worksheet.Cell(row, 7).Value = averageQC4Pass == 0 ? "" : averageQC4Pass;
                        worksheet.Cell(row, 8).Value = averageQC5Pass == 0 ? "" : averageQC5Pass;

                        worksheet.Cell(row, 9).Value = cntUnitQC1Pass == 0 ? "" : cntUnitQC1Pass;
                        worksheet.Cell(row, 10).Value = cntUnitQC2Pass == 0 ? "" : cntUnitQC2Pass;
                        worksheet.Cell(row, 11).Value = cntUnitQC3Pass == 0 ? "" : cntUnitQC3Pass;
                        worksheet.Cell(row, 12).Value = cntUnitQC4Pass == 0 ? "" : cntUnitQC4Pass;
                        worksheet.Cell(row, 13).Value = cntUnitQC5Pass == 0 ? "" : cntUnitQC5Pass;

                        // Update totals
                        totals[0] += averageQC1Pass;
                        totals[1] += averageQC2Pass;
                        totals[2] += averageQC3Pass;
                        totals[3] += averageQC4Pass;
                        totals[4] += averageQC5Pass;
                        totals[5] += cntUnitQC1Pass;
                        totals[6] += cntUnitQC2Pass;
                        totals[7] += cntUnitQC3Pass;
                        totals[8] += cntUnitQC4Pass;
                        totals[9] += cntUnitQC5Pass;

                        // Style the row
                        worksheet.Range(row, 1, row, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Range(row, 1, row, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row++;
                    }



                    // Add total row
                    worksheet.Cell(row, 1).Value = "รวมทั้งหมด";
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Populate totals
                    for (int i = 0; i < totals.Length; i++)
                    {
                        var totalValue = totals[i];
                        worksheet.Cell(row, 4 + i).Value = totalValue % 1 == 0 ? (totalValue == 0 ? "" : totalValue.ToString("0")) : totalValue.ToString("0.00");
                        worksheet.Cell(row, 4 + i).Style.Font.Bold = true;
                        worksheet.Cell(row, 4 + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    // Style total row
                    worksheet.Range(row, 1, row, 13).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    worksheet.Range(row, 1, row, 13).Style.Fill.BackgroundColor = XLColor.LightGray;
                }

                // Adjust columns to fit the content
                worksheet.Columns().AdjustToContents();


                // Return the file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"รายงานค่าเฉลี่ยจำนวนครั้งที่ตรวจผ่าน.xlsx"
                    );
                }
            }
        }

    }
}
