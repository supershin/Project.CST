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

    public FormCheckListModel.Form_getUnitFormData GetUnitFormData(FormCheckListModel.Form_getUnitFormData filterData)
    {
        var result = (from t1 in _context.tm_Project
                      join t2 in _context.tm_Unit on t1.ProjectID equals t2.ProjectID into units
                      from unit in units.DefaultIfEmpty()
                      join t3 in _context.tr_ProjectModelForm on t1.ProjectID equals t3.ProjectID into projectModelForms
                      from projectModelForm in projectModelForms.DefaultIfEmpty()
                      join t4 in _context.tm_Form on projectModelForm.FormTypeID equals t4.FormTypeID into forms
                      from form in forms.DefaultIfEmpty()
                      join t5 in _context.tm_FormGroup on form.ID equals t5.FormID into formGroups
                      from formGroup in formGroups.DefaultIfEmpty()
                      join t6 in _context.tr_UnitForm on new { ProjectID = (Guid?)t1.ProjectID, UnitID = (Guid?)unit.UnitID, FormID = (int?)form.ID } equals new { t6.ProjectID, t6.UnitID, t6.FormID } into unitForms
                      from unitForm in unitForms.DefaultIfEmpty()
                      where unit.UnitID == filterData.UnitID && form.ID == filterData.FormID && formGroup.ID == filterData.GroupID
                      select new FormCheckListModel.Form_getUnitFormData
                      {
                          ProjectID = t1.ProjectID,
                          ProjectName = t1.ProjectName,
                          UnitID = unit.UnitID,
                          UnitCode = unit.UnitCode,
                          FormID = form.ID,
                          FormName = form.Name,
                          GroupID = formGroup.ID,
                          GroupName = formGroup.Name,
                          UnitFormID = unitForm.ID,
                          UnitFormStatusID = unitForm.StatusID
                      }).FirstOrDefault();


        return result;
    }

    public List<FormCheckListModel.Form_getListPackages> GetFormCheckList(FormCheckListModel.Form_getFilterData filterData)
    {
        // เริ่มการสร้าง query โดยใช้ LINQ
        var query = from t1 in _context.tm_FormPackage
                        // เชื่อมกับ tm_FormCheckList โดยใช้ left join
                    join t2 in _context.tm_FormCheckList.Where(f => f.FlagActive == true) on t1.ID equals t2.PackageID into t2Join
                    from t2 in t2Join.DefaultIfEmpty()

                        // เชื่อมกับ tr_UnitFormCheckList โดยใช้ left join และจับคู่คีย์ที่ระบุ
                    join t4 in _context.tr_UnitFormCheckList.Where(f => f.FlagActive == true) on new { t1.GroupID, PackageID = (int?)t1.ID, CheckListID = (int?)t2.ID, filterData.UnitFormID } equals new { t4.GroupID, t4.PackageID, t4.CheckListID, t4.UnitFormID } into t4Join
                    from t4 in t4Join.DefaultIfEmpty()

                        // เชื่อมกับ tr_UnitFormPackage โดยใช้ left join และจับคู่คีย์ที่ระบุ
                    join t5 in _context.tr_UnitFormPackage on new { t1.GroupID, PackageID = (int?)t1.ID, filterData.UnitFormID } equals new { t5.GroupID, t5.PackageID, t5.UnitFormID } into t5Join
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
                                      ListRadioCheck = (from rc in _context.tm_Ext
                                                        where rc.ExtTypeID == 6
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
        var unitForms = _context.tr_UnitForm.Where(f => f.FlagActive == true)
            .Where(t1 => (!filterData.ProjectID.HasValue || t1.ProjectID == filterData.ProjectID) &&
                         (!filterData.UnitID.HasValue || t1.UnitID == filterData.UnitID) &&
                         (!filterData.FormID.HasValue || t1.FormID == filterData.FormID))
            .OrderBy(t1 => t1.FormID)
            .ToList();

        var unitFormIds = unitForms.Select(t1 => t1.ID).ToList();

        var unitFormActions = _context.tr_UnitFormAction
            .Where(t2 => unitFormIds.Contains(t2.UnitFormID.Value) && t2.RoleID == 1)
            .ToList();

        var unitFormActionsPM = _context.tr_UnitFormAction
            .Where(t2 => unitFormIds.Contains(t2.UnitFormID.Value) && t2.RoleID == 2)
            .ToList();

        var unitFormActionsPJM = _context.tr_UnitFormAction
            .Where(t2 => unitFormIds.Contains(t2.UnitFormID.Value) && t2.RoleID == 3)
            .ToList();

        var unitFormPassConditions = _context.tr_UnitFormPassCondition
            .Where(t3 => unitFormIds.Contains(t3.UnitFormID.Value) && t3.FlagActive == true && t3.GroupID == filterData.GroupID)
            .ToList();

        // Join tr_UnitFormPassCondition, tr_UnitFormResource, and tm_Resource to get the image data
        var unitFormImages = (from formResource in _context.tr_UnitFormResource
                              join resource in _context.tm_Resource on formResource.ResourceID equals resource.ID
                              where unitFormIds.Contains(formResource.UnitFormID.Value) && formResource.FlagActive == true && formResource.GroupID == filterData.GroupID
                              select new
                              {
                                  formResource.UnitFormID,
                                  ResourceID = resource.ID,
                                  FileName = resource.FileName,
                                  FilePath = resource.FilePath
                              }).ToList();

        var result = unitForms
            .Select(t1 => new
            {
                t1.ID,
                t1.ProjectID,
                t1.UnitID,
                t1.FormID,
                UnitFormAction = unitFormActions.FirstOrDefault(t2 => t2.UnitFormID == t1.ID),
                UnitFormActionPM = unitFormActionsPM.FirstOrDefault(t2 => t2.UnitFormID == t1.ID),
                UnitFormActionPJM = unitFormActionsPJM.FirstOrDefault(t2 => t2.UnitFormID == t1.ID),
                UnitFormPassCondition = unitFormPassConditions.FirstOrDefault(t3 => t3.UnitFormID == t1.ID),
                Form_getListImagePasswithCondition = unitFormImages
                    .Where(img => img.UnitFormID == t1.ID)
                    .Select(img => new FormCheckListModel.Form_getListImagePasswithCondition
                    {
                        ResourceID = img.ResourceID,
                        FileName = img.FileName,
                        FilePath = img.FilePath
                    }).ToList()
            })
            .Select(x => new FormCheckListModel.Form_getListStatus
            {
                ID = x.ID,
                ProjectID = x.ProjectID,
                UnitID = x.UnitID,
                UnitFormActionID = x.UnitFormAction?.ID,
                PM_StatusID = x.UnitFormActionPM?.StatusID,
                PM_ActionType = x.UnitFormActionPM?.ActionType,
                PJM_StatusID = x.UnitFormActionPJM?.StatusID,
                PJM_ActionType = x.UnitFormActionPJM?.ActionType,
                FormID = x.FormID,
                LockStatusID = x.UnitFormPassCondition?.LockStatusID,
                RemarkPassCondition = x.UnitFormPassCondition?.PE_Remark,
                RoleID = x.UnitFormAction?.RoleID,
                ActionType = x.UnitFormAction?.ActionType,
                UpdateDate = x.UnitFormAction?.UpdateDate.HasValue ?? false
                    ? FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(x.UnitFormAction.UpdateDate.Value.ToString("dd/MM/yyyy"))
                    : "",
                Form_getListImagePasswithCondition = x.Form_getListImagePasswithCondition // Ensure this maps correctly
            })
            .ToList();


        return result;
    }

    public void InsertOrUpdate(FormChecklistIUDModel model, Guid? userID, int RoleID)
    {
        var UnitFormIDUse = Guid.Empty;
        var package = model.Packages.FirstOrDefault();
        var pcCheck = model.PcCheck;

        if (package != null)
        {
            UnitFormIDUse = InsertOrUpdateUnitForm(package);
            InsertOrUpdateUnitFormAction(package, UnitFormIDUse);
            InsertOrUpdateUnitFormPackage(model.Packages, UnitFormIDUse);
            InsertOrUpdateUnitFormCheckList(model.CheckLists, UnitFormIDUse);
            InsertImagesPE(model, package.FormID, package.GroupID, UnitFormIDUse, userID, RoleID); // Insert images if there are any
        }
        if (pcCheck != null)
        {
            InsertOrUpdatePassConditionCheck(pcCheck, UnitFormIDUse);
            
        }
        else if (pcCheck == null)
        {
            InsertOrUpdatePassConditionUnCheck(package, UnitFormIDUse);
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
                    //CreateBy = package.UserID,
                    UpdateDate = DateTime.Now,
                    //UpdateBy = package.UserID
                };
                _context.tr_UnitForm.Add(unitForm);
                //UnitFormIDUse = unitForm.ID;
            }
            //else
            //{
            //    unitForm.StatusID = 18;
            //    unitForm.UpdateDate = DateTime.Now;
            //    unitForm.UpdateBy = package.UserID;
            //    UnitFormIDUse = unitForm.ID;
            //}

            UnitFormIDUse = unitForm.ID;
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
                //Remark = package.Remark,
                ActionDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CraeteDate = DateTime.Now,
            };
            _context.tr_UnitFormAction.Add(unitFormAction);
            InsertUnitFormActionLog(unitFormAction , package.GroupID,"Insert New");
        }
        else
        {
            unitFormAction.ActionDate = DateTime.Now;
            unitFormAction.UpdateDate = DateTime.Now;
            InsertUnitFormActionLog(unitFormAction, package.GroupID, "Update");
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

    private void InsertOrUpdatePassConditionCheck(PassConditionCheckModel pcCheck, Guid unitFormIDUse)
    {
        var passCondition = _context.tr_UnitFormPassCondition
            .Where(pc => pc.UnitFormID == unitFormIDUse && pc.GroupID == pcCheck.GroupID)
            .FirstOrDefault();

        if (passCondition == null)
        {
            // Insert new record
            passCondition = new tr_UnitFormPassCondition
            {
                UnitFormID = unitFormIDUse,
                GroupID = pcCheck.GroupID,
                LockStatusID = 7,
                PE_Remark = pcCheck.Remark,
                FlagActive = true,
                ActionDate = DateTime.Now,
                CraeteDate = DateTime.Now,
                //CreateBy = pcCheck.UserID,
                UpdateDate = DateTime.Now,
                //UpdateBy = pcCheck.UserID
            };
            _context.tr_UnitFormPassCondition.Add(passCondition);
        }
        else
        {
            // Update existing record
            //passCondition.GroupID = pcCheck.GroupID;
            //passCondition.LockStatusID = 1;
            passCondition.PE_Remark = pcCheck.Remark;
            passCondition.FlagActive = true;
            passCondition.ActionDate = DateTime.Now;
            passCondition.UpdateDate = DateTime.Now;
            //passCondition.UpdateBy = pcCheck.UserID;
            InsertUnitFormActionLogPassCondition(passCondition, "Checked");
        }
        _context.SaveChanges();


        //var passConditionID = passCondition.ID;

        //// Update tr_UnitFormAction 
        //var unitFormAction = _context.tr_UnitFormAction
        //    .Where(ua => ua.UnitFormID == unitFormIDUse)
        //    .FirstOrDefault();

        //if (unitFormAction != null)
        //{
        //    // Update existing record
        //    //unitFormAction.PassConditionID = passConditionID;
        //    unitFormAction.ActionDate = DateTime.Now;
        //    unitFormAction.UpdateDate = DateTime.Now;
        //    //unitFormAction.Remark = pcCheck.Remark; 

        //    _context.SaveChanges();
        //}
    }

    private void InsertOrUpdatePassConditionUnCheck(PackageModel pcCheck, Guid unitFormIDUse)
    {
        var passCondition = _context.tr_UnitFormPassCondition
            .Where(pc => pc.UnitFormID == unitFormIDUse && pc.GroupID == pcCheck.GroupID)
            .FirstOrDefault();

        if (passCondition != null)
        {
            if (passCondition.FlagActive == true)
            {
                //passCondition.GroupID = pcCheck.GroupID;
                //passCondition.LockStatusID = 1;
                passCondition.FlagActive = false;
                passCondition.ActionDate = DateTime.Now;
                passCondition.UpdateDate = DateTime.Now;
                InsertUnitFormActionLogPassCondition(passCondition, "UnChecked");
            }
        }       
        _context.SaveChanges();

        // Update tr_UnitFormAction 
        //var unitFormAction = _context.tr_UnitFormAction
        //    .Where(ua => ua.UnitFormID == unitFormIDUse)
        //    .FirstOrDefault();

        //if (unitFormAction != null)
        //{
        //    // Update existing record
        //    //unitFormAction.PassConditionID = null;
        //    unitFormAction.UpdateDate = DateTime.Now;
        //    //unitFormAction.Remark = null;

        //    _context.SaveChanges();
        //}
    }

    private void InsertImagesPE(FormChecklistIUDModel model, int formID , int groupID ,Guid unitFormIDUse, Guid? userID, int RoleID)
    {
        if (model.Images != null && model.Images.Count > 0)
        {
            var folder = DateTime.Now.ToString("yyyyMM");
            var dirPath = Path.Combine(model.ApplicationPath, "wwwroot", "Upload", "document", folder, "PEImage");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var passCondition = _context.tr_UnitFormPassCondition
                .Where(pc => pc.UnitFormID == unitFormIDUse && pc.GroupID == groupID)
                .FirstOrDefault();

            foreach (var image in model.Images)
            {
                if (image.Length > 0)
                {
                    Guid guidId = Guid.NewGuid(); // Generate a new Guid for the file
                    string fileName = guidId + ".jpg"; // Set the file name with .jpg extension
                    var filePath = Path.Combine(dirPath, fileName); // Determine the full file path

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }

                    // Prepare the relative file path and replace backslashes with forward slashes
                    string relativeFilePath = Path.Combine("Upload", "document", folder, "PEImage", fileName).Replace("\\", "/");

                    // Save the image details in the tm_Resource table
                    var newResource = new tm_Resource
                    {
                        ID = Guid.NewGuid(),
                        FileName = fileName,
                        FilePath = relativeFilePath, // Store the relative path with forward slashes
                        MimeType = "image/jpeg", // Ensure the MimeType is set to "image/jpeg"
                        FlagActive = true,
                        CreateDate = DateTime.Now,
                        //CreateBy = userID.HasValue ? userID.Value.GetHashCode() : (int?)null, // Uncomment and adjust as needed
                        UpdateDate = DateTime.Now,
                        //UpdateBy = userID.HasValue ? userID.Value.GetHashCode() : (int?)null, // Uncomment and adjust as needed
                    };
                    _context.tm_Resource.Add(newResource);

                    // Link this resource to the pass condition, if necessary
                    var newFormResource = new tr_UnitFormResource
                    {
                        GroupID = groupID,
                        PassConditionID = passCondition?.ID, // Link to the pass condition, if available
                        RoleID = RoleID,
                        UnitFormID = unitFormIDUse,
                        ResourceID = newResource.ID,
                        CreateDate = DateTime.Now,
                        //CreateBy = userID.HasValue ? userID.Value.GetHashCode() : (int?)null, // Uncomment and adjust as needed
                    };
                    _context.tr_UnitFormResource.Add(newFormResource);
                }
            }

            _context.SaveChanges();
        }
    }

    public bool DeleteImage(Guid resourceId, string applicationPath)
    {
        // Retrieve the resource record from the database
        var resource = _context.tm_Resource.FirstOrDefault(r => r.ID == resourceId);
        if (resource == null)
        {
            return false;
        }

        // If resource.FilePath already contains the directory and filename, you can directly combine it
        // with the application path:
        var fullFilePath = Path.Combine(applicationPath ,"wwwroot", resource.FilePath);

        // Check if the file exists and delete it
        if (File.Exists(fullFilePath))
        {
            File.Delete(fullFilePath);
        }
        else
        {
            // Log or handle the case where the file was not found
            return false; // File not found, deletion failed
        }

        // Remove any related entries in the tr_UnitFormResource table
        var relatedEntries = _context.tr_UnitFormResource.Where(ufr => ufr.ResourceID == resourceId).ToList();
        _context.tr_UnitFormResource.RemoveRange(relatedEntries);

        // Remove the resource record from the database
        _context.tm_Resource.Remove(resource);

        // Save changes to the database
        _context.SaveChanges();

        return true;
    }

    private void InsertUnitFormActionLog(tr_UnitFormAction unitFormAction , int group_ID , string Action)
    {
        var actionLog = new tr_UnitFormActionLog
        {
            UnitFormID = unitFormAction.UnitFormID,
            RoleID = 1,
            GroupID = group_ID,
            StatusID = 1,
            Remark = "PE/"+ Action + "/UnitFormAction",
            ActionDate = DateTime.Now,
            CraeteDate = DateTime.Now,
            // CreateBy = unitFormAction.CreateBy, // Uncomment and set appropriately if you have the CreateBy field
        };

        _context.tr_UnitFormActionLog.Add(actionLog);
        _context.SaveChanges();
    }

    private void InsertUnitFormActionLogPassCondition(tr_UnitFormPassCondition UnitFormPassCondition , string PcCheck)
    {
        var pcvalue = PcCheck == "Checked" ? "ให้ผ่านแบบมีเงื่อนไขเพื่อส่งPM" : "ยกเลิกผ่านแบบมีเงื่อนไขเ";

        var actionLog = new tr_UnitFormActionLog
        {
            UnitFormID = UnitFormPassCondition.UnitFormID,
            GroupID = UnitFormPassCondition.GroupID,
            RoleID = 1,
            //StatusID = UnitFormPassCondition.StatusID,
            Remark = "PE/" + pcvalue + "/UnitFormPassCondition/"+"PCID = " + UnitFormPassCondition.ID,
            ActionDate = DateTime.Now,
            CraeteDate = DateTime.Now,
            // CreateBy = unitFormAction.CreateBy, // Uncomment and set appropriately if you have the CreateBy field
        };

        _context.tr_UnitFormActionLog.Add(actionLog);
        _context.SaveChanges();
    }
}
