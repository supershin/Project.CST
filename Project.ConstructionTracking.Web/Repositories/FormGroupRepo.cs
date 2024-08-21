using Humanizer.Localisation;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Project.ConstructionTracking.Web.Models.FormGroupModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class FormGroupRepo : IFormGroupRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public FormGroupRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<FormGroupModel> GetFormGroupList(FormGroupModel model)
        {
            var query = from t1 in _context.tm_FormGroup.Where(g => g.FlagActive == true)
                        where t1.FormID == model.FormID
                        join t2 in _context.tm_FormPackage.Where(p => p.FlagActive == true) on t1.ID equals t2.GroupID into t2Group
                        from t2 in t2Group.DefaultIfEmpty()
                        join t3 in _context.tm_FormCheckList.Where(c => c.FlagActive == true) on t2.ID equals t3.PackageID into t3Group
                        from t3 in t3Group.DefaultIfEmpty()
                        join t4 in _context.tr_UnitFormCheckList.Where(f => f.FlagActive == true) on new { t1.FormID, PackageID = (int?)t2.ID, CheckListID = (int?)t3.ID, model.UnitFormID } equals new { t4.FormID, t4.PackageID, t4.CheckListID, t4.UnitFormID } into t4Group
                        from t4 in t4Group.Where(t => t.StatusID == 1 || t.StatusID == 3).DefaultIfEmpty()
                        join t5 in _context.tr_UnitFormCheckList.Where(f => f.FlagActive == true) on new { t1.FormID, PackageID = (int?)t2.ID, CheckListID = (int?)t3.ID, model.UnitFormID } equals new { t5.FormID, t5.PackageID, t5.CheckListID, t5.UnitFormID } into t5Group
                        from t5 in t5Group.Where(t => t.StatusID == 2).DefaultIfEmpty()
                        join t4All in _context.tr_UnitFormCheckList.Where(f => f.FlagActive == true) on new { t1.FormID, PackageID = (int?)t2.ID, CheckListID = (int?)t3.ID, model.UnitFormID } equals new { t4All.FormID, t4All.PackageID, t4All.CheckListID, t4All.UnitFormID } into t4AllGroup
                        from t4All in t4AllGroup.Where(t => t.StatusID != null).DefaultIfEmpty()
                        join t6 in _context.tr_UnitFormPassCondition.Where(p => p.FlagActive == true) on new { t4.UnitFormID, t4.GroupID } equals new { t6.UnitFormID, t6.GroupID } into t6Group
                        from t6 in t6Group.DefaultIfEmpty()
                        join t7 in _context.tr_UnitFormAction on new { t4.UnitFormID, RoleID = (int?)2 } equals new { t7.UnitFormID, t7.RoleID } into t7Group
                        from t7 in t7Group.DefaultIfEmpty()
                        group new { t3, t4, t4All, t5, t6 ,t7 } by new { t1.ID, t1.FormID, t1.Name, t6.LockStatusID, t6.PE_Remark, t6.PM_Remark, t6.PJM_Remark, t7.StatusID} into g
                        select new FormGroupModel
                        {
                            GroupID = g.Key.ID,
                            FormID = g.Key.FormID,
                            GroupName = g.Key.Name,
                            LockStatusID = g.Key.LockStatusID,
                            PC_Remark_PE = g.Key.PE_Remark,
                            PC_Remark_PM = g.Key.PM_Remark,
                            PC_Remark_PJM = g.Key.PJM_Remark,
                            StatusID = g.Key.StatusID,
                            Cnt_CheckList_All = g.Count(x => x.t3 != null),
                            Cnt_CheckList_Pass = g.Count(x => x.t4 != null),
                            Cnt_CheckList_NotPass = g.Count(x => x.t5 != null),
                            StatusUse = g.Count(x => x.t5 != null) > 0 ? "danger" :
                                        g.Count(x => x.t4All != null) == 0 ? "secondary" :
                                        g.Count(x => x.t4 != null) == g.Count(x => x.t3 != null) ? "success" :
                                        g.Count(x => x.t4All != null) > 0 && g.Count(x => x.t3 != null) > 0 && g.Count(x => x.t5 != null) == 0 ? "warning" : ""
                        };

            var orderedQuery = query.OrderBy(fg => fg.GroupID).ToList();
            return orderedQuery;
        }
        public FormGroupDetail GetFormGroupDetail(Guid unitFormId)
        {
            // Step 1: Fetch the UnitForm and related actions (PE, PM, and PJM)
            var result = (from t1 in _context.tr_UnitForm
                          where t1.ID == unitFormId
                          join t2 in _context.tr_UnitFormAction.Where(a => a.RoleID == 1)
                              on t1.ID equals t2.UnitFormID into peActions
                          from peAction in peActions.DefaultIfEmpty()
                          join t3 in _context.tr_UnitFormAction.Where(a => a.RoleID == 2)
                              on t1.ID equals t3.UnitFormID into pmActions
                          from pmAction in pmActions.DefaultIfEmpty()
                          join t4 in _context.tr_UnitFormAction.Where(a => a.RoleID == 3)
                              on t1.ID equals t4.UnitFormID into pjmActions
                          from pjmAction in pjmActions.DefaultIfEmpty()
                          join t5 in _context.tm_Vendor.Where(a => a.FlagActive == true)
                               on t1.VendorID equals t5.ID into venderNs
                          from venderN in venderNs.DefaultIfEmpty()
                          select new
                          {
                              t1.ID,
                              t1.Grade,
                              t1.FormID,
                              venderName = venderN.Name,
                              UnitFormstatus = t1.StatusID,
                              PE_ActionType = peAction != null ? peAction.ActionType : null,
                              PE_StatusID = peAction != null ? peAction.StatusID : null,
                              PM_ActionType = pmAction != null ? pmAction.ActionType : null,
                              PM_StatusID = pmAction != null ? pmAction.StatusID : null,
                              PM_Remark = pmAction != null ? pmAction.Remark : null,
                              PJM_ActionType = pjmAction != null ? pjmAction.ActionType : null,
                              PJM_StatusID = pjmAction != null ? pjmAction.StatusID : null
                          }).FirstOrDefault();

            if (result == null)
                return null;

            // Step 2: Fetch the latest UnitFormResource and related Resource
            var latestResource = (from ure in _context.tr_UnitFormResource
                                  where ure.UnitFormID == result.ID && ure.PassConditionID == null && ure.RoleID == 1 && ure.FormID == result.FormID
                                  orderby ure.CreateDate descending
                                  select new
                                  {
                                      ure.ResourceID,
                                      ure.CreateDate
                                  }).FirstOrDefault();

            string filePath = null;
            string fileName = null;

            if (latestResource != null)
            {
                var resource = _context.tm_Resource
                    .Where(r => r.ID == latestResource.ResourceID)
                    .Select(r => new { r.FilePath, r.FileName })
                    .FirstOrDefault();

                if (resource != null)
                {
                    filePath = resource.FilePath;
                    fileName = resource.FileName;
                }
            }

            // Step 3: Combine results into FormGroupDetail
            return new FormGroupDetail
            {
                ID = result.ID,
                Grade = result.Grade,
                FormID = result.FormID,
                VenderName = result.venderName,
                UnitFormstatusID = result.UnitFormstatus,
                UnitFormstatus = result.UnitFormstatus switch
                {
                    1 => "บันทึกร่าง",
                    2 => "ส่งเรื่องขอ PM อนุมัติ",
                    3 => "PM กำลังตรวจสอบ",
                    4 => "PM อนุมัติคำขอ",
                    5 => "PM ไม่อนุมัติคำขอ",
                    6 => "PM ส่งเรื่องอนุมัติการผ่านแบบมีเงื่อนไขให้ PJM พิจารณา",
                    7 => "PJM อนุมัติคำขอ",
                    8 => "PJM ไม่อนุมัติคำขอ",
                    _ => "สถานะไม่ทราบ"
                },
                PE_ActionType = result.PE_ActionType,
                PE_StatusID = result.PE_StatusID,
                PM_ActionType = result.PM_ActionType,
                PM_StatusID = result.PM_StatusID,
                PM_Remark = result.PM_Remark,
                PJM_ActionType = result.PJM_ActionType,
                PJM_StatusID = result.PJM_StatusID,
                FilePath = filePath,
                FileName = fileName
            };
        }


        public void SubmitSaveFormGroup(FormGroupModel.FormGroupIUDModel model)
        {

            var unitForm = _context.tr_UnitForm.Where(uf => uf.ID == model.UnitFormID).FirstOrDefault();

            if (unitForm != null)
            {
                unitForm.Grade = model.FormGrade;
                unitForm.VendorID = model.VendorID ?? unitForm.VendorID;
                unitForm.StatusID = model.Act == "submit" ? 2 : 1;
                unitForm.UpdateDate = DateTime.Now;
                //unitForm.UpdateBy = userID;
            }

            var unitFormAction = _context.tr_UnitFormAction
            .Where(uf => uf.UnitFormID == model.UnitFormID && uf.RoleID == 1)
            .FirstOrDefault();

            if (unitFormAction != null)
            {
                unitFormAction.ActionType = model.Act;
                unitFormAction.UpdateDate = DateTime.Now;
                //unitFormAction.UpdateBy = userID;
            }

            if (model.Act == "save")
            {               
                if (model.Sign != null)
                {
                    SaveSignature(model.Sign, model.ApplicationPath, model.UnitFormID, model.FormGrade, model.VendorID, model.userID, model.RoleID, model.FormID, "save");
                }
                InsertUnitFormActionLog(model.UnitFormID, "save");
            }
            else
            {
                if (model.Sign != null)
                {
                    SaveSignature(model.Sign, model.ApplicationPath, model.UnitFormID, model.FormGrade, model.VendorID, model.userID, model.RoleID, model.FormID,"submit");
                }
                InsertUnitFormActionLog(model.UnitFormID, "submit");
            }

            _context.SaveChanges();
        }
        private void SaveSignature(SignatureData signData, string? appPath, Guid? UnitFormID, string? FormGrade, int? VendorID, Guid? userID, int? RoleID ,int? FormID ,string? ActionType)
        {
            var resource = new FormGroupModel.Resources
            {
                MimeType = signData.MimeType,
                ResourceStorageBase64 = signData.StorageBase64
            };

            Guid guidId = Guid.NewGuid(); // Generate a new Guid for the file
            string fileName = guidId + ".jpg"; // Set the file name with .jpg extension
            var folder = DateTime.Now.ToString("yyyyMM");
            var dirPath = Path.Combine(appPath, "wwwroot", "Upload", "document", folder, "sign"); // Ensure path is within wwwroot
            var filePath = Path.Combine(dirPath, fileName); // Determine the full file path

            resource.PhysicalPathServer = dirPath;
            resource.ResourceStoragePath = Path.Combine("Upload", "document", folder, "sign", fileName).Replace("\\", "/"); // Store as a relative path with forward slashes

            ConvertByteToImage(resource);
            InsertResource(guidId, fileName, resource.ResourceStoragePath, "image/jpeg", userID);
            InsertOrUpdateUnitFormResource(guidId, UnitFormID, userID, RoleID , FormID);
            SubmitUpdateVendorUnitForm(guidId, UnitFormID, FormGrade, VendorID, userID, RoleID , ActionType);
            if (ActionType == "submit")
            {
                UpdateUnitFormActionTypePM(UnitFormID);
            }

        }
        private void ConvertByteToImage(FormGroupModel.Resources item)
        {
            // Convert the Base64 UUEncoded input into binary output. 
            byte[] binaryData;
            try
            {
                binaryData = Convert.FromBase64String(item.ResourceStorageBase64);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Base64 string is null.");
                return;
            }
            catch (FormatException ex)
            {
                throw ex;
            }

            // Write out the decoded data.
            try
            {
                if (!Directory.Exists(item.PhysicalPathServer))
                {
                    Directory.CreateDirectory(item.PhysicalPathServer);
                }

                // Use the correct path by combining the physical directory and filename
                var fullPath = Path.Combine(item.PhysicalPathServer, Path.GetFileName(item.ResourceStoragePath));
                using (var outFile = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    outFile.Write(binaryData, 0, binaryData.Length);
                }
            }
            catch (Exception exp)
            {
                // Error creating stream or writing to it.
                throw exp;
            }
        }
        public void InsertResource(Guid guidId, string fileName, string filePath, string mimeType, Guid? userID)
        {
            var newResource = new tm_Resource
            {
                ID = guidId,
                FileName = fileName,
                FilePath = filePath, // This should already be the relative path
                MimeType = mimeType,
                FlagActive = true,
                CreateDate = DateTime.Now,
                //CreateBy = userID,
                UpdateDate = DateTime.Now,
                //UpdateBy = userID,
            };

            _context.tm_Resource.Add(newResource);
            _context.SaveChanges();
        }
        public bool InsertOrUpdateUnitFormResource(Guid ResourceID, Guid? UnitFormID, Guid? userID, int? RoleID, int? FormID)
        {
            // Check if a row already exists with the given UnitFormID, FormID, and RoleID
            var existingResource = _context.tr_UnitFormResource.FirstOrDefault(r => r.UnitFormID == UnitFormID && r.FormID == FormID && r.RoleID == RoleID);

            if (existingResource != null)
            {
                // If found, update the existing row
                existingResource.ResourceID = ResourceID;
                existingResource.FlagActive = true;
                existingResource.UpdateDate = DateTime.Now;
                //existingResource.UpdateBy = userID; // Update the user ID if applicable

                _context.tr_UnitFormResource.Update(existingResource);
            }
            else
            {
                // If not found, insert a new row
                var newResource = new tr_UnitFormResource
                {
                    UnitFormID = UnitFormID,
                    FormID = FormID,
                    RoleID = RoleID,
                    ResourceID = ResourceID,
                    FlagActive = true,
                    CreateDate = DateTime.Now,
                    //CreateBy = userID, // Set the user ID if applicable
                    UpdateDate = DateTime.Now,
                    //UpdateBy = userID // Set the user ID if applicable
                };

                _context.tr_UnitFormResource.Add(newResource);
            }

            // Save the changes to the database
            _context.SaveChanges();

            return true;
        }
        public bool SubmitUpdateVendorUnitForm(Guid ResourceID, Guid? UnitFormID , string? FormGrade , int? VendorID, Guid? userID, int? RoleID ,string? ActionType)
        {
             var unitForm = _context.tr_UnitForm
            .Where(uf => uf.ID == UnitFormID)
            .FirstOrDefault();

            if (unitForm != null)
            {
                unitForm.VendorID = VendorID;
                unitForm.VendorResourceID = ResourceID;
                unitForm.UpdateDate = DateTime.Now;
            }

            return true;
        }
        private void UpdateUnitFormActionTypePM(Guid? UnitFormID)
        {
            var unitFormAction = _context.tr_UnitFormAction.FirstOrDefault(a => a.UnitFormID == UnitFormID && a.RoleID == 2);
            if (unitFormAction != null)
            {
                unitFormAction.ActionType = null;
                unitFormAction.UpdateDate = DateTime.Now;
                _context.tr_UnitFormAction.Update(unitFormAction);
                _context.SaveChanges();
            }
        }
        private void InsertUnitFormActionLog(Guid? UnitFormID , string ActionType)
        {
            var actionLog = new tr_UnitFormActionLog
            {
                UnitFormID = UnitFormID,
                Remark = "PE " + ActionType + " UnitFormAction",
                ActionDate = DateTime.Now,
                CraeteDate = DateTime.Now,
                // CreateBy = unitFormAction.CreateBy, // Uncomment and set appropriately if you have the CreateBy field
            };

            _context.tr_UnitFormActionLog.Add(actionLog);
            _context.SaveChanges();
        }
    }
}
