using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Models.ImportQC5Model
{
    public static class ImportQC5RowStatus
    {
        public const string Valid = "valid";                 // พร้อมนำเข้า QC5 (ยังไม่มีข้อมูลในระบบ)
        public const string CompleteExisting = "complete_existing"; // มี QC5 ที่ยังไม่ผ่าน/ยังไม่ submit → ปิดเป็นผ่าน
        public const string SyncOnly = "sync_only";          // มี QC5 แล้ว แต่ยังไม่มี QC Sync → เพิ่มเฉพาะ Sync
        public const string Imported = "imported";           // นำเข้า QC5 สำเร็จ
        public const string ExistingCompleted = "existing_completed"; // ปิด QC5 เดิมเป็นผ่านสำเร็จ
        public const string SyncImported = "sync_imported";  // เพิ่ม QC Sync สำเร็จ
        public const string Duplicate = "duplicate";         // มีข้อมูลครบแล้ว → ข้าม
        public const string NoDate = "no_date";              // ไม่มี QC5 Date → ข้าม
        public const string Error = "error";                 // ไม่พบโครงการ/ยูนิต หรือวันที่ผิดรูปแบบ
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
        public Guid? ExistingQCUnitCheckListID { get; set; }
        public string Status { get; set; } = ImportQC5RowStatus.Valid;
        public string? Message { get; set; }

        // ต้องเพิ่ม tr_QC_UnitCheckList + tr_QC_UnitCheckList_Action หรือไม่
        public bool NeedCheckList { get; set; }
        // ต้องปิด tr_QC_UnitCheckList เดิมเป็นผ่านและเปลี่ยน Action เป็น submit หรือไม่
        public bool NeedCompleteCheckList { get; set; }
        // ต้องเพิ่ม/อัปเดต tr_QC_Sync หรือไม่
        public bool NeedSync { get; set; }
    }

    public class ImportQC5CommitResult
    {
        public int CheckListInserted { get; set; }
        public int CheckListUpdated { get; set; }
        public int SyncInserted { get; set; }
        public int SyncUpdated { get; set; }
    }

    public class ImportQC5ResultModel
    {
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int CompleteExistingRows { get; set; }
        public int SyncOnlyRows { get; set; }
        public int ImportedRows { get; set; }
        public int ExistingCompletedRows { get; set; }
        public int ImportedSyncRows { get; set; }
        public int DuplicateRows { get; set; }
        public int NoDateRows { get; set; }
        public int ErrorRows { get; set; }
        public List<ImportQC5RowModel> Rows { get; set; } = new List<ImportQC5RowModel>();
    }
}
