using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;
using System.Linq;
using static Project.ConstructionTracking.Web.Models.FormCheckListModel;

public class FormChecklistRepo : IFormChecklistRepo
{
    private readonly ContructionTrackingDbContext _context;

    public FormChecklistRepo(ContructionTrackingDbContext context)
    {
        _context = context;
    }

    public List<FormCheckListModel.Form_getListPackages> GetFormCheckList(FormCheckListModel.Form_getFilterData filterData)
    {
        // เริ่มการสร้าง query โดยใช้ LINQ
        var query = from t1 in _context.tm_FormPackage
                        // เชื่อมกับ tm_FormCheckList โดยใช้ left join
                    join t2 in _context.tm_FormCheckList on t1.ID equals t2.PackageID into t2Join
                    from t2 in t2Join.DefaultIfEmpty()

                        // เชื่อมกับ tr_UnitFormCheckList โดยใช้ left join และจับคู่คีย์ที่ระบุ
                    join t4 in _context.tr_UnitFormCheckList on new { t1.GroupID, PackageID = t1.ID, CheckListID = t2.ID } equals new { t4.GroupID, t4.PackageID, t4.CheckListID } into t4Join
                    from t4 in t4Join.DefaultIfEmpty()

                        // เชื่อมกับ tr_UnitFormPackage โดยใช้ left join และจับคู่คีย์ที่ระบุ
                    join t5 in _context.tr_UnitFormPackage on new { t1.GroupID, PackageID = t1.ID } equals new { t5.GroupID, t5.PackageID } into t5Join
                    from t5 in t5Join.DefaultIfEmpty()

                        // กำหนดเงื่อนไขให้กรองข้อมูลที่ GroupID ตรงกับค่าใน filterData
                    where t1.GroupID == filterData.GroupID

                    // จัดเรียงข้อมูลตามลำดับที่กำหนด
                    orderby t1.Sort, t2.Sort

                    // เลือกข้อมูลที่ต้องการจาก query
                    select new { t1, t2, t4, t5 };

        // จัดกลุ่มข้อมูลที่ได้จาก query
        var result = query
            .GroupBy(g => new { g.t1.GroupID, g.t1.ID, g.t1.Name, Unit_PackagesID = g.t5.ID, g.t5.Remark })
            .Select(g => new FormCheckListModel.Form_getListPackages
            {
                UnitPackagesID = g.Key.Unit_PackagesID, // กำหนดค่า UnitPackagesID
                GroupID = g.Key.GroupID, // กำหนดค่า GroupID
                PackagesID = g.Key.ID, // กำหนดค่า PackageID
                PackagesName = g.Key.Name, // กำหนดชื่อ Package
                Remark = g.Key.Remark, // กำหนด Remark

                // สร้าง ListCheckLists โดยการเลือกข้อมูลจากแต่ละกลุ่ม
                ListCheckLists = g.GroupBy(c => new { c.t2.ID, c.t2.Name, Unit_CheckListID = c.t4.ID, c.t4.StatusID })
                                  .Select(cg => new FormCheckListModel.Form_getListCheckLists
                                  {
                                      UnitCheckListID = cg.Key.Unit_CheckListID, // กำหนดค่า UnitCheckListID
                                      PackageID = cg.First().t2.PackageID, // กำหนดค่า PackageID
                                      CheckListID = cg.Key.ID, // กำหนดค่า CheckListID
                                      CheckListName = cg.Key.Name, // กำหนดชื่อ CheckList
                                      StatusCheck = cg.Key.StatusID, // กำหนดค่า StatusCheck

                                      // สร้าง ListRadioCheck สำหรับแต่ละ CheckList
                                      ListRadioCheck = (from rc in _context.tr_RoleActionStatus
                                                        where rc.RoleID == 1
                                                        select new FormCheckListModel.Form_getRadioCheckLists
                                                        {
                                                            RadioCheck_ID = rc.ID, // กำหนดค่า RadioCheck_ID
                                                            RadioCheck_Name = rc.Name // กำหนดชื่อ RadioCheck
                                                        }).ToList()
                                  }).ToList()
            }).ToList();

        // ส่งคืนผลลัพธ์
        return result;
    }

