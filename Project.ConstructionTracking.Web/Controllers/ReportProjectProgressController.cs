using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.IO;
using System;
using Project.ConstructionTracking.Web.Controllers;
using Microsoft.Extensions.Hosting;
using Project.ConstructionTracking.Web.Library.DAL;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using QuestPDF.Infrastructure;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

public class ReportProjectProgressController : BaseController
{
    private readonly IHostEnvironment _hosting;
    private readonly MasterManagementProviderProject _ProjectProgressReportProvider;
    private readonly IGetDDLService _getDDLService;
    public ReportProjectProgressController(IHostEnvironment hosting , MasterManagementProviderProject ProjectProgressReportProvider, IGetDDLService getDDLService)
    {
        _hosting = hosting;
        _ProjectProgressReportProvider = ProjectProgressReportProvider;
        _getDDLService = getDDLService;
    }

    public IActionResult Index()
    {
        // Retrieve the projectId from the cookie, if it exists
        string projectId = Request.Cookies.ContainsKey("CkselectedProjectId") ? Request.Cookies["CkselectedProjectId"] : null;

        var ddlModel = new GetDDL { Act = "ProjectAdmin" };
        List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
        ViewBag.DDLProject = ListProject;

        // Check if projectId exists before fetching data
        List<ReportProjectProgressModel> ProjectProgressReportlists = new List<ReportProjectProgressModel>();
        if (!string.IsNullOrEmpty(projectId))
        {
            var en = new ReportProjectProgressModel
            {
                act = "ReportProjectProgress",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = ""
            };

            ProjectProgressReportlists = _ProjectProgressReportProvider.sp_get_report_Project_Prcress(en);
        }

        ViewBag.SelectedProjectId = projectId;
        return View(ProjectProgressReportlists);
    }


    [HttpGet]
    public IActionResult ExportToExcel(string projectId)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Project Progress Report");

            // Add headers
            worksheet.Cell(1, 1).Value = "No.";
            worksheet.Cell(1, 2).Value = "โครงการ";
            worksheet.Cell(1, 3).Value = "แปลงที่";
            worksheet.Cell(1, 4).Value = "ผู้รับเหมา";
            worksheet.Cell(1, 5).Value = "วิศวกรผู้ควบคุมงาน";
            worksheet.Cell(1, 6).Value = "สถานะขาย (จอง สัญญา โอน ห้องว่าง)";
            worksheet.Cell(1, 7).Value = "วันกำหนดโอน (ตามสัญญา)";
            worksheet.Cell(1, 8).Value = "แผนเริ่มตามสัญญา";
            worksheet.Cell(1, 9).Value = "แผนสิ้นสุดตามสัญญา";
            worksheet.Cell(1, 10).Value = "สถานะงวดงานที่ผ่านล่าสุด";
            worksheet.Cell(1, 11).Value = "% Progress ในแผน";
            worksheet.Cell(1, 12).Value = "% Progress จริง";
            worksheet.Cell(1, 13).Value = "% Delay/ahead";
            worksheet.Cell(1, 14).Value = "งวดที่เบิกล่าสุด";

            // Data retrieval based on the selected projectId
            var en = new ReportProjectProgressModel
            {
                act = "ReportProjectProgress",
                project_id = projectId,
                unit_id = "",
                unit_status = "",
                build_status = ""
            };

            List<ReportProjectProgressModel> ProjectProgressReportlists = _ProjectProgressReportProvider.sp_get_report_Project_Prcress(en);

            if (ProjectProgressReportlists == null || ProjectProgressReportlists.Count == 0)
            {
                // If no data, display "ไม่มีข้อมูล" in the first cell
                worksheet.Cell(2, 1).Value = "ไม่มีข้อมูล";
                worksheet.Range("A2:N2").Merge(); // Merge cells for a more centered message
                worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(2, 1).Style.Font.Bold = true;
            }
            else
            {
                // Add data rows
                int rowIndex = 2;
                foreach (var report in ProjectProgressReportlists)
                {
                    worksheet.Cell(rowIndex, 1).Value = report.index;
                    worksheet.Cell(rowIndex, 2).Value = report.ProjectName;
                    worksheet.Cell(rowIndex, 3).Value = report.UnitCode;
                    worksheet.Cell(rowIndex, 4).Value = report.CompanyName;
                    worksheet.Cell(rowIndex, 5).Value = report.PEName;
                    worksheet.Cell(rowIndex, 6).Value = report.UnitStatus;
                    worksheet.Cell(rowIndex, 7).Value = report.TransferDate;
                    worksheet.Cell(rowIndex, 8).Value = report.StartDatePlan;
                    worksheet.Cell(rowIndex, 9).Value = report.EndDatePlan;
                    worksheet.Cell(rowIndex, 10).Value = report.FormActual;
                    worksheet.Cell(rowIndex, 11).Value = report.ProgressPlan;
                    worksheet.Cell(rowIndex, 12).Value = report.ProgressActual;
                    worksheet.Cell(rowIndex, 13).Value = report.DelayAhead;
                    worksheet.Cell(rowIndex, 14).Value = report.LastFormTransfer;

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
    public IActionResult SearchProject(string projectId)
    {
        // Save the projectId in a cookie
        Response.Cookies.Append("CkselectedProjectId", projectId, new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(7) // Set cookie to expire in 7 days
        });


        var en = new ReportProjectProgressModel
        {
            act = "ReportProjectProgress",
            project_id = projectId,
            unit_id = "",
            unit_status = "",
            build_status = ""
        };

        List<ReportProjectProgressModel> filteredProjects = _ProjectProgressReportProvider.sp_get_report_Project_Prcress(en);

        // Return the filtered data to the partial view
        return PartialView("PartialTable", filteredProjects);
    }
}
