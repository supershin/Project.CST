using ClosedXML.Excel;
using Project.ConstructionTracking.Web.Models.ImportQC5Model;
using Project.ConstructionTracking.Web.Repositories;
using System.Globalization;

namespace Project.ConstructionTracking.Web.Services
{
    public class ImportQC5Service : IImportQC5Service
    {
        private readonly IImportQC5Repo _IImportQC5Repo;

        public ImportQC5Service(IImportQC5Repo importQC5Repo)
        {
            _IImportQC5Repo = importQC5Repo;
        }

        public ImportQC5ResultModel PreviewExcel(IFormFile fileExcel)
        {
            var rows = ParseExcel(fileExcel);
            _IImportQC5Repo.ValidateRows(rows);
            return BuildResult(rows);
        }

        public ImportQC5ResultModel ImportExcel(IFormFile fileExcel, Guid userID)
        {
            var rows = ParseExcel(fileExcel);
            _IImportQC5Repo.ValidateRows(rows);

            // นำเข้า QC5 ใหม่, ปิด QC5 เดิมที่ยังไม่เสร็จ และเติม QC Sync ที่ขาด
            var rowsToImport = rows
                .Where(o => o.Status == ImportQC5RowStatus.Valid
                         || o.Status == ImportQC5RowStatus.CompleteExisting
                         || o.Status == ImportQC5RowStatus.SyncOnly)
                .ToList();

            var commitResult = new ImportQC5CommitResult();
            if (rowsToImport.Count > 0)
            {
                commitResult = _IImportQC5Repo.ImportRows(rowsToImport, userID);
                foreach (var row in rowsToImport)
                {
                    if (row.NeedCheckList)
                    {
                        row.Status = ImportQC5RowStatus.Imported;
                        row.Message = "นำเข้าข้อมูล QC5 สำเร็จ";
                    }
                    else if (row.NeedCompleteCheckList)
                    {
                        row.Status = ImportQC5RowStatus.ExistingCompleted;
                        row.Message = "ปิดข้อมูล QC5 เดิมเป็นผ่านสำเร็จ";
                    }
                    else
                    {
                        row.Status = ImportQC5RowStatus.SyncImported;
                        row.Message = "เพิ่มข้อมูล QC Sync สำเร็จ";
                    }
                }
            }

            var result = BuildResult(rows);
            result.ImportedRows = commitResult.CheckListInserted;
            result.ExistingCompletedRows = commitResult.CheckListUpdated;
            result.ImportedSyncRows = commitResult.SyncInserted + commitResult.SyncUpdated;
            return result;
        }

        private static List<ImportQC5RowModel> ParseExcel(IFormFile fileExcel)
        {
            var rows = new List<ImportQC5RowModel>();

            using (var stream = fileExcel.OpenReadStream())
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var lastRowUsed = worksheet.LastRowUsed();
                var lastColUsed = worksheet.LastColumnUsed();

                if (lastRowUsed == null || lastColUsed == null)
                {
                    throw new Exception("ไฟล์ Excel ไม่มีข้อมูล");
                }

                int lastRow = lastRowUsed.RowNumber();
                int lastCol = lastColUsed.ColumnNumber();

                // หาแถวหัวตาราง (สแกน 10 แถวแรก) โดยดูจากชื่อคอลัมน์ REM Unit Code / Project ID / QC5 Date
                int headerRow = -1, colUnit = -1, colProject = -1, colDate = -1;
                int maxScanRow = Math.Min(10, lastRow);

                for (int r = 1; r <= maxScanRow; r++)
                {
                    int cu = -1, cp = -1, cd = -1;
                    for (int c = 1; c <= lastCol; c++)
                    {
                        string header = NormalizeHeader(worksheet.Cell(r, c).GetString());
                        if (header == "remunitcode") cu = c;
                        else if (header == "projectid") cp = c;
                        else if (header == "qc5date") cd = c;
                    }

                    if (cu > 0 && cp > 0 && cd > 0)
                    {
                        headerRow = r;
                        colUnit = cu;
                        colProject = cp;
                        colDate = cd;
                        break;
                    }
                }

                if (headerRow < 0)
                {
                    throw new Exception("ไม่พบหัวตาราง 'REM Unit Code', 'Project ID', 'QC5 Date' ในไฟล์ Excel");
                }

                for (int r = headerRow + 1; r <= lastRow; r++)
                {
                    string unitCode = worksheet.Cell(r, colUnit).GetString().Trim();
                    string projectCode = worksheet.Cell(r, colProject).GetString().Trim();

                    if (unitCode == "" && projectCode == "") continue; // ข้ามแถวว่าง

                    var row = new ImportQC5RowModel
                    {
                        RowNumber = r,
                        UnitCode = unitCode,
                        ProjectCode = projectCode
                    };

                    row.QC5Date = ReadDateCell(worksheet.Cell(r, colDate), out string rawText);
                    row.QC5DateText = rawText;

                    rows.Add(row);
                }
            }

            return rows;
        }

        private static string NormalizeHeader(string? header)
        {
            if (string.IsNullOrWhiteSpace(header)) return "";
            return header.Replace(" ", "").Replace("\u00A0", "").Trim().ToLower();
        }

        private static DateTime? ReadDateCell(IXLCell cell, out string rawText)
        {
            rawText = "";
            try
            {
                if (cell.DataType == XLDataType.DateTime)
                {
                    var dateValue = cell.GetDateTime();
                    rawText = dateValue.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
                    return dateValue;
                }

                if (cell.DataType == XLDataType.Number)
                {
                    var dateValue = DateTime.FromOADate(cell.GetDouble());
                    rawText = dateValue.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
                    return dateValue;
                }

                rawText = cell.GetString().Trim();
                if (string.IsNullOrWhiteSpace(rawText)) return null;

                // ไฟล์จาก SAP ใช้รูปแบบวันที่แบบอเมริกัน เดือน/วัน/ปี ค.ศ. เช่น 2/8/2024 = 8 ก.พ. 2024
                if (DateTime.TryParse(rawText, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out DateTime parsed))
                {
                    return parsed;
                }

                return null; // มีข้อความแต่แปลงวันที่ไม่ได้ → repo จะรายงานเป็น error
            }
            catch
            {
                return null;
            }
        }

        private static ImportQC5ResultModel BuildResult(List<ImportQC5RowModel> rows)
        {
            return new ImportQC5ResultModel
            {
                TotalRows = rows.Count,
                ValidRows = rows.Count(o => o.Status == ImportQC5RowStatus.Valid),
                CompleteExistingRows = rows.Count(o => o.Status == ImportQC5RowStatus.CompleteExisting),
                SyncOnlyRows = rows.Count(o => o.Status == ImportQC5RowStatus.SyncOnly),
                ExistingCompletedRows = rows.Count(o => o.Status == ImportQC5RowStatus.ExistingCompleted),
                DuplicateRows = rows.Count(o => o.Status == ImportQC5RowStatus.Duplicate),
                NoDateRows = rows.Count(o => o.Status == ImportQC5RowStatus.NoDate),
                ErrorRows = rows.Count(o => o.Status == ImportQC5RowStatus.Error),
                Rows = rows
            };
        }
    }
}
