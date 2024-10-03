using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Drawing;
using static Project.ConstructionTracking.Web.Models.ApproveFormcheckIUDModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Project.ConstructionTracking.Web.Models.FormGroupModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class QC5CheckRepo : IQC5CheckRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public QC5CheckRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public QC5DetailModel GetQC5CheckDetail(QC5DetailModel filterData)
        {
            var query = (from t1 in _context.tm_Project
                         join t2 in _context.tm_Unit on t1.ProjectID equals t2.ProjectID into unitGroup
                         from t2 in unitGroup.DefaultIfEmpty()
                         join t3 in _context.tm_Ext on t2.UnitStatusID equals t3.ID into extGroup
                         from t3 in extGroup.DefaultIfEmpty()
                         join t4 in _context.tr_QC_UnitCheckList on new { ProjectID = (Guid?)t1.ProjectID, UnitID = (Guid?)t2.UnitID , filterData.Seq } equals new { t4.ProjectID, t4.UnitID ,t4.Seq } into unitCheckListGroup
                         from t4 in unitCheckListGroup.DefaultIfEmpty()
                         join t5 in _context.tr_QC_UnitCheckList_Action on t4.ID equals t5.QCUnitCheckListID into actionGroup
                         from t5 in actionGroup.DefaultIfEmpty()
                         join t6 in _context.tm_User on t5.UpdateBy equals t6.ID into Users
                         from t6 in Users.DefaultIfEmpty()
                         where t1.ProjectID == filterData.ProjectID
                               && t2.UnitID == filterData.UnitID
                         select new QC5DetailModel
                         {
                             ProjectID = t1.ProjectID,
                             ProjectName = t1.ProjectName,
                             ProjectTypeID = t1.ProjectTypeID,
                             UnitID = t2.UnitID,
                             UnitCode = t2.UnitCode,
                             UnitStatusName = t3.Name,
                             QC5UnitChecklistID = t4.ID,
                             QC5UnitStatusID = t4.QCStatusID,
                             QC5UpdateDate = FormatExtension.FormatDateToDayMonthNameYearTime(t4.UpdateDate),
                             QC5UpdateByName = t6.FirstName + ' ' + t6.LastName,
                             Seq = t4.Seq,
                             ActionType = t5.ActionType
                         }).FirstOrDefault(); 

            return query;
        }

        public List<QC5ChecklistModel> GetQCUnitCheckListDefects(QC5ChecklistModel filterData)
        {
            var result = (from t1 in _context.tr_QC_UnitCheckList_Defect
                          join t2 in _context.tm_DefectArea on t1.DefectAreaID equals t2.ID into defectAreaGroup
                          from t2 in defectAreaGroup.DefaultIfEmpty()
                          join t3 in _context.tm_DefectType on t1.DefectTypeID equals t3.ID into defectTypeGroup
                          from t3 in defectTypeGroup.DefaultIfEmpty()
                          join t4 in _context.tm_DefectDescription on t1.DefectDescriptionID equals t4.ID into defectDescriptionGroup
                          from t4 in defectDescriptionGroup.DefaultIfEmpty()
                          where t1.QCUnitCheckListID == filterData.QCUnitCheckListID && t1.FlagActive == true
                          select new QC5ChecklistModel
                          {
                              ID = t1.ID,
                              QCUnitCheckListID = t1.QCUnitCheckListID,
                              Seq = t1.Seq,
                              DefectAreaID = t1.DefectAreaID,
                              DefectAreaName = t2.Name,
                              DefectTypeID = t1.DefectTypeID,
                              DefectTypeName = t3.Name,
                              DefectDescriptionID = t1.DefectDescriptionID,
                              DefectDescriptionName = t4.Name,
                              StatusID = t1.StatusID,
                              Remark = t1.Remark,
                              IsMajorDefect = t1.IsMajorDefect,
                              FlagActive = t1.FlagActive,
      
                              ListRadioChecklist = (from rc in _context.tm_Ext
                                                    where rc.ExtTypeID == SystemConstant.Ext_Type.QC5RadioChecklist
                                                    select new QC5RadioCheckListModel
                                                    {
                                                        RadioCheckValue = rc.ID, 
                                                        RadioCheckText = rc.Name  
                                                    }).ToList()
                          }).ToList();

            return result;
        }

        public QC5DefectModel GetQC5DefactEdit(QC5DefectModel filterData)
        {
            var result = (from t1 in _context.tr_QC_UnitCheckList_Defect
                          join t2 in _context.tm_DefectArea on t1.DefectAreaID equals t2.ID into defectAreaGroup
                          from t2 in defectAreaGroup.DefaultIfEmpty()
                          join t3 in _context.tm_DefectType on t1.DefectTypeID equals t3.ID into defectTypeGroup
                          from t3 in defectTypeGroup.DefaultIfEmpty()
                          join t4 in _context.tm_DefectDescription on t1.DefectDescriptionID equals t4.ID into defectDescriptionGroup
                          from t4 in defectDescriptionGroup.DefaultIfEmpty()
                          where t1.ID == filterData.DefectID
                          select new QC5DefectModel
                          {
                              DefectID = t1.ID,
                              QCUnitCheckListID = t1.QCUnitCheckListID,
                              Seq = t1.Seq,
                              DefectAreaID = t1.DefectAreaID,
                              DefectAreaName = t2.Name,
                              DefectTypeID = t1.DefectTypeID,
                              DefectTypeName = t3.Name,
                              DefectDescriptionID = t1.DefectDescriptionID,
                              DefectDescriptionName = t4.Name,
                              StatusID = t1.StatusID,
                              Remark = t1.Remark,
                              IsMajorDefect = t1.IsMajorDefect,
                              FlagActive = t1.FlagActive,

                              listImageNotpass = (from rs in _context.tr_QC_UnitCheckList_Resource
                                                  join resource in _context.tm_Resource on rs.ResourceID equals resource.ID
                                                  where rs.DefectID == filterData.DefectID && rs.FlagActive == true && resource.FlagActive == true
                                                  select new QC5DefactListImageNotPass
                                                    {
                                                        ResourceID = rs.ResourceID,
                                                        FileName = resource.FileName,
                                                        FilePath = resource.FilePath
                                                    }).ToList()
                          }).FirstOrDefault();

            return result;
        }

        public void InsertQCUnitCheckListDefect(QC5IUDModel model, Guid userid)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(2)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    // Try to find the existing UnitCheckList
                    var UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(x => x.ProjectID == model.ProjectID && x.UnitID == model.UnitID && x.Seq == model.Seq.ToInt());

                    Guid QCUnitCheckListID;

                    if (UnitCheckList == null)
                    {
                        QCUnitCheckListID = Guid.NewGuid();

                        // Create new UnitCheckList if not found
                        UnitCheckList = new tr_QC_UnitCheckList
                        {
                            ID = QCUnitCheckListID,
                            ProjectID = model.ProjectID,
                            UnitID = model.UnitID,
                            CheckListID = null,
                            QCTypeID = SystemConstant.Unit_Form_QC.QC5,
                            Seq = model.Seq.ToInt(),
                            CheckListDate = DateTime.Now,
                            FlagActive = true,
                            CreateDate = DateTime.Now,
                            CreateBy = userid,
                            UpdateDate = DateTime.Now,
                            UpdateBy = userid
                        };

                        _context.tr_QC_UnitCheckList.Add(UnitCheckList);
                    }
                    else
                    {
                        QCUnitCheckListID = UnitCheckList.ID;
                    }

                    // Try to find the existing action
                    var existingAction = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(x => x.QCUnitCheckListID == QCUnitCheckListID);
                    if (existingAction == null)
                    {
                        var newAction = new tr_QC_UnitCheckList_Action
                        {
                            QCUnitCheckListID = QCUnitCheckListID,
                            RoleID = SystemConstant.UserRole.QC,
                            ActionDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            UpdateBy = userid,
                            CraeteDate = DateTime.Now,
                            CreateBy = userid
                        };

                        _context.tr_QC_UnitCheckList_Action.Add(newAction);
                    }

                    // Insert the new defect record
                    var newDefect = new tr_QC_UnitCheckList_Defect
                    {
                        QCUnitCheckListID = QCUnitCheckListID,
                        Seq = model.Seq.ToInt(),
                        DefectAreaID = model.DefectAreaID,
                        DefectTypeID = model.DefectTypeID,
                        DefectDescriptionID = model.DefectDescriptionID,
                        StatusID = model.StatusID,
                        Remark = string.IsNullOrEmpty(model.Remark) ? "" : model.Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now),
                        IsMajorDefect = model.isMajorDefect,
                        FlagActive = true,
                        CreateDate = DateTime.Now,
                        CreateBy = userid,
                        UpdateDate = DateTime.Now,
                        UpdateBy = userid
                    };

                    _context.tr_QC_UnitCheckList_Defect.Add(newDefect);
                    _context.SaveChanges(); 

                    int newDefectID = newDefect.ID; 

                    if (model.Images != null && model.Images.Count > 0)
                    {
                        var folder = DateTime.Now.ToString("yyyyMM");
                        var dirPath = Path.Combine(model.ApplicationPath, "wwwroot", "Upload", "document", folder, "QC5Image");
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }

                        foreach (var image in model.Images)
                        {
                            if (image.Length > 0)
                            {
                                Guid guidId = Guid.NewGuid(); 
                                string fileName = guidId + ".jpg";
                                var filePath = Path.Combine(dirPath, fileName);

                                // Resize and save the image
                                using (var imageStream = image.OpenReadStream())
                                {
                                    using (var resizedImageStream = ResizeImage(imageStream, 0.7)) // Resize to 50%
                                    {
                                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            resizedImageStream.CopyTo(fileStream); // Save resized image
                                        }
                                    }
                                }

                                string relativeFilePath = Path.Combine("Upload", "document", folder, "QC5Image", fileName).Replace("\\", "/");

                                var newResource = new tm_Resource
                                {
                                    ID = Guid.NewGuid(),
                                    FileName = fileName,
                                    FilePath = relativeFilePath, 
                                    MimeType = "image/jpeg", 
                                    FlagActive = true,
                                    CreateDate = DateTime.Now,
                                    CreateBy = userid,
                                    UpdateDate = DateTime.Now,
                                    UpdateBy = userid
                                };
                                _context.tm_Resource.Add(newResource);

                                var newQCUnitCheckListResource = new tr_QC_UnitCheckList_Resource
                                {
                                    QCUnitCheckListID = QCUnitCheckListID,
                                    DefectID = newDefectID,
                                    ResourceID = newResource.ID,
                                    FlagActive = true,
                                    CreateDate = DateTime.Now,
                                    CreateBy = userid,
                                    UpdateDate = DateTime.Now,
                                    UpdateBy = userid
                                };
                                _context.tr_QC_UnitCheckList_Resource.Add(newQCUnitCheckListResource);
                            }
                        }
                    }

                    _context.SaveChanges();

                    // Complete the transaction
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    string errorMessage;

                    if (ex.Message.Contains("tr_QC_UnitCheckList_Action"))
                    {
                        errorMessage = "บันทึกข้อมูลลง tr_QC_UnitCheckList_Action ไม่สำเร็จ";
                    }
                    else if (ex.Message.Contains("tr_QC_UnitCheckList_Defect"))
                    {
                        errorMessage = "บันทึกข้อมูลลง tr_QC_UnitCheckList_Defect ไม่สำเร็จ";
                    }
                    else if (ex.Message.Contains("tr_QC_UnitCheckList"))
                    {
                        errorMessage = "บันทึกข้อมูลลง tr_QC_UnitCheckList ไม่สำเร็จ";
                    }
                    else
                    {
                        errorMessage = "บันทึกข้อมูลไม่สำเร็จ";
                    }

                    throw new Exception(errorMessage, ex);
                }
            }
        }

        public void UpdateQCUnitCheckListDefect(QC5IUDModel model)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(2)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    var existingDefect = _context.tr_QC_UnitCheckList_Defect.FirstOrDefault(d => d.ID == model.ID);

                    if (existingDefect != null)
                    {
                        existingDefect.DefectAreaID = model.DefectAreaID;
                        existingDefect.DefectTypeID = model.DefectTypeID;
                        existingDefect.DefectDescriptionID = model.DefectDescriptionID;
                        if (!string.IsNullOrEmpty(model.Remark))
                        {
                            if (existingDefect.Remark != model.Remark)
                            {
                                existingDefect.Remark = model.Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now);
                            }
                        }
                        else
                        {
                            existingDefect.Remark = "";
                        }
                        existingDefect.IsMajorDefect = model.isMajorDefect;
                        existingDefect.UpdateDate = DateTime.Now;
                        existingDefect.UpdateBy = model.UserID;


                        if (model.Images != null && model.Images.Count > 0)
                        {
                            var folder = DateTime.Now.ToString("yyyyMM");
                            var dirPath = Path.Combine(model.ApplicationPath, "wwwroot", "Upload", "document", folder, "QC5Image");
                            if (!Directory.Exists(dirPath))
                            {
                                Directory.CreateDirectory(dirPath);
                            }

                            foreach (var image in model.Images)
                            {
                                if (image.Length > 0)
                                {
                                    Guid guidId = Guid.NewGuid();
                                    string fileName = guidId + ".jpg";
                                    var filePath = Path.Combine(dirPath, fileName);

                                    // Resize and save the image
                                    using (var imageStream = image.OpenReadStream())
                                    {
                                        using (var resizedImageStream = ResizeImage(imageStream, 0.7)) // Resize to 50%
                                        {
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                resizedImageStream.CopyTo(fileStream); // Save resized image
                                            }
                                        }
                                    }

                                    string relativeFilePath = Path.Combine("Upload", "document", folder, "QC5Image", fileName).Replace("\\", "/");

                                    var newResource = new tm_Resource
                                    {
                                        ID = Guid.NewGuid(),
                                        FileName = fileName,
                                        FilePath = relativeFilePath,
                                        MimeType = "image/jpeg",
                                        FlagActive = true,
                                        CreateDate = DateTime.Now,
                                        CreateBy = model.UserID,
                                        UpdateDate = DateTime.Now,
                                        UpdateBy = model.UserID
                                    };
                                    _context.tm_Resource.Add(newResource);

                                    var newQCUnitCheckListResource = new tr_QC_UnitCheckList_Resource
                                    {
                                        QCUnitCheckListID = existingDefect.QCUnitCheckListID,
                                        DefectID = existingDefect.ID,
                                        ResourceID = newResource.ID,
                                        FlagActive = true,
                                        CreateDate = DateTime.Now,
                                        CreateBy = model.UserID,
                                        UpdateDate = DateTime.Now,
                                        UpdateBy = model.UserID
                                    };
                                    _context.tr_QC_UnitCheckList_Resource.Add(newQCUnitCheckListResource);
                                }
                            }
                        }

                        _context.SaveChanges();
                    }



                    scope.Complete(); // Commit the transaction
                }
                catch (Exception ex)
                {
                    throw new Exception("แก้ไขข้อมูลลง tr_QC_UnitCheckList_Defect ไม่สำเร็จ", ex);
                }
            }
        }

        public void RemoveQCUnitCheckListDefect(QC5IUDModel model)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(2)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    var existingDefect = _context.tr_QC_UnitCheckList_Defect.FirstOrDefault(d => d.ID == model.ID);

                    if (existingDefect != null)
                    {
                        existingDefect.FlagActive = false;
                        existingDefect.UpdateDate = DateTime.Now;
                        existingDefect.UpdateBy = model.UserID;

                        _context.SaveChanges();
                    }

                    scope.Complete(); // Commit the transaction
                }
                catch (Exception ex)
                {
                    throw new Exception("แก้ไขข้อมูลลง tr_QC_UnitCheckList_Defect ไม่สำเร็จ", ex);
                }
            }
        }

        public Stream ResizeImage(Stream imageStream, double scaleFactor)
        {
            using (var originalImage = System.Drawing.Image.FromStream(imageStream))
            {
                int newWidth = (int)(originalImage.Width * scaleFactor);
                int newHeight = (int)(originalImage.Height * scaleFactor);

                var resizedImage = new System.Drawing.Bitmap(newWidth, newHeight);
                using (var graphics = System.Drawing.Graphics.FromImage(resizedImage))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                }

                var resizedImageStream = new MemoryStream();
                resizedImage.Save(resizedImageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                resizedImageStream.Seek(0, SeekOrigin.Begin); // Reset stream position

                return resizedImageStream;
            }
        }

        public void RemoveImage(Guid resourceId , Guid UserID)
        {

            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(3)
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {

                    var Resource = _context.tm_Resource.FirstOrDefault(d => d.ID == resourceId);

                    if (Resource != null)
                    {
                        Resource.FlagActive = false;
                        Resource.UpdateDate = DateTime.Now;
                        Resource.UpdateBy = UserID;
                    }

                    var UnitCheckList_Resource = _context.tr_QC_UnitCheckList_Resource.FirstOrDefault(d => d.ResourceID == resourceId);

                    if (UnitCheckList_Resource != null)
                    {
                        UnitCheckList_Resource.FlagActive = false;
                        UnitCheckList_Resource.UpdateDate = DateTime.Now;
                        UnitCheckList_Resource.UpdateBy = UserID;                        
                    }
                    _context.SaveChanges();
                    scope.Complete(); // Commit the transaction
                }
                catch (Exception ex)
                {
                    throw new Exception("แก้ไขข้อมูลลง tr_QC_UnitCheckList_Defect ไม่สำเร็จ", ex);
                }
            }
        }

        public void SaveSubmitQC5UnitCheckList(QC5SaveSubmitModel model)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(2)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    var QC_UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(d => d.ID == model.QCUnitCheckListID);

                    if (QC_UnitCheckList != null)
                    {
                        QC_UnitCheckList.QCStatusID = model.QCStatusID;
                        if (model.QCStatusID == SystemConstant.UnitQCStatus.IsNotReadyInspect)
                        {
                            QC_UnitCheckList.IsNotReadyInspect = true;                         
                        }
                        else if (model.QCStatusID == SystemConstant.UnitQCStatus.IsPassCondition)
                        {
                            QC_UnitCheckList.IsPassCondition = true;
                        }
                        QC_UnitCheckList.UpdateDate = DateTime.Now;
                        QC_UnitCheckList.UpdateBy = model.UserID;

                        var QC_UnitCheckList_Action = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(d => d.ID == model.QCUnitCheckListActionID);

                        if (QC_UnitCheckList_Action != null)
                        {
                            QC_UnitCheckList_Action.ActionType = model.ActionType;
                            QC_UnitCheckList_Action.Remark = model.QCRemark;
                            QC_UnitCheckList_Action.ActionDate = DateTime.Now;
                            QC_UnitCheckList_Action.UpdateDate = DateTime.Now;
                            QC_UnitCheckList_Action.UpdateBy = model.UserID;
                        }

                        if (model.Images != null && model.Images.Count > 0)
                        {
                            var folder = DateTime.Now.ToString("yyyyMM");
                            var dirPath = Path.Combine(model.ApplicationPath, "wwwroot", "Upload", "document", folder, "QC5Image");
                            if (!Directory.Exists(dirPath))
                            {
                                Directory.CreateDirectory(dirPath);
                            }

                            foreach (var image in model.Images)
                            {
                                if (image.Length > 0)
                                {
                                    Guid guidId = Guid.NewGuid();
                                    string fileName = guidId + ".jpg";
                                    var filePath = Path.Combine(dirPath, fileName);

                                    // Resize and save the image
                                    using (var imageStream = image.OpenReadStream())
                                    {
                                        using (var resizedImageStream = ResizeImage(imageStream, 0.7)) // Resize to 50%
                                        {
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                resizedImageStream.CopyTo(fileStream); // Save resized image
                                            }
                                        }
                                    }

                                    string relativeFilePath = Path.Combine("Upload", "document", folder, "QC5Image", fileName).Replace("\\", "/");

                                    var newResource = new tm_Resource
                                    {
                                        ID = Guid.NewGuid(),
                                        FileName = fileName,
                                        FilePath = relativeFilePath,
                                        MimeType = "image/jpeg",
                                        FlagActive = true,
                                        CreateDate = DateTime.Now,
                                        CreateBy = model.UserID,
                                        UpdateDate = DateTime.Now,
                                        UpdateBy = model.UserID
                                    };
                                    _context.tm_Resource.Add(newResource);

                                    var newQCUnitCheckListResource = new tr_QC_UnitCheckList_Resource
                                    {
                                        QCUnitCheckListID = QC_UnitCheckList.ID,
                                        DefectID = null,
                                        ResourceID = newResource.ID,
                                        FlagActive = true,
                                        CreateDate = DateTime.Now,
                                        CreateBy = model.UserID,
                                        UpdateDate = DateTime.Now,
                                        UpdateBy = model.UserID
                                    };
                                    _context.tr_QC_UnitCheckList_Resource.Add(newQCUnitCheckListResource);
                                }
                            }
                        }

                        _context.SaveChanges();
                    }

                    scope.Complete(); // Commit the transaction
                }
                catch (Exception ex)
                {
                    throw new Exception("แก้ไขข้อมูลลง tr_QC_UnitCheckList_Defect ไม่สำเร็จ", ex);
                }
            }
        }


        private void SaveSignature(SignatureQC5 signData, string? appPath, Guid? UnitFormID, string? FormGrade, int? VendorID, Guid? userID, int? RoleID, int? FormID, string? ActionType)
        {
            var resource = new ResourcesSignature
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
            InsertOrUpdateUnitFormResource(guidId, UnitFormID, userID, RoleID, FormID);
            SubmitUpdateVendorUnitForm(guidId, UnitFormID, FormGrade, VendorID, userID, RoleID, ActionType);

        }
        private void ConvertByteToImage(ResourcesSignature item)
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
                var fullPath = Path.Combine(item.PhysicalPathServer, Path.GetFileName(item.ResourceStoragePath));
                using (var outFile = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    outFile.Write(binaryData, 0, binaryData.Length);
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        public void InsertResource(Guid guidId, string fileName, string filePath, string mimeType, Guid? userID)
        {
            var newResource = new tm_Resource
            {
                ID = guidId,
                FileName = fileName,
                FilePath = filePath,
                MimeType = mimeType,
                FlagActive = true,
                CreateDate = DateTime.Now,
                CreateBy = userID,
                UpdateDate = DateTime.Now,
                UpdateBy = userID,
            };

            _context.tm_Resource.Add(newResource);
            _context.SaveChanges();
        }
        public bool InsertOrUpdateUnitFormResource(Guid ResourceID, Guid? UnitFormID, Guid? userID, int? RoleID, int? FormID)
        {

            var existingResource = _context.tr_UnitFormResource.FirstOrDefault(r => r.UnitFormID == UnitFormID && r.FormID == FormID && r.RoleID == RoleID);

            if (existingResource != null)
            {
                existingResource.ResourceID = ResourceID;
                existingResource.FlagActive = true;
                existingResource.UpdateDate = DateTime.Now;
                existingResource.UpdateBy = userID; // Update the user ID if applicable

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
                    CreateBy = userID,
                    UpdateDate = DateTime.Now,
                    UpdateBy = userID
                };

                _context.tr_UnitFormResource.Add(newResource);
            }

            _context.SaveChanges();

            return true;
        }
        public bool SubmitUpdateVendorUnitForm(Guid ResourceID, Guid? UnitFormID, string? FormGrade, int? VendorID, Guid? userID, int? RoleID, string? ActionType)
        {
            var CompanyvenderID = _context.tr_CompanyVendor.Where(uf => uf.VendorID == VendorID).FirstOrDefault();

            var unitForm = _context.tr_UnitForm.Where(uf => uf.ID == UnitFormID).FirstOrDefault();

            if (unitForm != null)
            {
                unitForm.VendorID = VendorID ?? unitForm.VendorID;
                unitForm.CompanyVendorID = CompanyvenderID.CompanyVendorID != null ? CompanyvenderID.CompanyVendorID : 0;
                unitForm.VendorResourceID = ResourceID;
                unitForm.UpdateBy = userID;
                unitForm.UpdateDate = DateTime.Now;
            }

            return true;
        }

    }
}
