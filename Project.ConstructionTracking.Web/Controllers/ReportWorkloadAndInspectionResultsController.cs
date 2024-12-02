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

            string currentYear = DateTime.Now.Year.ToString();

            List<ReportWorkloadAndInspectionResultsModel> ReportWorkloadAndInspectionResultslists = new List<ReportWorkloadAndInspectionResultsModel>();

            var en = new ReportWorkloadAndInspectionResultsModel
            {
                act = "ReportWorkloadAndInspectionResults",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = "",
                start_date = currentYear
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
            // Convert year from B.E. to A.D. if needed
            if (!string.IsNullOrEmpty(year))
            {
                if (int.TryParse(year, out int parsedYear) && parsedYear > 2400) // Assuming B.E. year > 2400
                {
                    year = (parsedYear - 543).ToString(); // Convert to A.D. year
                }
            }

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
        public IActionResult ExportToExcel(string projectId, string companyVendorId, string year)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Inspection Results Report");

                // Create header rows
                // Row 1: Main headers with merged cells
                worksheet.Cell(1, 1).Value = "เดือน";
                worksheet.Cell(1, 2).Value = "QC1";
                worksheet.Cell(1, 6).Value = "QC2";
                worksheet.Cell(1, 10).Value = "QC3";
                worksheet.Cell(1, 14).Value = "QC4";
                worksheet.Cell(1, 18).Value = "QC5";
                worksheet.Cell(1, 22).Value = "QC1-QC5";

                // Merge main headers
                worksheet.Range(1, 2, 1, 5).Merge();
                worksheet.Range(1, 6, 1, 9).Merge();
                worksheet.Range(1, 10, 1, 13).Merge();
                worksheet.Range(1, 14, 1, 17).Merge();
                worksheet.Range(1, 18, 1, 21).Merge();
                worksheet.Range(1, 22, 1, 25).Merge();

                // Row 2: Sub-headers
                worksheet.Cell(2, 2).Value = "ผ่าน";
                worksheet.Cell(2, 3).Value = "ไม่ผ่าน";
                worksheet.Cell(2, 4).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(2, 5).Value = "% ผ่าน";
                worksheet.Cell(2, 6).Value = "ผ่าน";
                worksheet.Cell(2, 7).Value = "ไม่ผ่าน";
                worksheet.Cell(2, 8).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(2, 9).Value = "% ผ่าน";
                worksheet.Cell(2, 10).Value = "ผ่าน";
                worksheet.Cell(2, 11).Value = "ไม่ผ่าน";
                worksheet.Cell(2, 12).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(2, 13).Value = "% ผ่าน";
                worksheet.Cell(2, 14).Value = "ผ่าน";
                worksheet.Cell(2, 15).Value = "ไม่ผ่าน";
                worksheet.Cell(2, 16).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(2, 17).Value = "% ผ่าน";
                worksheet.Cell(2, 18).Value = "ผ่าน";
                worksheet.Cell(2, 19).Value = "ไม่ผ่าน";
                worksheet.Cell(2, 20).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(2, 21).Value = "% ผ่าน";
                worksheet.Cell(2, 22).Value = "ผ่าน";
                worksheet.Cell(2, 23).Value = "ไม่ผ่าน";
                worksheet.Cell(2, 24).Value = "ไม่พร้อมตรวจ";
                worksheet.Cell(2, 25).Value = "% ผ่าน";

                // Style the header rows
                worksheet.Range("A1:Y2").Style.Font.Bold = true;
                worksheet.Range("A1:Y2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:Y2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Fetch data based on filters
                var reportData = _ReportWorkloadAndInspectionResultsProvider.sp_get_report_workload_inspection_results(
                    new ReportWorkloadAndInspectionResultsModel
                    {
                        act = "ReportWorkloadAndInspectionResults",
                        project_id = projectId,
                        vender_id = Commons.FormatExtension.NullToString(companyVendorId),
                        start_date = year
                    }
                );

                if (reportData == null || reportData.Count == 0)
                {
                    worksheet.Cell(3, 1).Value = "ไม่มีข้อมูล";
                    worksheet.Range("A3:Y3").Merge();
                    worksheet.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(3, 1).Style.Font.Bold = true;
                }
                else
                {
                    // Populate data rows
                    int row = 3;
                    foreach (var item in reportData)
                    {
                        worksheet.Cell(row, 1).Value = item.MonthName;
                        worksheet.Cell(row, 2).Value = item.QC1Pass;
                        worksheet.Cell(row, 3).Value = item.QC1NotPass;
                        worksheet.Cell(row, 4).Value = item.QC1NotReady;
                        worksheet.Cell(row, 5).Value = item.QC1PercentPass;
                        worksheet.Cell(row, 6).Value = item.QC2Pass;
                        worksheet.Cell(row, 7).Value = item.QC2NotPass;
                        worksheet.Cell(row, 8).Value = item.QC2NotReady;
                        worksheet.Cell(row, 9).Value = item.QC2PercentPass;
                        worksheet.Cell(row, 10).Value = item.QC3Pass;
                        worksheet.Cell(row, 11).Value = item.QC3NotPass;
                        worksheet.Cell(row, 12).Value = item.QC3NotReady;
                        worksheet.Cell(row, 13).Value = item.QC3PercentPass;
                        worksheet.Cell(row, 14).Value = item.QC4Pass;
                        worksheet.Cell(row, 15).Value = item.QC4NotPass;
                        worksheet.Cell(row, 16).Value = item.QC4NotReady;
                        worksheet.Cell(row, 17).Value = item.QC4PercentPass;
                        worksheet.Cell(row, 18).Value = item.QC5Pass;
                        worksheet.Cell(row, 19).Value = item.QC5NotPass;
                        worksheet.Cell(row, 20).Value = item.QC5NotReady;
                        worksheet.Cell(row, 21).Value = item.QC5PercentPass;
                        worksheet.Cell(row, 22).Value = item.QCALLPass;
                        worksheet.Cell(row, 23).Value = item.QCALLNotPass;
                        worksheet.Cell(row, 24).Value = item.QCALLNotReady;
                        worksheet.Cell(row, 25).Value = item.QCALLPercentPass;

                        row++;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"InspectionResults_{year}.xlsx"
                    );
                }
            }
        }


    }
}
