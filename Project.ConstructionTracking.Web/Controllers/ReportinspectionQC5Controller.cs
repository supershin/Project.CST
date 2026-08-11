using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;
using ClosedXML.Excel;
using ClosedXML.Parser;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReportinspectionQC5Controller : BaseController
    {
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ReportinspectionQC5Provider;
        private readonly IGetDDLService _getDDLService;

        public ReportinspectionQC5Controller(IHostEnvironment hosting, MasterManagementProviderProject ReportinspectionQC5Provider, IGetDDLService getDDLService)
        {
            _hosting = hosting;
            _ReportinspectionQC5Provider = ReportinspectionQC5Provider;
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

            string? projectId = Request.Cookies.ContainsKey("ReportinspectionQC5selectedProjectId")
                ? Request.Cookies["ReportinspectionQC5selectedProjectId"]
                : null;

            // Default to the first available project so the initial report and dropdown
            // use the same project when there is no valid saved selection.
            bool hasValidSelectedProject = !string.IsNullOrWhiteSpace(projectId)
                && ListProject.Any(project => string.Equals(
                    project.ValueGuid?.ToString(),
                    projectId,
                    StringComparison.OrdinalIgnoreCase));

            if (!hasValidSelectedProject)
            {
                projectId = ListProject.FirstOrDefault(project => project.ValueGuid.HasValue)
                    ?.ValueGuid
                    ?.ToString();
            }

            List<ReportinspectionQC5Model> ReportinspectionQC5 = new List<ReportinspectionQC5Model>();

            var en = new ReportinspectionQC5Model
            {
                act = "ReportinspectionQC5",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = "",
                start_date = "",
                end_date = ""
            };

            ReportinspectionQC5 = _ReportinspectionQC5Provider.sp_get_report_inspection_QC5(en);
            ViewBag.SelectedProjectId = projectId;
            return View(ReportinspectionQC5);
        }

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
            Response.Cookies.Append("ReportinspectionQC5selectedProjectId", projectId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7) // Set cookie to expire in 7 days
            });

            // Prepare date filtering values by parsing dates from dd/mm/yyyy to DateTime
            string? parsedStartDate = Commons.FormatExtension.ToDateString(startdate);
            string? parsedEndDate = Commons.FormatExtension.ToDateString(enddate);

            List<ReportinspectionQC5Model> ReportinspectionQC5 = new List<ReportinspectionQC5Model>();

            var en = new ReportinspectionQC5Model
            {
                act = "ReportinspectionQC5",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = Commons.FormatExtension.NullToString(CompanyvenderID),
                start_date = parsedStartDate,
                end_date = parsedEndDate
            };

            List<ReportinspectionQC5Model> Datatable = _ReportinspectionQC5Provider.sp_get_report_inspection_QC5(en);

            return PartialView("PartialTable", Datatable);
        }

        [HttpGet]
        public IActionResult ExportToExcel(Guid projectId, string companyVendorId, string startdate, string enddate)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("รายงานการตรวจ QC5");

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
                worksheet.Cell(6, 3).Value = "Unit No.";
                worksheet.Cell(6, 4).Value = "Type บ้าน";
                worksheet.Cell(6, 5).Value = "ผู้รับเหมา";
                worksheet.Cell(6, 6).Value = "จำนวนครั้งที่ตรวจถึงปัจจุบัน";
                worksheet.Cell(6, 7).Value = "วันที่ตรวจ QC5 ครั้งแรก";
                worksheet.Cell(6, 8).Value = "วันที่ QC5 ผ่าน";
                worksheet.Cell(6, 9).Value = "วันที่ QC5 (CRM)";
                worksheet.Cell(6, 10).Value = "จำนวนรายการที่เป็น major";
                worksheet.Cell(6, 11).Value = "จำนวนรายการ Defect ทั้งหมด";

                // Define the header range properly
                var headerRange = worksheet.Range(6, 1, 6, 11);

                // Style the header row
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray; // Optional: Add a background color for clarity

                // Fetch data based on filters
                string? parsedStartDate = Commons.FormatExtension.ToDateString(startdate);
                string? parsedEndDate = Commons.FormatExtension.ToDateString(enddate);

                List<ReportinspectionQC5Model> ReportAverageNumberofPassedInspections = new List<ReportinspectionQC5Model>();

                var en = new ReportinspectionQC5Model
                {
                    act = "ReportinspectionQC5",
                    project_id = Commons.FormatExtension.NullToString(projectId),
                    unit_id = "",
                    unit_status = "",
                    build_status = "",
                    vender_id = Commons.FormatExtension.NullToString(companyVendorId),
                    start_date = parsedStartDate,
                    end_date = parsedEndDate
                };

                List<ReportinspectionQC5Model> reportData = _ReportinspectionQC5Provider.sp_get_report_inspection_QC5(en);

                if (reportData == null || reportData.Count == 0)
                {
                    // Handle the case with no data
                    worksheet.Cell(7, 1).Value = "ไม่มีข้อมูล";
                    worksheet.Range("A7:K7").Merge();
                    worksheet.Cell(7, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(7, 1).Style.Font.Bold = true;
                }
                else
                {
                    // Populate data rows
                    int row = 7; // Starting row for data
                    foreach (var item in reportData)
                    {
                        worksheet.Cell(row, 1).Value = item.index;
                        worksheet.Cell(row, 2).Value = item.ProjectName;
                        worksheet.Cell(row, 3).Value = item.UnitCode;
                        worksheet.Cell(row, 4).Value = item.ModelTypeName;
                        worksheet.Cell(row, 5).Value = item.CompanyVendorName;
                        worksheet.Cell(row, 6).Value = item.MAXSeq;
                        worksheet.Cell(row, 7).Value = item.FirstDateCheck;
                        worksheet.Cell(row, 8).Value = item.DatePass;
                        worksheet.Cell(row, 9).Value = item.SyncCrmDate;
                        worksheet.Cell(row, 10).Value = item.CNTMajorDefect;
                        worksheet.Cell(row, 11).Value = item.CNTDefect;

                        // Style the row
                        worksheet.Range(row, 1, row, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Range(row, 1, row, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row++;
                    }

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
                        $"รายงานการตรวจ_QC5.xlsx"
                    );
                }
            }
        }

    }
}
