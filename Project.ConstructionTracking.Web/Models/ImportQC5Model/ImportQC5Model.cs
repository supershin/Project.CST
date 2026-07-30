using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Models.ImportQC5Model
{
    public static class ImportQC5RowStatus
    {
        public const string Valid = "valid";         // พร้อมนำเข้า
        public const string Imported = "imported";   // นำเข้าสำเร็จ
        public const string Duplicate = "duplicate"; // มีข้อมูล QC5 ในระบบแล้ว → ข้าม
        public const string NoDate = "no_date";      // ไม่มี QC5 Date → ข้าม
        public const string Error = "error";         // ไม่พบโครงการ/ยูนิต หรือวันที่ผิดรูปแบบ
    }

    public class ImportQC5RowModel
    {
        public int RowNumber { get; set; }
        public string? ProjectCode { get; set; }
        public string? UnitCode { get; set; }
        public string? QC5DateText { get; set; }
        public DateTime? QC5Date { get; set; }
        public string? QC5DateDisplay => QC5Date?.ToString("dd/MM/yyyy");
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public string Status { get; set; } = ImportQC5RowStatus.Valid;
        public string? Message { get; set; }
    }

    public class ImportQC5ResultModel
    {
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int ImportedRows { get; set; }
        public int DuplicateRows { get; set; }
        public int NoDateRows { get; set; }
        public int ErrorRows { get; set; }
        public List<ImportQC5RowModel> Rows { get; set; } = new List<ImportQC5RowModel>();
    }
}
