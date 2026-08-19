using System;
using ClosedXML.Excel;
using Project.ConstructionTracking.Web.Models.MFormModel;

namespace Project.ConstructionTracking.Web.Commons
{
    /// <summary>
    /// สร้างไฟล์ Excel แบบฟอร์มการตรวจงวดงาน ทั้ง 4 ระดับ
    /// ประกอบด้วย 3 ชีท : ภาพรวม / โครงสร้างฟอร์ม / ตารางข้อมูล
    /// </summary>
    public static class MasterFormExcelExporter
    {
        // ชุดสีของระบบ (อิงจากสีหลักของเว็บ #0054A6)
        private static readonly XLColor Navy = XLColor.FromHtml("#0B3C68");
        private static readonly XLColor BrandBlue = XLColor.FromHtml("#0054A6");
        private static readonly XLColor Lv2Blue = XLColor.FromHtml("#2A79C8");
        private static readonly XLColor Lv3Blue = XLColor.FromHtml("#A8CBEA");
        private static readonly XLColor Lv4Blue = XLColor.FromHtml("#E8F1FA");
        private static readonly XLColor Amber = XLColor.FromHtml("#F0A500");
        private static readonly XLColor SoftGrey = XLColor.FromHtml("#F5F7FA");
        private static readonly XLColor LineGrey = XLColor.FromHtml("#D6DEE8");
        private static readonly XLColor TextDark = XLColor.FromHtml("#1F2D3D");
        private static readonly XLColor TextMute = XLColor.FromHtml("#6B7C93");

        private const string FontName = "Tahoma";

