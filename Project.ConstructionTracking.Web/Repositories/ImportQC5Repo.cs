using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models.ImportQC5Model;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class ImportQC5Repo : IImportQC5Repo
    {
        private readonly ContructionTrackingDbContext _context;

        // ค่าคงที่สำหรับข้อมูล QC5 ย้อนหลัง (ตามที่ตกลงกับผู้ใช้งาน)
        private const int QC5CheckListID = 6;
        private const int QC5QCTypeID = 17;
        private const int QC5Seq = 1;
        private const int QC5QCStatusID = 1;
        private const int QC5ActionRoleID = 4;
        private const string QC5ActionType = "submit";
        private const int QC5SyncType = 41;
        private const string QC5SyncAppointTime = "12:00";

        public ImportQC5Repo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public void ValidateRows(List<ImportQC5RowModel> rows)
        {
            var projectCodes = rows
                .Where(o => !string.IsNullOrWhiteSpace(o.ProjectCode))
                .Select(o => o.ProjectCode!.Trim())
                .Distinct()
                .ToList();

            var projects = _context.tm_Project
                .Where(o => o.ProjectCode != null && projectCodes.Contains(o.ProjectCode) && o.FlagActive == true)
                .Select(o => new { o.ProjectID, o.ProjectCode })
                .ToList();

            var projectDict = projects
                .GroupBy(o => o.ProjectCode!.Trim().ToUpper())
                .ToDictionary(g => g.Key, g => g.First().ProjectID);

            var projectIDs = projectDict.Values.ToList();

            var units = _context.tm_Unit
                .Where(o => o.ProjectID != null && projectIDs.Contains(o.ProjectID.Value) && o.FlagActive == true)
                .Select(o => new { o.UnitID, o.ProjectID, o.UnitCode })
                .ToList();

            var unitDict = new Dictionary<string, Guid>();
            foreach (var unit in units)
            {
                if (unit.UnitCode == null || unit.ProjectID == null) continue;
                string key = unit.ProjectID.Value + "|" + unit.UnitCode.Trim().ToUpper();
                if (!unitDict.ContainsKey(key))
                {
                    unitDict.Add(key, unit.UnitID);
                }
            }

            // รอบที่ 1 : จับคู่โครงการ/ยูนิต และตรวจวันที่ เพื่อให้ได้รายการ Unit ที่ต้องไปตรวจสอบในฐานข้อมูล
            var seenInFile = new HashSet<Guid>();
            var resolvedRows = new List<ImportQC5RowModel>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.ProjectCode) || string.IsNullOrWhiteSpace(row.UnitCode))
                {
                    row.Status = ImportQC5RowStatus.Error;
                    row.Message = "ข้อมูล Project ID หรือ REM Unit Code ไม่ครบถ้วน";
                    continue;
                }

                if (!projectDict.TryGetValue(row.ProjectCode.Trim().ToUpper(), out Guid projectID))
                {
                    row.Status = ImportQC5RowStatus.Error;
                    row.Message = "ไม่พบรหัสโครงการนี้ในระบบ";
                    continue;
                }
                row.ProjectID = projectID;

                string unitKey = projectID + "|" + row.UnitCode.Trim().ToUpper();
                if (!unitDict.TryGetValue(unitKey, out Guid unitID))
                {
                    row.Status = ImportQC5RowStatus.Error;
                    row.Message = "ไม่พบ Unit นี้ในโครงการ";
                    continue;
                }
                row.UnitID = unitID;

                if (row.QC5Date == null)
                {
                    if (string.IsNullOrWhiteSpace(row.QC5DateText))
                    {
                        row.Status = ImportQC5RowStatus.NoDate;
                        row.Message = "ไม่มีข้อมูล QC5 Date (ข้ามการนำเข้า)";
                    }
                    else
                    {
                        row.Status = ImportQC5RowStatus.Error;
                        row.Message = "รูปแบบวันที่ไม่ถูกต้อง : " + row.QC5DateText;
                    }
                    continue;
                }

                if (!seenInFile.Add(unitID))
                {
                    row.Status = ImportQC5RowStatus.Duplicate;
                    row.Message = "ข้อมูล Unit นี้ซ้ำในไฟล์ Excel (ข้ามการนำเข้า)";
                    continue;
                }

                resolvedRows.Add(row);
            }

            if (resolvedRows.Count == 0) return;

            // ตรวจสอบข้อมูลเดิมเฉพาะ Unit ที่อยู่ในไฟล์ Excel เท่านั้น
            var targetUnitIDs = resolvedRows.Select(o => o.UnitID!.Value).ToList();

            var existingQC5s = _context.tr_QC_UnitCheckList
                .Where(o => o.UnitID != null && targetUnitIDs.Contains(o.UnitID.Value)
                         && o.QCTypeID == QC5QCTypeID && o.FlagActive == true)
                .ToList()
                .GroupBy(o => o.UnitID!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(o => o.Seq)
                          .ThenByDescending(o => o.UpdateDate ?? o.CreateDate)
                          .First());

            var existingQC5IDs = existingQC5s.Values.Select(o => o.ID).ToList();
            var existingQC5Actions = _context.tr_QC_UnitCheckList_Action
                .Where(o => o.QCUnitCheckListID != null && existingQC5IDs.Contains(o.QCUnitCheckListID.Value))
                .ToList()
                .GroupBy(o => o.QCUnitCheckListID!.Value)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(o => o.ID).First());

            var existingSyncUnitIDs = _context.tr_QC_Sync
                .Where(o => o.UnitID != null && targetUnitIDs.Contains(o.UnitID.Value) && o.QCTypeID == QC5QCTypeID)
                .Select(o => o.UnitID!.Value)
                .ToList()
                .ToHashSet();

            // รอบที่ 2 : ตัดสินว่าแต่ละแถวต้องเพิ่มอะไรบ้าง
            foreach (var row in resolvedRows)
            {
                Guid unitID = row.UnitID!.Value;

                // ถ้า QC5 ล่าสุดยังไม่ผ่านหรือยังไม่ submit ให้ Import ปิดรายการเดิมเป็นผ่าน
                // โดยไม่แก้ไขหรือลบ Defect ที่ผูกกับรายการนั้น
                if (existingQC5s.TryGetValue(unitID, out tr_QC_UnitCheckList? existingQC5))
                {
                    row.ExistingQCUnitCheckListID = existingQC5.ID;

                    bool hasSubmittedAction = existingQC5Actions.TryGetValue(existingQC5.ID, out tr_QC_UnitCheckList_Action? existingAction)
                        && string.Equals(existingAction.ActionType, QC5ActionType, StringComparison.OrdinalIgnoreCase);
                    bool isCompleted = existingQC5.QCStatusID == QC5QCStatusID && hasSubmittedAction;

                    if (!isCompleted)
                    {
                        row.Status = ImportQC5RowStatus.CompleteExisting;
                        row.NeedCompleteCheckList = true;
                        row.NeedSync = true;
                        row.Message = "QC5 เดิมยังไม่เสร็จ จะปิดเป็นผ่านและบันทึกวันที่จาก Excel";
                        continue;
                    }

                    if (existingSyncUnitIDs.Contains(unitID))
                    {
                        row.Status = ImportQC5RowStatus.Duplicate;
                        row.Message = "Unit นี้มีข้อมูล QC5 และ QC Sync ในระบบแล้ว (ข้ามการนำเข้า)";
                    }
                    else
                    {
                        row.Status = ImportQC5RowStatus.SyncOnly;
                        row.NeedSync = true;
                        row.Message = "มีข้อมูล QC5 แล้ว จะเพิ่มเฉพาะข้อมูล QC Sync";
                    }
                    continue;
                }

                row.Status = ImportQC5RowStatus.Valid;
                row.NeedCheckList = true;
                row.NeedSync = true;
                row.Message = "พร้อมนำเข้า";
            }
        }

        public ImportQC5CommitResult ImportRows(List<ImportQC5RowModel> rowsToImport, Guid userID)
        {
            DateTime now = DateTime.Now;
            var commitResult = new ImportQC5CommitResult();

            // โหลด tr_QC_Sync เดิมของทุก Unit ในชุดนี้ไว้ก่อน (คีย์ = UnitID + QCTypeID ตามที่ระบบใช้ค้นหา)
            var importUnitIDs = rowsToImport
                .Where(o => o.UnitID != null)
                .Select(o => o.UnitID!.Value)
                .ToList();

            var existingSyncs = _context.tr_QC_Sync
                .Where(o => o.UnitID != null && importUnitIDs.Contains(o.UnitID.Value) && o.QCTypeID == QC5QCTypeID)
                .ToList()
                .GroupBy(o => o.UnitID!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            var checklistIDsToComplete = rowsToImport
                .Where(o => o.NeedCompleteCheckList && o.ExistingQCUnitCheckListID != null)
                .Select(o => o.ExistingQCUnitCheckListID!.Value)
                .Distinct()
                .ToList();

            var checklistsToComplete = _context.tr_QC_UnitCheckList
                .Where(o => checklistIDsToComplete.Contains(o.ID))
                .ToDictionary(o => o.ID);

            var actionsToComplete = _context.tr_QC_UnitCheckList_Action
                .Where(o => o.QCUnitCheckListID != null && checklistIDsToComplete.Contains(o.QCUnitCheckListID.Value))
                .ToList()
                .GroupBy(o => o.QCUnitCheckListID!.Value)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(o => o.ID).First());

            foreach (var row in rowsToImport)
            {
                if (row.NeedCheckList)
                {
                    var newQCUnitCheckList = new tr_QC_UnitCheckList
                    {
                        ID = Guid.NewGuid(),
                        ProjectID = row.ProjectID,
                        UnitID = row.UnitID,
                        CheckListID = QC5CheckListID,
                        QCTypeID = QC5QCTypeID,
                        Seq = QC5Seq,
                        CheckListDate = row.QC5Date,
                        QCStatusID = QC5QCStatusID,
                        PESignResourceID = null,
                        FlagActive = true,
                        CreateDate = now,
                        CreateBy = userID,
                        UpdateDate = now,
                        UpdateBy = userID
                    };
                    _context.tr_QC_UnitCheckList.Add(newQCUnitCheckList);

                    var newQCUnitCheckListAction = new tr_QC_UnitCheckList_Action
                    {
                        QCUnitCheckListID = newQCUnitCheckList.ID,
                        RoleID = QC5ActionRoleID,
                        ActionType = QC5ActionType,
                        StatusID = null,
                        Remark = null,
                        ActionDate = row.QC5Date,
                        CreateDate = now,
                        CreateBy = userID,
                        UpdateDate = now,
                        UpdateBy = userID
                    };
                    _context.tr_QC_UnitCheckList_Action.Add(newQCUnitCheckListAction);

                    commitResult.CheckListInserted++;
                }

                if (row.NeedCompleteCheckList && row.ExistingQCUnitCheckListID != null)
                {
                    Guid checklistID = row.ExistingQCUnitCheckListID.Value;
                    if (!checklistsToComplete.TryGetValue(checklistID, out tr_QC_UnitCheckList? existingChecklist))
                    {
                        throw new Exception($"ไม่พบข้อมูล QC5 เดิมของ Unit {row.UnitCode}");
                    }

                    existingChecklist.CheckListDate = row.QC5Date;
                    existingChecklist.QCStatusID = QC5QCStatusID;
                    existingChecklist.UpdateDate = now;
                    existingChecklist.UpdateBy = userID;
                    _context.tr_QC_UnitCheckList.Update(existingChecklist);

                    if (actionsToComplete.TryGetValue(checklistID, out tr_QC_UnitCheckList_Action? existingAction))
                    {
                        existingAction.ActionType = QC5ActionType;
                        existingAction.ActionDate = row.QC5Date;
                        existingAction.UpdateDate = now;
                        existingAction.UpdateBy = userID;
                        _context.tr_QC_UnitCheckList_Action.Update(existingAction);
                    }
                    else
                    {
                        var newAction = new tr_QC_UnitCheckList_Action
                        {
                            QCUnitCheckListID = checklistID,
                            RoleID = QC5ActionRoleID,
                            ActionType = QC5ActionType,
                            StatusID = null,
                            Remark = null,
                            ActionDate = row.QC5Date,
                            CreateDate = now,
                            CreateBy = userID,
                            UpdateDate = now,
                            UpdateBy = userID
                        };
                        _context.tr_QC_UnitCheckList_Action.Add(newAction);
                    }

                    commitResult.CheckListUpdated++;
                }

                if (!row.NeedSync || row.UnitID == null) continue;

                // Upsert tr_QC_Sync : ถ้ามีแถวของ Unit นี้ (QCTypeID = 17) อยู่แล้วให้อัปเดต ถ้าไม่มีให้เพิ่มใหม่
                if (existingSyncs.TryGetValue(row.UnitID.Value, out tr_QC_Sync? existingSync))
                {
                    existingSync.SyncType = QC5SyncType;
                    existingSync.QCAppointDate = row.QC5Date;
                    existingSync.QCAppointTimeFrom = QC5SyncAppointTime;
                    existingSync.QCAppointTimeTo = QC5SyncAppointTime;
                    existingSync.QCResponseUserID = null;
                    existingSync.QCResponseDate = row.QC5Date;
                    existingSync.QCRemark = null;
                    existingSync.SubmitDate = row.QC5Date;
                    _context.tr_QC_Sync.Update(existingSync);

                    commitResult.SyncUpdated++;
                }
                else
                {
                    var newSync = new tr_QC_Sync
                    {
                        ID = Guid.NewGuid(),
                        ProjectID = row.ProjectID,
                        UnitID = row.UnitID,
                        SyncType = QC5SyncType,
                        QCTypeID = QC5QCTypeID,
                        QCAppointDate = row.QC5Date,
                        QCAppointTimeFrom = QC5SyncAppointTime,
                        QCAppointTimeTo = QC5SyncAppointTime,
                        QCResponseUserID = null,
                        QCResponseDate = row.QC5Date,
                        QCRemark = null,
                        SubmitDate = row.QC5Date
                    };
                    _context.tr_QC_Sync.Add(newSync);

                    commitResult.SyncInserted++;
                }
            }

            // SaveChanges ครั้งเดียว = ทั้งชุดอยู่ใน transaction เดียว ถ้าพลาดจะ rollback ทั้งหมด
            _context.SaveChanges();

            return commitResult;
        }
    }
}