    public List<FormCheckListModel.Form_getListStatus> GetFormCheckListStatus(FormCheckListModel.Form_getFilterData filterData)
    {
        var unitForms = _context.tr_UnitForm
            .Where(t1 => (!filterData.ProjectID.HasValue || t1.ProjectID == filterData.ProjectID) &&
                         (!filterData.UnitID.HasValue || t1.UnitID == filterData.UnitID) &&
                         (!filterData.FormID.HasValue || t1.FormID == filterData.FormID))
            .OrderBy(t1 => t1.FormID)
            .ToList();

        var unitFormIds = unitForms.Select(t1 => t1.ID).ToList();
        var unitFormActions = _context.tr_UnitFormAction
            .Where(t2 => unitFormIds.Contains(t2.UnitFormID.Value) && t2.RoleID == 1)
            .ToList();

        var result = unitForms
            .Select(t1 => new
            {
                t1.ID,
                t1.ProjectID,
                t1.UnitID,
                t1.FormID,
                UnitFormAction = unitFormActions.FirstOrDefault(t2 => t2.UnitFormID == t1.ID)
            })
            .Select(x => new FormCheckListModel.Form_getListStatus
            {
                ID = x.ID,
                ProjectID = x.ProjectID,
                UnitID = x.UnitID,
                UnitFormActionID = x.UnitFormAction?.ID,
                FormID = x.FormID,
                RoleID = x.UnitFormAction?.RoleID,
                ActionType = x.UnitFormAction?.ActionType,
                UpdateDate = x.UnitFormAction?.UpdateDate.HasValue ?? false
                    ? FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(x.UnitFormAction.UpdateDate.Value.ToString("dd/MM/yyyy"))
                    : ""
            })
            .ToList();

        return result;
    }


    public void InsertOrUpdate(FormChecklistIUDModel model)
    {
        var UnitFormIDUse = Guid.Empty;
        var package = model.Packages.FirstOrDefault();

        if (package != null)
        {
            UnitFormIDUse = InsertOrUpdateUnitForm(package);
            InsertOrUpdateUnitFormAction(package, UnitFormIDUse);
            InsertOrUpdateUnitFormPackage(model.Packages, UnitFormIDUse);
            InsertOrUpdateUnitFormCheckList(model.CheckLists, UnitFormIDUse);
        }

        _context.SaveChanges();
    }

    private Guid InsertOrUpdateUnitForm(PackageModel package)
    {
        var UnitFormIDUse = Guid.Empty;
        var form = _context.tm_Form.Find(package.FormID);
        if (form != null)
        {
            var unitForm = _context.tr_UnitForm
                .Where(uf => uf.ID == package.UnitFormID && uf.FormID == package.FormID && uf.UnitID == package.UnitId)
                .FirstOrDefault();

            if (package.UnitFormID == Guid.Empty)
            {
                unitForm = new tr_UnitForm
                {
                    ID = Guid.NewGuid(),
                    ProjectID = package.ProjectId,
                    UnitID = package.UnitId,
                    FormID = form.ID,
                    Progress = form.Progress,
                    Duration = form.DurationDay,
                    StatusID = 1,
                    FlagActive = true,
                    CreateDate = DateTime.Now,
                    CreateBy = package.UserID,
                    UpdateDate = DateTime.Now,
                    UpdateBy = package.UserID
                };
                _context.tr_UnitForm.Add(unitForm);
                UnitFormIDUse = unitForm.ID;
            }
            else
            {
                unitForm.StatusID = 1;
                unitForm.UpdateDate = DateTime.Now;
                unitForm.UpdateBy = package.UserID;
                UnitFormIDUse = unitForm.ID;
            }
            _context.SaveChanges();
        }
        return UnitFormIDUse;
    }

