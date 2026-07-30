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

            var unitIDs = unitDict.Values.ToList();

            var existingQC5UnitIDs = _context.tr_QC_UnitCheckList
                .Where(o => o.UnitID != null && unitIDs.Contains(o.UnitID.Value)
                         && o.QCTypeID == QC5QCTypeID && o.Seq == QC5Seq && o.FlagActive == true)
                .Select(o => o.UnitID!.Value)
                .ToList()
                .ToHashSet();

            var seenInFile = new HashSet<Guid>();

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

                if (existingQC5UnitIDs.Contains(unitID))
                {
                    row.Status = ImportQC5RowStatus.Duplicate;
                    row.Message = "Unit นี้มีข้อมูล QC5 ในระบบแล้ว (ข้ามการนำเข้า)";
                    continue;
                }

                if (!seenInFile.Add(unitID))
                {
                    row.Status = ImportQC5RowStatus.Duplicate;
                    row.Message = "ข้อมูล Unit นี้ซ้ำในไฟล์ Excel (ข้ามการนำเข้า)";
                    continue;
                }

                row.Status = ImportQC5RowStatus.Valid;
                row.Message = "พร้อมนำเข้า";
            }
        }

        public int ImportRows(List<ImportQC5RowModel> validRows, Guid userID)
        {
            DateTime now = DateTime.Now;

            foreach (var row in validRows)
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
            }

            // SaveChanges ครั้งเดียว = ทั้งชุดอยู่ใน transaction เดียว ถ้าพลาดจะ rollback ทั้งหมด
            _context.SaveChanges();

            return validRows.Count;
        }
    }
}
