using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReportinspectionQC5DefectController : BaseController
    {
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ReportinspectionQC5DefectProvider;
        private readonly IGetDDLService _getDDLService;

        public ReportinspectionQC5DefectController(IHostEnvironment hosting, MasterManagementProviderProject ReportinspectionQC5DefectProvider, IGetDDLService getDDLService)
        {
            _hosting = hosting;
            _ReportinspectionQC5DefectProvider = ReportinspectionQC5DefectProvider;
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

            string projectId = Request.Cookies.ContainsKey("ReportinspectionQC5DefectselectedProjectId") ? Request.Cookies["ReportinspectionQC5DefectselectedProjectId"] : null;

            List<ReportinspectionQC5DefectModel> ReportinspectionQC5Defect = new List<ReportinspectionQC5DefectModel>();

            var en = new ReportinspectionQC5DefectModel
            {
                act = "ReportinspectionQC5Defect",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = "",
                start_date = "",
                end_date = ""
            };

            ReportinspectionQC5Defect = _ReportinspectionQC5DefectProvider.sp_get_report_inspection_QC5_Defect(en);
            ViewBag.SelectedProjectId = projectId;
            return View(ReportinspectionQC5Defect);
        }

        public IActionResult GetDDLVenderByProject(Guid ProjectId)
        {
            var ddlModel = new GetDDL { Act = "GetListDDLCompanyVenderInProject", GuID = ProjectId };
            List<GetDDL> ListDDLCompanyVenderInProject = _getDDLService.GetDDLList(ddlModel);
            return Json(ListDDLCompanyVenderInProject);
        }

        [HttpPost]
        public IActionResult OnclickSearch(string projectId, string CompanyvenderID, string UnitSearch , string startdate, string enddate)
        {

            // Save the projectId in a cookie
            Response.Cookies.Append("ReportinspectionQC5DefectselectedProjectId", projectId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7) // Set cookie to expire in 7 days
            });

            // Prepare date filtering values by parsing dates from dd/mm/yyyy to DateTime
            string? parsedStartDate = Commons.FormatExtension.ToDateString(startdate);
            string? parsedEndDate = Commons.FormatExtension.ToDateString(enddate);


            var en = new ReportinspectionQC5DefectModel
            {
                act = "ReportinspectionQC5Defect",
                project_id = projectId,
                unit_id = Commons.FormatExtension.NullToString(UnitSearch),
                unit_status = "",
                build_status = "",
                vender_id = Commons.FormatExtension.NullToString(CompanyvenderID),
                start_date = parsedStartDate,
                end_date = parsedEndDate
            };

            List<ReportinspectionQC5DefectModel> Datatable = _ReportinspectionQC5DefectProvider.sp_get_report_inspection_QC5_Defect(en);

            return PartialView("PartialTable", Datatable);
        }

        [HttpGet]
        public IActionResult ExportToExcel(Guid projectId, string companyVendorId, string UnitSearch , string startdate, string enddate)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("รายงานรายละเอียด defect QC5");

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
                worksheet.Cell(4, 1).Value = "พิมพ์ค้นหาแปลง... :";
                worksheet.Cell(4, 2).Value = Commons.FormatExtension.NullToString(UnitSearch);
                worksheet.Cell(5, 1).Value = "วันที่ค้นหา :";
                worksheet.Cell(5, 2).Value = startdate + " ถึง " + enddate;
                worksheet.Cell(6, 1).Value = "Exprort วันที่ :";
                worksheet.Cell(6, 2).Value = Commons.FormatExtension.FormatDateToDayMonthNameYearTime(Datenow); ;
                worksheet.Range(1, 1, 1, 3).Merge();
                worksheet.Range(2, 2, 2, 3).Merge();
                worksheet.Range(3, 2, 3, 3).Merge();
                worksheet.Range(4, 2, 4, 3).Merge();
                worksheet.Range(5, 2, 5, 3).Merge();
                worksheet.Range(6, 2, 6, 3).Merge();

                // Row 1: Main headers
                worksheet.Cell(7, 1).Value = "ลำดับ";
                worksheet.Cell(7, 2).Value = "โครงการ";
                worksheet.Cell(7, 3).Value = "แปลงที่";
                worksheet.Cell(7, 4).Value = "ผู้รับเหมา";
                worksheet.Cell(7, 5).Value = "วันที่ QC ให้รายการ";
                worksheet.Cell(7, 6).Value = "Area";
                worksheet.Cell(7, 7).Value = "หมวด";
                worksheet.Cell(7, 8).Value = "รายการ defect";
                worksheet.Cell(7, 9).Value = "Major defect";
                worksheet.Cell(7, 10).Value = "สถานะปัจจุบัน";
                worksheet.Cell(7, 11).Value = "จำนวนครั้งที่ตรวจผ่าน";
                worksheet.Cell(7, 12).Value = "QC ผู้ตรวจ";
                worksheet.Cell(7, 13).Value = "ความเห็นเพิ่มเติม";

                // Define the header range properly
                var headerRange = worksheet.Range(7, 1, 7, 13); // Corrected to include all columns in the range

                // Style the header row
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray; // Optional: Add a background color for clarity

                // Fetch data based on filters
                string? parsedStartDate = Commons.FormatExtension.ToDateString(startdate);
                string? parsedEndDate = Commons.FormatExtension.ToDateString(enddate);


                var en = new ReportinspectionQC5DefectModel
                {
                    act = "ReportinspectionQC5Defect",
                    project_id = Commons.FormatExtension.NullToString(projectId),
                    unit_id = Commons.FormatExtension.NullToString(UnitSearch),
                    unit_status = "",
                    build_status = "",
                    vender_id = Commons.FormatExtension.NullToString(companyVendorId),
                    start_date = parsedStartDate,
                    end_date = parsedEndDate
                };

                List<ReportinspectionQC5DefectModel> reportData = _ReportinspectionQC5DefectProvider.sp_get_report_inspection_QC5_Defect(en);

                if (reportData == null || reportData.Count == 0)
                {
                    // Handle the case with no data
                    worksheet.Cell(8, 1).Value = "ไม่มีข้อมูล";
                    worksheet.Range("A7:M7").Merge(); // Adjusted for 13 columns
                    worksheet.Cell(8, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(8, 1).Style.Font.Bold = true;
                }
                else
                {
                    // Populate data rows
                    int row = 8; // Starting row for data
                    foreach (var item in reportData)
                    {
                        worksheet.Cell(row, 1).Value = item.index;
                        worksheet.Cell(row, 2).Value = item.ProjectName;
                        worksheet.Cell(row, 3).Value = item.UnitCode;
                        worksheet.Cell(row, 4).Value = item.CompanyVendorName;
                        worksheet.Cell(row, 5).Value = item.ActionDate;
                        worksheet.Cell(row, 6).Value = item.DefectAreaName;
                        worksheet.Cell(row, 7).Value = item.DefectTypeName;
                        worksheet.Cell(row, 8).Value = item.DefectDescription;
                        worksheet.Cell(row, 9).Value = item.IsMajorDefect;
                        worksheet.Cell(row, 10).Value = item.StatusPresent;
                        worksheet.Cell(row, 11).Value = item.SeqPass;
                        worksheet.Cell(row, 12).Value = item.QCUser;
                        worksheet.Cell(row, 13).Value = item.Remark;
                        // Style the row
                        worksheet.Range(row, 1, row, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Range(row, 1, row, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

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
                        $"รายงานรายละเอียด_defect_QC5.xlsx"
                    );
                }
            }
        }

    }
}
