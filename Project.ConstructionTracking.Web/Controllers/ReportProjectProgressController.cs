using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.IO;
using System;
using Project.ConstructionTracking.Web.Controllers;
using Microsoft.Extensions.Hosting;

public class ReportProjectProgressController : BaseController
{
    private readonly IHostEnvironment _hosting;

    public ReportProjectProgressController(IHostEnvironment hosting)
    {
        _hosting = hosting;
    }

    public IActionResult Index()
    {
        return View();
    }

    //[HttpGet]
    //public IActionResult ExportToExcel()
    //{
    //    using (var workbook = new XLWorkbook())
    //    {
    //        var worksheet = workbook.Worksheets.Add("Project Progress Report");

    //        // Add headers
    //        worksheet.Cell(1, 1).Value = "No.";
    //        worksheet.Cell(1, 2).Value = "โครงการ";
    //        worksheet.Cell(1, 3).Value = "แปลงที่";
    //        worksheet.Cell(1, 4).Value = "ผู้รับเหมา";
    //        worksheet.Cell(1, 5).Value = "วิศวกรผู้ควบคุมงาน";
    //        worksheet.Cell(1, 6).Value = "สถานะขาย (จอง สัญญา โอน ห้องว่าง)";
    //        worksheet.Cell(1, 7).Value = "วันกำหนดโอน (ตามสัญญา)";
    //        worksheet.Cell(1, 8).Value = "แผนเริ่มตามสัญญา";
    //        worksheet.Cell(1, 9).Value = "แผนสิ้นสุดตามสัญญา";
    //        worksheet.Cell(1, 10).Value = "สถานะงวดงานที่ผ่านล่าสุด";
    //        worksheet.Cell(1, 11).Value = "% Progress ในแผน";
    //        worksheet.Cell(1, 12).Value = "% Progress จริง";
    //        worksheet.Cell(1, 13).Value = "% Delay/ahead";
    //        worksheet.Cell(1, 14).Value = "งวดที่เบิกล่าสุด";

    //        // Add sample data (replace with your real data)
    //        worksheet.Cell(2, 1).Value = 1;
    //        worksheet.Cell(2, 2).Value = "โครงการ เอสต้า ซีรีนิตี้ บรมราชชนนี";
    //        worksheet.Cell(2, 3).Value = "AA001";
    //        worksheet.Cell(2, 4).Value = "บริษัท XYZ ก่อสร้าง";
    //        worksheet.Cell(2, 5).Value = "นายสมชาย วิศวกร";
    //        worksheet.Cell(2, 6).Value = "จอง";
    //        worksheet.Cell(2, 7).Value = "15-Jan-2025";
    //        worksheet.Cell(2, 8).Value = "01-Feb-2024";
    //        worksheet.Cell(2, 9).Value = "31-Dec-2024";
    //        worksheet.Cell(2, 10).Value = "งวดที่ 5";
    //        worksheet.Cell(2, 11).Value = "70%";
    //        worksheet.Cell(2, 12).Value = "65%";
    //        worksheet.Cell(2, 13).Value = "-5%";
    //        worksheet.Cell(2, 14).Value = "งวดที่ 4";

    //        // Generate unique file name
    //        string uniqueFileName = $"Excel-{Guid.NewGuid()}.xlsx";

    //        // Construct the full path
    //        string filePath = Path.Combine(_hosting.ContentRootPath, "File", "temp", uniqueFileName);

    //        // Ensure the directory exists
    //        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

    //        // Save the file to the specified path
    //        workbook.SaveAs(filePath);

    //        // Return the file path or file itself as needed
    //        return Ok(new { filePath });
    //    }
    //}

    [HttpGet]
    public IActionResult ExportToExcel()
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

            // Add sample data (replace with your real data)
            worksheet.Cell(2, 1).Value = 1;
            worksheet.Cell(2, 2).Value = "โครงการ เอสต้า ซีรีนิตี้ บรมราชชนนี";
            worksheet.Cell(2, 3).Value = "AA001";
            worksheet.Cell(2, 4).Value = "บริษัท XYZ ก่อสร้าง";
            worksheet.Cell(2, 5).Value = "นายสมชาย วิศวกร";
            worksheet.Cell(2, 6).Value = "จอง";
            worksheet.Cell(2, 7).Value = "15-Jan-2025";
            worksheet.Cell(2, 8).Value = "01-Feb-2024";
            worksheet.Cell(2, 9).Value = "31-Dec-2024";
            worksheet.Cell(2, 10).Value = "งวดที่ 5";
            worksheet.Cell(2, 11).Value = "70%";
            worksheet.Cell(2, 12).Value = "65%";
            worksheet.Cell(2, 13).Value = "-5%";
            worksheet.Cell(2, 14).Value = "งวดที่ 4";

            // Generate unique file name
            string uniqueFileName = $"Excel-{Guid.NewGuid()}.xlsx";

            // Construct the full path
            string filePath = Path.Combine(_hosting.ContentRootPath, "File", "temp", uniqueFileName);

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Save the file to the specified path
            workbook.SaveAs(filePath);

            // Return the file as a download for the client
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", uniqueFileName);
        }
    }

}
