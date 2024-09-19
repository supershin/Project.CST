using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Drawing;

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
                          where t1.QCUnitCheckListID == filterData.QCUnitCheckListID
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

        public QC5DefectModel GetQC5Defact(QC5DefectModel filterData)
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
                              //QCUnitCheckListID = t1.QCUnitCheckListID,
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
                                                  where rs.DefectID == filterData.DefectID
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
                    var UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(x => x.ProjectID == model.ProjectID && x.UnitID == model.UnitID && x.Seq == model.Seq);

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
                            Seq = model.Seq,
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
                        Seq = model.Seq,
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
                                    using (var resizedImageStream = ResizeImage(imageStream, 0.5)) // Resize to 50%
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
                        existingDefect.QCUnitCheckListID = model.QCUnitCheckListID;
                        existingDefect.Seq = model.Seq;
                        existingDefect.DefectAreaID = model.DefectAreaID;
                        existingDefect.DefectTypeID = model.DefectTypeID;
                        existingDefect.DefectDescriptionID = model.DefectDescriptionID;
                        existingDefect.StatusID = model.StatusID;
                        existingDefect.Remark = model.Remark;
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

    }
}