    private void InsertOrUpdateUnitFormAction(PackageModel package, Guid unitFormIDUse)
    {
        var unitFormAction = _context.tr_UnitFormAction
        .Where(uf => uf.ID == package.UnitFormActionID && uf.UnitFormID == package.UnitFormID)
        .FirstOrDefault();

            if (unitFormAction == null)
            {
                unitFormAction = new tr_UnitFormAction
                {
                    UnitFormID = unitFormIDUse,
                    RoleID = 1,
                    ActionType = "save",
                    StatusID = 1,
                    Remark = package.Remark,
                    ActionDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CraeteDate = DateTime.Now,
                };
                _context.tr_UnitFormAction.Add(unitFormAction);
            }
            else
            {
                unitFormAction.ActionDate = DateTime.Now;
                unitFormAction.UpdateDate = DateTime.Now;
            }

            _context.SaveChanges();
    }

    private void InsertOrUpdateUnitFormPackage(List<PackageModel> packages, Guid unitFormIDUse)
    {
        foreach (var packagelist in packages)
        {
            var existingUnitFormPackage = _context.tr_UnitFormPackage
                .Where(ufp => ufp.ID == packagelist.UnitPackageID && ufp.UnitFormID == unitFormIDUse)
                .FirstOrDefault();

            if (existingUnitFormPackage == null)
            {
                var newUnitFormPackage = new tr_UnitFormPackage
                {
                    UnitFormID = unitFormIDUse,
                    FormID = packagelist.FormID,
                    GroupID = packagelist.GroupID,
                    PackageID = packagelist.PackageID,
                    Remark = packagelist.Remark,
                    UpdateDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                };
                _context.tr_UnitFormPackage.Add(newUnitFormPackage);

                //InsertLog(newUnitFormPackage.UnitFormID, newUnitFormPackage.GroupID, null, "Insert", null);
            }
            else
            {
                existingUnitFormPackage.Remark = packagelist.Remark;
                existingUnitFormPackage.UpdateDate = DateTime.Now;

                //InsertLog(existingUnitFormPackage.UnitFormID, existingUnitFormPackage.GroupID, null, "Update", null);
            }
        }

        _context.SaveChanges();
    }

    private void InsertOrUpdateUnitFormCheckList(List<ChecklistModel> checkLists, Guid unitFormIDUse)
    {
        foreach (var checklist in checkLists)
        {
            // ตรวจสอบว่าถ้า RadioValue เป็น 0 ให้ตั้งค่าเป็น null
            int? statusID = checklist.RadioValue == 0 ? (int?)null : checklist.RadioValue;

            // ค้นหา entity ที่มีอยู่แล้วจากฐานข้อมูลโดยใช้ UnitChecklistID และ UnitFormID
            var existingUnitFormCheckList = _context.tr_UnitFormCheckList
                .Where(ufc => ufc.ID == checklist.UnitChecklistID && ufc.UnitFormID == unitFormIDUse)
                .FirstOrDefault();

            if (existingUnitFormCheckList == null) // ถ้าไม่พบ entity ที่มีอยู่แล้ว
            {
                var newUnitFormCheckList = new tr_UnitFormCheckList
                {
                    UnitFormID = unitFormIDUse,
                    FormID = checklist.FormID,
                    GroupID = checklist.GroupID,
                    PackageID = checklist.PackageID,
                    CheckListID = checklist.CheckListID,
                    StatusID = statusID,
                    UpdateDate = DateTime.Now,
                    CreateDate = DateTime.Now
                };

                _context.tr_UnitFormCheckList.Add(newUnitFormCheckList); // เพิ่ม entity ใหม่
            }
            else // ถ้าพบ entity ที่มีอยู่แล้ว
            {
                existingUnitFormCheckList.StatusID = statusID; // อัปเดต StatusID
                existingUnitFormCheckList.UpdateDate = DateTime.Now; // อัปเดตวันที่
            }
        }

        _context.SaveChanges(); // บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล
    }


}
