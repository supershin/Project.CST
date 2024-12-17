using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Services;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ReporQC14FailController : BaseController
    {
        private readonly IHostEnvironment _hosting;
        private readonly MasterManagementProviderProject _ReportReportQC14FailProvider;
        private readonly IGetDDLService _getDDLService;

        public ReporQC14FailController(IHostEnvironment hosting, MasterManagementProviderProject ReportReportQC14FailProvider, IGetDDLService getDDLService)
        {
            _hosting = hosting;
            _ReportReportQC14FailProvider = ReportReportQC14FailProvider;
            _getDDLService = getDDLService;
        }

        public IActionResult Index()
        {
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

            string projectId = Request.Cookies.ContainsKey("ReporQC14FailselectedProjectId") ? Request.Cookies["ReporQC14FailselectedProjectId"] : null;

            List<ReportQC14FailModel> ReportQC14Fail = new List<ReportQC14FailModel>();

            var en = new ReportQC14FailModel
            {
                act = "ReporQC1-4Fail",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = "",
                qctype_id = "",
                start_date = "",
                end_date = ""
            };

            ReportQC14Fail = _ReportReportQC14FailProvider.sp_get_report_qc1_4fail(en);
            ViewBag.SelectedProjectId = projectId;

            return View(ReportQC14Fail);
        }

        [HttpGet]
        public IActionResult GetDDLVenderByProject(Guid ProjectId)
        {
            var ddlModel = new GetDDL { Act = "GetListDDLCompanyVenderInProject", GuID = ProjectId };
            List<GetDDL> ListDDLCompanyVenderInProject = _getDDLService.GetDDLList(ddlModel);
            return Json(ListDDLCompanyVenderInProject);
        }

        [HttpGet]
        public IActionResult GetDDLQCTypeByProject(Guid ProjectId)
        {
            var ddlModel = new GetDDL { Act = "GetListDDLQCTypeByProject", GuID = ProjectId };
            List<GetDDL> ListDDLQCTypeByProject = _getDDLService.GetDDLList(ddlModel);
            return Json(ListDDLQCTypeByProject);
        }

        [HttpPost]
        public IActionResult OnclickSearch(string projectId, string CompanyvenderID, string qctypeIDs, string startdate, string enddate)
        {

            // Save the projectId in a cookie
            Response.Cookies.Append("ReporQC14FailselectedProjectId", projectId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7) // Set cookie to expire in 7 days
            });

            // Prepare date filtering values by parsing dates from dd/mm/yyyy to DateTime
            string? parsedStartDate = Commons.FormatExtension.ToDateString(startdate);
            string? parsedEndDate = Commons.FormatExtension.ToDateString(enddate);

            List<ReportQC14FailModel> ReportQC14Fail = new List<ReportQC14FailModel>();

            var en = new ReportQC14FailModel
            {
                act = "ReporQC1-4Fail",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = "",
                vender_id = Commons.FormatExtension.NullToString(CompanyvenderID),
                qctype_id = Commons.FormatExtension.NullToString(qctypeIDs),
                start_date = parsedStartDate,
                end_date = parsedEndDate
            };

            List<ReportQC14FailModel> Datatable = _ReportReportQC14FailProvider.sp_get_report_qc1_4fail(en);

            return PartialView("PartialTable", Datatable);
        }

        [HttpGet]
        public IActionResult ExportToExcel(Guid projectId, string companyVendorId, string qctypeIDs, string startdate, string enddate)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("รายการที่ตรวจไม่ผ่านช่วง QC 1-4");
                DateTime Datenow = DateTime.Now;

                // Fetch Project and Vendor Names
                var ddlModel = new GetDDL { Act = "GetListDDLCompanyVenderInProject", GuID = projectId, ID = Commons.FormatExtension.Nulltoint(companyVendorId) };
                List<GetDDL> CompanyVenderName = _getDDLService.GetDDLList(ddlModel);

                var ddlModel2 = new GetDDL { Act = "ProjectAdmin", GuID = projectId };
                List<GetDDL> ProjectName = _getDDLService.GetDDLList(ddlModel2);

                // Add filter values
                worksheet.Cell(1, 1).Value = "ตัวเลือกการค้นหา";
                worksheet.Cell(2, 1).Value = "โครงการ :";
                worksheet.Cell(2, 2).Value = ProjectName?.FirstOrDefault()?.Text ?? "ไม่พบชื่อโครงการนี้";
                worksheet.Cell(3, 1).Value = "ผู้รับเหมา :";
                worksheet.Cell(3, 2).Value = companyVendorId != null
                    ? CompanyVenderName?.FirstOrDefault()?.Text ?? "ไม่พบชื่อผู้รับเหมา"
                    : "-- ทั้งหมด --";
                worksheet.Cell(4, 1).Value = "วันที่ค้นหา :";
                worksheet.Cell(4, 2).Value = $"{startdate} ถึง {enddate}";
                worksheet.Cell(5, 1).Value = "Export วันที่ :";
                worksheet.Cell(5, 2).Value = Datenow.ToString("dd/MM/yyyy HH:mm:ss");
                worksheet.Range(1, 1, 1, 2).Merge();

                // Add table headers
                worksheet.Cell(7, 1).Value = "QC";
                worksheet.Cell(7, 2).Value = "No.";
                worksheet.Cell(7, 3).Value = "รายการตรวจ";
                worksheet.Cell(7, 4).Value = "จำนวนครั้งที่ตรวจทั้งหมด";
                worksheet.Cell(7, 5).Value = "จำนวนครั้งที่่ไม่ผ่าน";
                worksheet.Cell(7, 6).Value = "% สัดส่วนที่ไม่ผ่าน";
                worksheet.Cell(7, 7).Value = "จำนวนหลังที่ตรวจ";

                worksheet.Range(7, 1, 7, 7).Style.Font.Bold = true;
                worksheet.Range(7, 1, 7, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(7, 1, 7, 7).Style.Fill.BackgroundColor = XLColor.LightGray;

                // Fetch data and group by QCTypeID
                List<ReportQC14FailModel> reportData = _ReportReportQC14FailProvider.sp_get_report_qc1_4fail(
                        new ReportQC14FailModel
                        {
                            act = "ReporQC1-4Fail",
                            project_id = Commons.FormatExtension.NullToString(projectId),
                            unit_id = "",
                            unit_status = "",
                            build_status = "",
                            vender_id = Commons.FormatExtension.NullToString(companyVendorId),
                            qctype_id = Commons.FormatExtension.NullToString(qctypeIDs),
                            start_date = Commons.FormatExtension.ToDateString(startdate),
                            end_date = Commons.FormatExtension.ToDateString(enddate)
                        });

                if (reportData == null || !reportData.Any())
                {
                    worksheet.Cell(8, 1).Value = "ไม่มีข้อมูล";
                    worksheet.Range(8, 1, 8, 7).Merge();
                    worksheet.Range(8, 1, 8, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range(8, 1, 8, 7).Style.Font.Bold = true;
                }
                else
                {
                    int row = 8;
                    var groupedData = reportData.GroupBy(x => x.QCTypeID).ToList();

                    foreach (var group in groupedData)
                    {
                        bool isFirstRow = true;
                        int runningIndex = 1;

                        foreach (var item in group)
                        {
                            // Special row styling for ParentID == 0 and other conditions
                            bool isSpecialRow = item.ParentID == 0 &&
                                                string.IsNullOrEmpty(item.AllQC) &&
                                                string.IsNullOrEmpty(item.AllQCFail) &&
                                                string.IsNullOrEmpty(item.CNTUnit) &&
                                                item.QCTypeID != 12 && 
                                                item.QCTypeID != 14;

                            if (isFirstRow)
                            {
                                worksheet.Cell(row, 1).Value = item.QCTypeName;
                                worksheet.Cell(row, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                worksheet.Cell(row, 1).Style.Font.Bold = true;
                                worksheet.Range(row, 1, row + group.Count() - 1, 1).Merge(); // Merge rows for QCTypeName
                                isFirstRow = false;
                            }

                            worksheet.Cell(row, 2).Value = runningIndex;
                            worksheet.Cell(row, 3).Value = item.ChecklistName;
                            worksheet.Cell(row, 4).Value = item.AllQC;
                            worksheet.Cell(row, 5).Value = item.AllQCFail;
                            worksheet.Cell(row, 6).Value = item.QCPercentFail;
                            worksheet.Cell(row, 7).Value = item.CNTUnit;

                            // Apply styles directly based on the condition
                            var detailRange = worksheet.Range(row, 3, row, 7);
                            if (isSpecialRow)
                            {
                                detailRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                                detailRange.Style.Font.Bold = true;
                                detailRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            }
                            else
                            {
                                detailRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                            detailRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                            // Increment counters
                            row++;
                            runningIndex++;
                        }

                    }
                }

                // Adjust column widths
                worksheet.Columns().AdjustToContents();

                // Return the file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "รายการที่ตรวจไม่ผ่านช่วงQC1_4.xlsx"
                    );
                }
            }
        }

    }
}
