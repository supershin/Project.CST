using Humanizer.Localisation;
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
                        join t4 in _context.tr_UnitFormCheckList.Where(f => f.FlagActive == true) on new { t1.FormID, PackageID = t2.ID, CheckListID = t3.ID, model.UnitFormID } equals new { t4.FormID, t4.PackageID, t4.CheckListID, t4.UnitFormID } into t4Group
                        from t4 in t4Group.Where(t => t.StatusID != null).DefaultIfEmpty()
                        join t5 in _context.tr_UnitFormCheckList.Where(f => f.FlagActive == true) on new { t1.FormID, PackageID = t2.ID, CheckListID = t3.ID, model.UnitFormID } equals new { t5.FormID, t5.PackageID, t5.CheckListID, t5.UnitFormID } into t5Group
                        from t5 in t5Group.Where(t => t.StatusID == 2).DefaultIfEmpty()
                        join t6 in _context.tr_UnitFormPassCondition.Where(p => p.FlagActive == true) on new { t4.UnitFormID, t4.GroupID } equals new { t6.UnitFormID, t6.GroupID } into t6Group
                        from t6 in t6Group.DefaultIfEmpty()
                        group new { t3, t4, t5, t6 } by new { t1.ID, t1.FormID, t1.Name, t6.LockStatusID } into g
                        select new FormGroupModel
                        {
                            GroupID = g.Key.ID,
                            FormID = g.Key.FormID,
                            GroupName = g.Key.Name,
                            LockStatusID = g.Key.LockStatusID,
                            Cnt_CheckList_All = g.Count(x => x.t3 != null),
                            Cnt_CheckList_Unit = g.Count(x => x.t4 != null),
                            Cnt_CheckList_NotPass = g.Count(x => x.t5 != null),
                            StatusUse = g.Count(x => x.t5 != null) > 0 && g.Count(x => x.t3 != null) == g.Count(x => x.t4 != null) ? "danger" :
                                        g.Count(x => x.t4 != null) == 0 ? "secondary" :
                                        g.Count(x => x.t3 != null) == g.Count(x => x.t4 != null) ? "success" :
                                        g.Count(x => x.t3 != null) > g.Count(x => x.t4 != null) ? "warning" : ""
                        };

            var orderedQuery = query.OrderBy(fg => fg.GroupID).ToList();

            //var query = from t0 in _context.tm_Form
            //            where t0.ID == model.FormID
            //            join t1 in _context.tm_FormGroup on t0.ID equals t1.FormID into t1Group
            //            from t1 in t1Group.DefaultIfEmpty()
            //            join t2 in _context.tm_FormPackage on t1.ID equals t2.GroupID into t2Group
            //            from t2 in t2Group.DefaultIfEmpty()
            //            join t3 in _context.tm_FormCheckList on t2.ID equals t3.PackageID into t3Group
            //            from t3 in t3Group.DefaultIfEmpty()
            //            join tUnitForm in _context.tr_UnitForm on t0.ID equals tUnitForm.FormID into tUnitFGroup
            //            from tUnitForm in tUnitFGroup.Where(t => t.UnitID == model.UnitID).DefaultIfEmpty()
            //            join t4 in _context.tr_UnitFormCheckList on new { t1.FormID, PackageID = t2.ID, CheckListID = t3.ID } equals new { t4.FormID, t4.PackageID, t4.CheckListID } into t4Group
            //            from t4 in t4Group.Where(t => t.StatusID != null && t.UnitFormID == model.UnitFormID).DefaultIfEmpty()
            //            join t5 in _context.tr_UnitFormCheckList on new { t1.FormID, PackageID = t2.ID, CheckListID = t3.ID } equals new { t5.FormID, t5.PackageID, t5.CheckListID } into t5Group
            //            from t5 in t5Group.Where(t => t.StatusID == 2 && t.UnitFormID == model.UnitFormID).DefaultIfEmpty()
            //            join t6 in _context.tr_UnitFormPassCondition on new { t4.UnitFormID, t4.GroupID } equals new { t6.UnitFormID, t6.GroupID } into t6Group
            //            from t6 in t6Group.DefaultIfEmpty()
            //            group new { t0, t1, t2, t3, t4, t5, tUnitForm, t6 } by new { t0.ID, t0.Name, GroupID = t1.ID, GroupName = t1.Name, tUnitForm.UnitID, t6.LockStatusID } into g
            //            select new FormGroupModel
            //            {
            //                FormID = g.Key.ID,
            //                UnitID = g.Key.UnitID,
            //                GroupID = g.Key.GroupID,
            //                GroupName = g.Key.GroupName,
            //                LockStatusID = g.Key.LockStatusID,
            //                Cnt_CheckList_All = g.Count(x => x.t3 != null),
            //                Cnt_CheckList_Unit = g.Count(x => x.t4 != null),
            //                Cnt_CheckList_NotPass = g.Count(x => x.t5 != null),
            //                StatusUse = g.Count(x => x.t5 != null) > 0 && g.Count(x => x.t3 != null) == g.Count(x => x.t4 != null) ? "danger" :
            //                                            g.Count(x => x.t4 != null) == 0 ? "secondary" :
            //                                            g.Count(x => x.t3 != null) == g.Count(x => x.t4 != null) ? "success" :
            //                                            g.Count(x => x.t3 != null) > g.Count(x => x.t4 != null) ? "warning" : ""
            //            };

            //var result = query.ToList();
            return orderedQuery;

        }

        public FormGroupDetail GetFormGroupDetail(Guid unitFormId)
        {
            var result = (from t1 in _context.tr_UnitForm
                          where t1.ID == unitFormId
                          join t2 in _context.tr_UnitFormAction.Where(a => a.RoleID == 1)
                              on t1.ID equals t2.UnitFormID into peActions
                          from peAction in peActions.DefaultIfEmpty()
                          join t3 in _context.tr_UnitFormAction.Where(a => a.RoleID == 2)
                              on t1.ID equals t3.UnitFormID into pmActions
                          from pmAction in pmActions.DefaultIfEmpty()
                          select new FormGroupDetail
                          {
                              ID = t1.ID,
                              Grade = t1.Grade,
                              FormID = t1.FormID,
                              PE_ActionType = peAction != null ? peAction.ActionType : null,
                              PE_StatusID = peAction != null ? peAction.StatusID : null,
                              PM_ActionType = pmAction != null ? pmAction.ActionType : null,
                              PM_StatusID = pmAction != null ? pmAction.StatusID : null
                          }).FirstOrDefault(); // ใช้ FirstOrDefault เพื่อดึงข้อมูลรายการแรกหรือค่า null หากไม่มีข้อมูล

            return result;
        }


        public void SubmitSaveFormGroup(FormGroupModel.FormGroupIUDModel model)
        {
            if (model.Act == "save")
            {
                var unitForm = _context.tr_UnitForm
               .Where(uf => uf.ID == model.UnitFormID)
               .FirstOrDefault();

                if (unitForm != null)
                {
                    unitForm.Grade = model.FormGrade;
                    unitForm.UpdateDate = DateTime.Now;
                    //unitForm.UpdateBy = 1 ;
                }
            }
            else
            {
                if (model.Sign != null)
                {
                    SaveSignature(model.Sign, model.ApplicationPath, model.UnitFormID, model.FormGrade, model.VendorID ,model.userID , model.RoleID);
                }
            }

            _context.SaveChanges();
        }
        private void SaveSignature(SignatureData signData, string? appPath , Guid? UnitFormID , string? FormGrade , int? VendorID , Guid? userID ,int? RoleID)
        {
            var resource = new FormGroupModel.Resources
            {
                MimeType = signData.MimeType,
                ResourceStorageBase64 = signData.StorageBase64
            };
            Guid guidId = Guid.NewGuid(); // สร้าง Guid ใหม่สำหรับไฟล์
            string fileName = guidId + ".jpg"; // ตั้งชื่อไฟล์ด้วย Guid และนามสกุล .jpg
            var folder = DateTime.Now.ToString("yyyyMM");
            var dirPath = $"Upload/document/{folder}/sign/"; // กำหนด path ของโฟลเดอร์
            var filePath = dirPath + fileName; // กำหนด path ของไฟล์

            resource.PhysicalPathServer = appPath;
            resource.ResourceStorageBase64 = signData.StorageBase64;
            resource.ResourceStoragePath = filePath;
            resource.Directory = Path.Combine(appPath, dirPath);

            ConvertByteToImage(resource);
            InsertResource(guidId , fileName , filePath , "jpg" , userID);
            InsertUnitFormResource(guidId, UnitFormID , userID , RoleID);
            SubmitUpdateUnitForm(guidId, UnitFormID , FormGrade , VendorID , userID, RoleID);
        }
        private void ConvertByteToImage(FormGroupModel.Resources item)
        {
            // Convert the Base64 UUEncoded input into binary output. 
            byte[] binaryData;
            try
            {
                binaryData =
                   System.Convert.FromBase64String(item.ResourceStorageBase64);
            }
            catch (System.ArgumentNullException)
            {
                System.Console.WriteLine("Base 64 string is null.");
                return;
            }
            catch (System.FormatException ex)
            {
                throw ex;
            }

            // Write out the decoded data.
            System.IO.FileStream outFile;
            try
            {
                if (!Directory.Exists(item.Directory))
                {
                    Directory.CreateDirectory(item.Directory);
                }
                //var pathFile = string.Format("{0}{1}", item.PhysicalPathServer, item.ResourceStoragePath);
                outFile = new System.IO.FileStream(item.ResourceStoragePath,
                                           System.IO.FileMode.Create,
                                           System.IO.FileAccess.Write);
                outFile.Write(binaryData, 0, binaryData.Length);
                outFile.Close();
            }
            catch (System.Exception exp)
            {
                // Error creating stream or writing to it.
                throw exp;
            }
        }
        public void InsertResource(Guid guidId , string fileName, string filePath, string mimeType , Guid? userID)
        {
            var newResource = new tm_Resource
            {
                ID = guidId, // สร้าง Guid ใหม่สำหรับ ID
                FileName = fileName,
                FilePath = filePath,
                MimeType = mimeType,
                FlagActive = true,
                CreateDate = DateTime.Now, // วันที่สร้าง
                //CreateBy = userID, // ผู้สร้าง
                UpdateDate = DateTime.Now, // วันที่อัพเดท
                //UpdateBy = userID // ผู้ที่อัพเดท
            };

            _context.tm_Resource.Add(newResource);
            _context.SaveChanges();
        }
        public bool InsertUnitFormResource(Guid ResourceID, Guid? UnitFormID ,Guid? userID ,int? RoleID)
        {
            var newResource = new tr_UnitFormResource
            {
                UnitFormID = UnitFormID,
                RoleID = RoleID,
                ResourceID = ResourceID,
                FlagActive = true,
                CreateDate = DateTime.Now, // วันที่สร้าง
                //CreateBy = userID, // ผู้สร้าง
                UpdateDate = DateTime.Now, // วันที่อัพเดท
                //UpdateBy = userID// ผู้ที่อัพเดท
            };

            _context.tr_UnitFormResource.Add(newResource);
            _context.SaveChanges();

            return true;
        }
        public bool SubmitUpdateUnitForm(Guid ResourceID, Guid? UnitFormID , string? FormGrade , int? VendorID, Guid? userID, int? RoleID)
        {
             var unitForm = _context.tr_UnitForm
            .Where(uf => uf.ID == UnitFormID)
            .FirstOrDefault();

            if (unitForm != null)
            {
                unitForm.VendorID = VendorID;
                unitForm.VendorResourceID = ResourceID;
                unitForm.Grade = FormGrade;
                unitForm.UpdateDate = DateTime.Now;
                //unitForm.UpdateBy = userID;
            }

            var unitFormAction = _context.tr_UnitFormAction
            .Where(uf => uf.UnitFormID == UnitFormID && uf.RoleID == 1)
            .FirstOrDefault();

            if (unitFormAction != null)
            {
                unitFormAction.RoleID = RoleID;
                unitFormAction.ActionType = "submit";
                unitFormAction.UpdateDate = DateTime.Now;
                //unitFormAction.UpdateBy = userID;
            }

            return true;
        }
    }
}