        public static byte[] Build(ExportFormStructureModel data)
        {
            using (var workbook = new XLWorkbook())
            {
                workbook.Style.Font.FontName = FontName;

                BuildSummarySheet(workbook, data);
                BuildStructureSheet(workbook, data);
                BuildFlatSheet(workbook, data);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static string BuildFileName(ExportFormStructureModel data)
        {
            string name = string.IsNullOrWhiteSpace(data.FormTypeName) ? "MasterForm" : data.FormTypeName.Trim();

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            return $"แบบฟอร์มการตรวจงวดงาน_{name}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
        }

        #region Sheet 1 : ภาพรวม

        private static void BuildSummarySheet(XLWorkbook workbook, ExportFormStructureModel data)
        {
            var ws = workbook.Worksheets.Add("ภาพรวม");
            ws.ShowGridLines = false;
            ws.Style.Font.FontSize = 10;

            // ---------- แถบหัวเรื่อง ----------
            ws.Range("B2:J3").Merge();
            ws.Cell(2, 2).Value = "แบบฟอร์มการตรวจงวดงาน";
            ws.Cell(2, 2).Style
                .Font.SetBold(true).Font.SetFontSize(20).Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(BrandBlue)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetIndent(1);

            ws.Range("B4:J4").Merge();
            ws.Cell(4, 2).Value = "Master Form Structure  •  Lv.1 Form → Lv.2 Form Group → Lv.3 Form Package → Lv.4 Form CheckList";
            ws.Cell(4, 2).Style
                .Font.SetFontSize(9).Font.SetFontColor(Navy)
                .Fill.SetBackgroundColor(Amber)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetIndent(1);
            ws.Row(4).Height = 18;

            // ---------- ข้อมูลประเภทฟอร์ม ----------
            int row = 6;
            row = WriteInfoLine(ws, row, "ประเภทโครงการ", data.ProjectTypeName);
            row = WriteInfoLine(ws, row, "ชื่อประเภทฟอร์ม", data.FormTypeName);
            row = WriteInfoLine(ws, row, "รายละเอียด", data.FormTypeDesc);
            row = WriteInfoLine(ws, row, "วันที่ออกรายงาน", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            // ---------- การ์ดสรุปจำนวนแต่ละระดับ ----------
            row++;
            int cardRow = row;
            WriteStatCard(ws, cardRow, 2, "Lv.1  งวดงาน", data.TotalForm, BrandBlue, XLColor.White);
            WriteStatCard(ws, cardRow, 4, "Lv.2  กลุ่มงาน", data.TotalGroup, Lv2Blue, XLColor.White);
            WriteStatCard(ws, cardRow, 6, "Lv.3  แพ็คเกจ", data.TotalPackage, Lv3Blue, Navy);
            WriteStatCard(ws, cardRow, 8, "Lv.4  รายการตรวจ", data.TotalCheckList, Lv4Blue, Navy);
            ws.Row(cardRow).Height = 20;
            ws.Row(cardRow + 1).Height = 26;

            // ---------- ตารางสรุปรายงวดงาน ----------
            row = cardRow + 3;
            ws.Cell(row, 2).Value = "สรุปรายงวดงาน (Lv.1 Form)";
            ws.Cell(row, 2).Style.Font.SetBold(true).Font.SetFontSize(12).Font.SetFontColor(Navy);
            row++;

            string[] headers = { "ลำดับ", "ชื่องวดงาน", "รายละเอียด", "% Progress", "ระยะเวลา (วัน)", "กลุ่มงาน", "แพ็คเกจ", "รายการตรวจ", "QC ที่เกี่ยวข้อง" };
            int headerRow = row;
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, 2 + i).Value = headers[i];
            }
            StyleHeaderRow(ws.Range(headerRow, 2, headerRow, 2 + headers.Length - 1));
            row++;

            if (data.Forms.Count == 0)
            {
                WriteEmptyRow(ws, row, 2, 2 + headers.Length - 1);
                row++;
            }
            else
            {
                int no = 1;
                foreach (var form in data.Forms)
                {
                    ws.Cell(row, 2).Value = no;
                    ws.Cell(row, 3).Value = form.Name;
                    ws.Cell(row, 4).Value = form.Description;
                    ws.Cell(row, 5).Value = form.Progress;
                    ws.Cell(row, 6).Value = form.DurationDay;
                    ws.Cell(row, 7).Value = form.Groups.Count;
                    ws.Cell(row, 8).Value = form.TotalPackage;
                    ws.Cell(row, 9).Value = form.TotalCheckList;
                    ws.Cell(row, 10).Value = form.QcList.Count > 0 ? string.Join(", ", form.QcList) : "-";

                    var line = ws.Range(row, 2, row, 10);
                    line.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetBottomBorderColor(LineGrey);
                    line.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    if (no % 2 == 0) line.Style.Fill.SetBackgroundColor(SoftGrey);

                    ws.Cell(row, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell(row, 3).Style.Font.SetBold(true).Font.SetFontColor(Navy);
                    ws.Cell(row, 5).Style.NumberFormat.SetFormat("0.00\"%\"").Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Range(row, 6, row, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    no++;
                    row++;
                }

                // แถวรวม
                ws.Cell(row, 2).Value = "รวม";
                ws.Range(row, 2, row, 4).Merge();
                ws.Cell(row, 5).Value = data.Forms.Sum(f => f.Progress ?? 0);
                ws.Cell(row, 6).Value = data.Forms.Sum(f => f.DurationDay ?? 0);
                ws.Cell(row, 7).Value = data.TotalGroup;
                ws.Cell(row, 8).Value = data.TotalPackage;
                ws.Cell(row, 9).Value = data.TotalCheckList;

                var totalLine = ws.Range(row, 2, row, 10);
                totalLine.Style
                    .Font.SetBold(true).Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(Navy)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Cell(row, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(row, 5).Style.NumberFormat.SetFormat("0.00\"%\"").Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Range(row, 6, row, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Row(row).Height = 20;
            }

            // ---------- ความกว้างคอลัมน์ ----------
            ws.Column(1).Width = 2;
            ws.Column(2).Width = 8;
            ws.Column(3).Width = 34;
            ws.Column(4).Width = 42;
            ws.Column(5).Width = 12;
            ws.Column(6).Width = 14;
            ws.Column(7).Width = 11;
            ws.Column(8).Width = 11;
            ws.Column(9).Width = 13;
            ws.Column(10).Width = 32;

            ws.SheetView.FreezeRows(headerRow);
        }

        private static int WriteInfoLine(IXLWorksheet ws, int row, string label, string? value)
        {
            ws.Cell(row, 2).Value = label;
            ws.Cell(row, 2).Style
                .Font.SetBold(true).Font.SetFontColor(TextMute).Font.SetFontSize(9)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Range(row, 3, row, 10).Merge();
            ws.Cell(row, 3).Value = string.IsNullOrWhiteSpace(value) ? "-" : value;
            ws.Cell(row, 3).Style
                .Font.SetFontColor(TextDark)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Border.SetBottomBorder(XLBorderStyleValues.Hair).Border.SetBottomBorderColor(LineGrey);

            ws.Row(row).Height = 17;
            return row + 1;
        }

        private static void WriteStatCard(IXLWorksheet ws, int row, int col, string label, int value, XLColor back, XLColor fore)
        {
            ws.Range(row, col, row, col + 1).Merge();
            ws.Cell(row, col).Value = label;
            ws.Cell(row, col).Style
                .Font.SetBold(true).Font.SetFontSize(9).Font.SetFontColor(fore)
                .Fill.SetBackgroundColor(back)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Range(row + 1, col, row + 1, col + 1).Merge();
            ws.Cell(row + 1, col).Value = value;
            ws.Cell(row + 1, col).Style
                .Font.SetBold(true).Font.SetFontSize(18).Font.SetFontColor(Navy)
                .Fill.SetBackgroundColor(SoftGrey)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(LineGrey);
        }

        #endregion

        #region Sheet 2 : โครงสร้างฟอร์ม

        private static void BuildStructureSheet(XLWorkbook workbook, ExportFormStructureModel data)
        {
            var ws = workbook.Worksheets.Add("โครงสร้างฟอร์ม");
            ws.ShowGridLines = false;
            ws.Style.Font.FontSize = 10;

            ws.Range("B2:H2").Merge();
            ws.Cell(2, 2).Value = $"โครงสร้างแบบฟอร์ม : {data.FormTypeName}";
            ws.Cell(2, 2).Style
                .Font.SetBold(true).Font.SetFontSize(14).Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(BrandBlue)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetIndent(1);
            ws.Row(2).Height = 26;

            ws.Range("B3:H3").Merge();
            ws.Cell(3, 2).Value = "กดปุ่ม +/- ด้านซ้ายเพื่อย่อ-ขยายแต่ละระดับ";
            ws.Cell(3, 2).Style.Font.SetFontSize(8).Font.SetItalic(true).Font.SetFontColor(TextMute).Alignment.SetIndent(1);

            int headerRow = 5;
            string[] headers = { "ระดับ", "ลำดับ", "รายการ", "รายละเอียด", "% Progress", "ระยะเวลา (วัน)", "QC ที่เกี่ยวข้อง" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, 2 + i).Value = headers[i];
            }
            StyleHeaderRow(ws.Range(headerRow, 2, headerRow, 2 + headers.Length - 1));

            int row = headerRow + 1;

            if (data.Forms.Count == 0)
            {
                WriteEmptyRow(ws, row, 2, 8);
            }
            else
            {
                int formNo = 1;
                foreach (var form in data.Forms)
                {
                    int formStart = row;

                    ws.Cell(row, 2).Value = "Lv.1";
                    ws.Cell(row, 3).Value = formNo;
                    ws.Cell(row, 4).Value = form.Name;
                    ws.Cell(row, 5).Value = form.Description;
                    ws.Cell(row, 6).Value = form.Progress;
                    ws.Cell(row, 7).Value = form.DurationDay;
                    ws.Cell(row, 8).Value = form.QcList.Count > 0 ? string.Join(", ", form.QcList) : "-";
                    StyleLevelRow(ws, row, 1);
                    row++;

                    int groupNo = 1;
                    foreach (var group in form.Groups)
                    {
                        int groupStart = row;

                        ws.Cell(row, 2).Value = "Lv.2";
                        ws.Cell(row, 3).Value = $"{formNo}.{groupNo}";
                        ws.Cell(row, 4).Value = group.Name;
                        StyleLevelRow(ws, row, 2);
                        row++;

                        int packageNo = 1;
                        foreach (var package in group.Packages)
                        {
                            int packageStart = row;

                            ws.Cell(row, 2).Value = "Lv.3";
                            ws.Cell(row, 3).Value = $"{formNo}.{groupNo}.{packageNo}";
                            ws.Cell(row, 4).Value = package.Name;
                            StyleLevelRow(ws, row, 3);
                            row++;

                            int checkNo = 1;
                            foreach (var check in package.CheckLists)
                            {
                                ws.Cell(row, 2).Value = "Lv.4";
                                ws.Cell(row, 3).Value = $"{formNo}.{groupNo}.{packageNo}.{checkNo}";
                                ws.Cell(row, 4).Value = check.Name;
                                StyleLevelRow(ws, row, 4);
                                row++;
                                checkNo++;
                            }

                            // ยุบกลุ่มของ Lv.4 ที่อยู่ใต้ Lv.3
                            if (row > packageStart + 1) ws.Rows(packageStart + 1, row - 1).Group();
                            packageNo++;
                        }

                        // ยุบกลุ่มของ Lv.3 - Lv.4 ที่อยู่ใต้ Lv.2
                        if (row > groupStart + 1) ws.Rows(groupStart + 1, row - 1).Group();
                        groupNo++;
                    }

                    // ยุบกลุ่มของ Lv.2 - Lv.4 ที่อยู่ใต้ Lv.1
                    if (row > formStart + 1) ws.Rows(formStart + 1, row - 1).Group();
                    formNo++;
                }
            }

            ws.Column(1).Width = 3;
            ws.Column(2).Width = 8;
            ws.Column(3).Width = 12;
            ws.Column(4).Width = 52;
            ws.Column(5).Width = 42;
            ws.Column(6).Width = 12;
            ws.Column(7).Width = 14;
            ws.Column(8).Width = 30;

            ws.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;
            ws.SheetView.FreezeRows(headerRow);
        }

        /// <summary>จัดรูปแบบแถวตามระดับ 1-4 พร้อมย่อหน้าให้เห็นลำดับชั้น</summary>
        private static void StyleLevelRow(IXLWorksheet ws, int row, int level)
        {
            var line = ws.Range(row, 2, row, 8);
            line.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetBottomBorderColor(LineGrey);

            ws.Cell(row, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetFontSize(8);
            ws.Cell(row, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetFontSize(9);
            ws.Range(row, 6, row, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell(row, 6).Style.NumberFormat.SetFormat("0.00\"%\"");
            ws.Cell(row, 4).Style.Alignment.SetIndent(level - 1);

            switch (level)
            {
                case 1:
                    line.Style.Fill.SetBackgroundColor(BrandBlue).Font.SetBold(true).Font.SetFontColor(XLColor.White);
                    ws.Row(row).Height = 22;
                    break;
                case 2:
                    line.Style.Fill.SetBackgroundColor(Lv2Blue).Font.SetBold(true).Font.SetFontColor(XLColor.White);
                    ws.Row(row).Height = 19;
                    break;
                case 3:
                    line.Style.Fill.SetBackgroundColor(Lv3Blue).Font.SetBold(true).Font.SetFontColor(Navy);
                    break;
                default:
                    line.Style.Fill.SetBackgroundColor(Lv4Blue).Font.SetFontColor(TextDark);
                    break;
            }
        }

        #endregion

        #region Sheet 3 : ตารางข้อมูล

        private static void BuildFlatSheet(XLWorkbook workbook, ExportFormStructureModel data)
        {
            var ws = workbook.Worksheets.Add("ตารางข้อมูล");
            ws.Style.Font.FontSize = 10;

            string[] headers =
            {
                "ประเภทโครงการ", "ประเภทฟอร์ม",
                "Lv.1 งวดงาน", "% Progress", "ระยะเวลา (วัน)", "QC ที่เกี่ยวข้อง",
                "Lv.2 กลุ่มงาน", "Lv.3 แพ็คเกจ", "Lv.4 รายการตรวจ"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, 1 + i).Value = headers[i];
            }
            StyleHeaderRow(ws.Range(1, 1, 1, headers.Length));

            int row = 2;
            foreach (var form in data.Forms)
            {
                string qc = form.QcList.Count > 0 ? string.Join(", ", form.QcList) : "-";

                if (form.Groups.Count == 0)
                {
                    WriteFlatRow(ws, row++, data, form, qc, "-", "-", "-");
                    continue;
                }

                foreach (var group in form.Groups)
                {
                    if (group.Packages.Count == 0)
                    {
                        WriteFlatRow(ws, row++, data, form, qc, group.Name, "-", "-");
                        continue;
                    }

                    foreach (var package in group.Packages)
                    {
                        if (package.CheckLists.Count == 0)
                        {
                            WriteFlatRow(ws, row++, data, form, qc, group.Name, package.Name, "-");
                            continue;
                        }

                        foreach (var check in package.CheckLists)
                        {
                            WriteFlatRow(ws, row++, data, form, qc, group.Name, package.Name, check.Name);
                        }
                    }
                }
            }

            if (row == 2)
            {
                WriteEmptyRow(ws, row, 1, headers.Length);
                row++;
            }

            ws.Range(1, 1, row - 1, headers.Length).SetAutoFilter();

            ws.Column(1).Width = 18;
            ws.Column(2).Width = 26;
            ws.Column(3).Width = 28;
            ws.Column(4).Width = 12;
            ws.Column(5).Width = 14;
            ws.Column(6).Width = 26;
            ws.Column(7).Width = 30;
            ws.Column(8).Width = 30;
            ws.Column(9).Width = 40;

            ws.SheetView.FreezeRows(1);
        }

        private static void WriteFlatRow(IXLWorksheet ws, int row, ExportFormStructureModel data,
                                         ExportFormLevel form, string qc, string? group, string? package, string? check)
        {
            ws.Cell(row, 1).Value = data.ProjectTypeName;
            ws.Cell(row, 2).Value = data.FormTypeName;
            ws.Cell(row, 3).Value = form.Name;
            ws.Cell(row, 4).Value = form.Progress;
            ws.Cell(row, 5).Value = form.DurationDay;
            ws.Cell(row, 6).Value = qc;
            ws.Cell(row, 7).Value = group;
            ws.Cell(row, 8).Value = package;
            ws.Cell(row, 9).Value = check;

            var line = ws.Range(row, 1, row, 9);
            line.Style.Border.SetBottomBorder(XLBorderStyleValues.Hair).Border.SetBottomBorderColor(LineGrey);
            if (row % 2 == 0) line.Style.Fill.SetBackgroundColor(SoftGrey);

            ws.Cell(row, 4).Style.NumberFormat.SetFormat("0.00\"%\"").Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell(row, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }

        #endregion

        #region Helper

        private static void StyleHeaderRow(IXLRange range)
        {
            range.Style
                .Font.SetBold(true).Font.SetFontColor(XLColor.White).Font.SetFontSize(10)
                .Fill.SetBackgroundColor(Navy)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetWrapText(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(Navy)
                .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.White);

            range.Worksheet.Row(range.FirstRow().RowNumber()).Height = 28;
        }

        private static void WriteEmptyRow(IXLWorksheet ws, int row, int firstCol, int lastCol)
        {
            ws.Range(row, firstCol, row, lastCol).Merge();
            ws.Cell(row, firstCol).Value = "ไม่มีข้อมูล";
            ws.Cell(row, firstCol).Style
                .Font.SetItalic(true).Font.SetFontColor(TextMute)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Row(row).Height = 24;
        }

        #endregion
    }
}
