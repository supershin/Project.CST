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
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Humanizer.Localisation;
using System.Linq;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class QC5CheckRepo : IQC5CheckRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public QC5CheckRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }



        //public void InsertSelectFromQCUnitCheckList(Guid qcUnitCheckListID_where, Guid qcUnitCheckListID_IUD, int? Seq)
        //{

        //    var defectsToInsert = _context.tr_QC_UnitCheckList_Defect
        //                        .Where(d => d.QCUnitCheckListID == qcUnitCheckListID_where && d.FlagActive == true)
        //                        .Select(d => new tr_QC_UnitCheckList_Defect
        //                        {
        //                            QCUnitCheckListID = qcUnitCheckListID_IUD,
        //                            Seq = Seq,
        //                            SeqPass = null,
        //                            DefectAreaID = d.DefectAreaID,
        //                            DefectTypeID = d.DefectTypeID,
        //                            DefectDescriptionID = d.DefectDescriptionID,
        //                            StatusID = d.StatusID,
        //                            Remark = d.Remark,
        //                            IsMajorDefect = d.IsMajorDefect,
        //                            FlagActive = d.FlagActive,
        //                            CreateDate = DateTime.Now,
        //                            CreateBy = d.CreateBy,
        //                            UpdateDate = DateTime.Now,
        //                            UpdateBy = d.UpdateBy
        //                        }).ToList();

        //    _context.tr_QC_UnitCheckList_Defect.AddRange(defectsToInsert);
        //    _context.SaveChanges();
        //}



        //public void InsertSelectFromQCUnitCheckList_Resource(Guid qcUnitCheckListID_where, Guid qcUnitCheckListID_IUD)
        //{
        //    var QCUnitCheckListResourceToInsert = _context.tr_QC_UnitCheckList_Resource
        //                        .Where(d => d.QCUnitCheckListID == qcUnitCheckListID_where && d.FlagActive == true)
        //                        .Select(d => new tr_QC_UnitCheckList_Resource
        //                        {
        //                            QCUnitCheckListID = qcUnitCheckListID_IUD,
        //                            QCUnitCheckListDetailID = d.QCUnitCheckListDetailID,
        //                            DefectID = d.DefectID,
        //                            ResourceID = d.ResourceID,
        //                            IsSign = d.IsSign,
        //                            FlagActive = d.FlagActive,
        //                            CreateDate = DateTime.Now,
        //                            CreateBy = d.CreateBy,
        //                            UpdateDate = DateTime.Now,
        //                            UpdateBy = d.UpdateBy
        //                        }).ToList();

        //    _context.tr_QC_UnitCheckList_Resource.AddRange(QCUnitCheckListResourceToInsert);
        //    _context.SaveChanges();
        //}



        public QC5DetailModel GetQC5CheckDetail(QC5DetailModel filterData)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                var Chk_QC5_Previous = _context.tr_QC_UnitCheckList.FirstOrDefault(d => d.ProjectID == filterData.ProjectID && d.UnitID == filterData.UnitID && d.Seq == filterData.Seq - 1);

                var Chk_QC5 = _context.tr_QC_UnitCheckList.FirstOrDefault(d => d.ProjectID == filterData.ProjectID && d.UnitID == filterData.UnitID && d.Seq == filterData.Seq);

                if (Chk_QC5 == null && Chk_QC5_Previous != null)
                {
                    Guid QCUnitCheckListID = Guid.NewGuid();

                    Chk_QC5 = new tr_QC_UnitCheckList
                    {
                        ID = QCUnitCheckListID,
                        ProjectID = filterData.ProjectID,
                        UnitID = filterData.UnitID,
                        CheckListID = 6,
                        QCTypeID = SystemConstant.Unit_Form_QC.QC5,
                        Seq = filterData.Seq,
                        CheckListDate = DateTime.Now,
                        FlagActive = true,
                        CreateDate = DateTime.Now,
                        CreateBy = filterData.UserID,
                        UpdateDate = DateTime.Now,
                        UpdateBy = filterData.UserID
                    };

                    _context.tr_QC_UnitCheckList.Add(Chk_QC5);
                    _context.SaveChanges();

                    var existingAction_Previous = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(d => d.QCUnitCheckListID == QCUnitCheckListID);

                    var existingAction = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(x => x.QCUnitCheckListID == QCUnitCheckListID);

                    if (existingAction == null)
                    {
                        var newAction = new tr_QC_UnitCheckList_Action
                        {
                            QCUnitCheckListID = QCUnitCheckListID,
                            RoleID = SystemConstant.UserRole.QC,
                            Remark = existingAction_Previous?.Remark,
                            ActionDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            UpdateBy = filterData.UserID,
                            CreateDate = DateTime.Now,
                            CreateBy = filterData.UserID
                        };

                        _context.tr_QC_UnitCheckList_Action.Add(newAction);
                    }

                    var defectIDMapping = InsertSelectFromQCUnitCheckList(Chk_QC5_Previous.ID, QCUnitCheckListID, filterData.Seq);
                    InsertSelectFromQCUnitCheckList_ResourceDefect(Chk_QC5_Previous.ID, QCUnitCheckListID, defectIDMapping, filterData.Seq - 1);
                    InsertSelectFromQCUnitCheckList_Resource(Chk_QC5_Previous.ID, QCUnitCheckListID);

                    //InsertSelectFromQCUnitCheckList(Chk_QC5_Previous.ID, QCUnitCheckListID, filterData.Seq);

                    //InsertSelectFromQCUnitCheckList_Resource(Chk_QC5_Previous.ID, QCUnitCheckListID);

                    _context.SaveChanges();
                }

                var query = (from t1 in _context.tm_Project
                             join t2 in _context.tm_Unit on t1.ProjectID equals t2.ProjectID into unitGroup
                             from t2 in unitGroup.DefaultIfEmpty()
                             join t3 in _context.tm_Ext on t2.UnitStatusID equals t3.ID into extGroup
                             from t3 in extGroup.DefaultIfEmpty()
                             join t4 in _context.tr_QC_UnitCheckList on new { ProjectID = (Guid?)t1.ProjectID, UnitID = (Guid?)t2.UnitID, filterData.Seq } equals new { t4.ProjectID, t4.UnitID, t4.Seq } into unitCheckListGroup
                             from t4 in unitCheckListGroup.DefaultIfEmpty()
                             join t5 in _context.tr_QC_UnitCheckList_Action on t4.ID equals t5.QCUnitCheckListID into actionGroup
                             from t5 in actionGroup.DefaultIfEmpty()
                             join t6 in _context.tm_User on t5.UpdateBy equals t6.ID into Users
                             from t6 in Users.DefaultIfEmpty()
                             join t7 in _context.tr_QC_UnitCheckList_Resource on new { QCUnitCheckListID = (Guid?)t4.ID, FlagActive = (bool?)true, IsSign = (bool?)true } equals new { t7.QCUnitCheckListID, t7.FlagActive, t7.IsSign } into QC5UnitResourceGroup
                             from t7 in QC5UnitResourceGroup.DefaultIfEmpty()
                             join t8 in _context.tm_Resource on t7.ResourceID equals t8.ID into ResourceGroups
                             from t8 in ResourceGroups.DefaultIfEmpty()
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
                                 QC5UnitChecklistActionID = t5.ID,
                                 QC5UnitStatusID = t4.QCStatusID,
                                 QC5UnitChecklistRemark = t5.Remark,
                                 PathQC5SignatureImage = t8.FilePath,
                                 QC5SignatureDate = FormatExtension.FormatDateToDayMonthNameYearTime(t8.UpdateDate),
                                 QC5UpdateDate = FormatExtension.FormatDateToDayMonthNameYearTime(t4.UpdateDate),
                                 QC5UpdateByName = t6.FirstName + ' ' + t6.LastName,
                                 Seq = t4.Seq,
                                 ActionType = t5.ActionType
                             }).FirstOrDefault();

                scope.Complete();  // Commit transaction if everything is successful

                return query;
            }
        }

        public Dictionary<int?, int?> InsertSelectFromQCUnitCheckList(Guid qcUnitCheckListID_where, Guid qcUnitCheckListID_IUD, int? Seq)
        {
            var defectsToInsert = _context.tr_QC_UnitCheckList_Defect
                                    .Where(d => d.QCUnitCheckListID == qcUnitCheckListID_where
                                             && d.FlagActive == true
                                             //&& d.StatusID == 28
                                             )
                                    .Select(d => new tr_QC_UnitCheckList_Defect
                                    {
                                        QCUnitCheckListID = qcUnitCheckListID_IUD,
                                        RefSeq = d.RefSeq,
                                        Seq = d.StatusID == 27 ? null : Seq,
                                        DefectAreaID = d.DefectAreaID,
                                        DefectTypeID = d.DefectTypeID,
                                        DefectDescriptionID = d.DefectDescriptionID,
                                        StatusID = d.StatusID,
                                        Remark = d.Remark,
                                        IsMajorDefect = d.IsMajorDefect,
                                        FlagActive = d.FlagActive,
                                        CreateDate = DateTime.Now,
                                        CreateBy = d.CreateBy,
                                        UpdateDate = DateTime.Now,
                                        UpdateBy = d.UpdateBy
                                    }).ToList();

            _context.tr_QC_UnitCheckList_Defect.AddRange(defectsToInsert);
            _context.SaveChanges();

            // Create a dictionary to map old DefectID to new DefectID
            var defectIDMapping = new Dictionary<int?, int?>();

            // Map the old DefectIDs to new ones
            for (int i = 0; i < defectsToInsert.Count; i++)
            {
                var oldDefectID = _context.tr_QC_UnitCheckList_Defect
                                 .Where(d => d.QCUnitCheckListID == qcUnitCheckListID_where
                                          && d.FlagActive == true
                                        //&& d.StatusID == 28
                                        )
                                 .Select(d => d.ID).Skip(i).First();

                defectIDMapping[oldDefectID] = defectsToInsert[i].ID;
            }

            return defectIDMapping;
        }

        public void InsertSelectFromQCUnitCheckList_ResourceDefect(Guid qcUnitCheckListID_where, Guid qcUnitCheckListID_IUD, Dictionary<int?, int?> defectIDMapping, int? Seq)
        {
            var QCUnitCheckListResourceToInsert = _context.tr_QC_UnitCheckList_Resource
                .Join(_context.tr_QC_UnitCheckList,
                      t1 => t1.QCUnitCheckListID,
                      t2 => t2.ID,
                      (t1, t2) => new { t1, t2 })
                .Join(_context.tr_QC_UnitCheckList_Defect,
                      combined => combined.t1.DefectID,
                      t3 => t3.ID,
                      (combined, t3) => new { combined.t1, combined.t2, t3 })
                .Where(x => x.t1.QCUnitCheckListID == qcUnitCheckListID_where
                            && x.t1.FlagActive == true
                            && x.t2.FlagActive == true
                            && x.t3.FlagActive == true
                            && x.t2.Seq == Seq
                       //&& x.t3.StatusID == 28
                       )
                .Select(x => new tr_QC_UnitCheckList_Resource
                {
                    QCUnitCheckListID = qcUnitCheckListID_IUD,
                    QCUnitCheckListDetailID = x.t1.QCUnitCheckListDetailID,
                    DefectID = x.t1.DefectID.HasValue ? defectIDMapping[x.t1.DefectID.Value] : (int?)null, // Handle nullable DefectID
                    ResourceID = x.t1.ResourceID,
                    IsSign = x.t1.IsSign,
                    FlagActive = x.t1.FlagActive,
                    CreateDate = DateTime.Now,
                    CreateBy = x.t1.CreateBy,
                    UpdateDate = DateTime.Now,
                    UpdateBy = x.t1.UpdateBy
                }).ToList();

            _context.tr_QC_UnitCheckList_Resource.AddRange(QCUnitCheckListResourceToInsert);
            _context.SaveChanges();

        }

        public void InsertSelectFromQCUnitCheckList_Resource(Guid qcUnitCheckListID_where, Guid qcUnitCheckListID_IUD)
        {
            // Step 1: Query to select records where QCUnitCheckListID = sourceQCUnitCheckListID and DefectID is null
            var recordsToInsert = _context.tr_QC_UnitCheckList_Resource
                .Where(d => d.QCUnitCheckListID == qcUnitCheckListID_where && d.DefectID == null)
                .Select(d => new tr_QC_UnitCheckList_Resource
                {
                    QCUnitCheckListID = qcUnitCheckListID_IUD, // Inserting into the same table with new QCUnitCheckListID
                    QCUnitCheckListDetailID = d.QCUnitCheckListDetailID,
                    DefectID = d.DefectID, // DefectID is null here, as per your filter
                    ResourceID = d.ResourceID,
                    IsSign = d.IsSign,
                    FlagActive = d.FlagActive,
                    CreateDate = DateTime.Now, // Set current date for the new record
                    CreateBy = d.CreateBy,
                    UpdateDate = DateTime.Now, // Set current date for the new record
                    UpdateBy = d.UpdateBy
                }).ToList();

            // Step 2: Insert the selected records into the same table
            _context.tr_QC_UnitCheckList_Resource.AddRange(recordsToInsert);
            _context.SaveChanges(); // Commit the transaction
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
                              RefSeq = t1.RefSeq,
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
                                                  where rs.DefectID == filterData.DefectID && rs.FlagActive == true && resource.FlagActive == true && rs.IsSign != true
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
                            CreateDate = DateTime.Now,
                            CreateBy = userid
                        };

                        _context.tr_QC_UnitCheckList_Action.Add(newAction);
                    }

                    // Insert the new defect record
                    var newDefect = new tr_QC_UnitCheckList_Defect
                    {
                        QCUnitCheckListID = QCUnitCheckListID,
                        RefSeq = model.Seq.ToInt(),
                        Seq = null,
                        DefectAreaID = model.DefectAreaID,
                        DefectTypeID = model.DefectTypeID,
                        DefectDescriptionID = model.DefectDescriptionID,
                        StatusID = 28,
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

        public void UpdateDetailQCUnitCheckListDefect(QC5IUDModel model)
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

        public void RemoveImage(Guid resourceId, Guid UserID)
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
                Timeout = TimeSpan.FromMinutes(3)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    var QC_UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(d => d.ID == model.QCUnitCheckListID);

                    if (QC_UnitCheckList != null)
                    {
                        QC_UnitCheckList.QCStatusID = model.QCStatusID;
                        //if (model.QCStatusID == SystemConstant.UnitQCStatus.IsNotReadyInspect)
                        //{
                        //    QC_UnitCheckList.IsNotReadyInspect = true;
                        //}
                        //else if (model.QCStatusID == SystemConstant.UnitQCStatus.IsPassCondition)
                        //{
                        //    QC_UnitCheckList.IsPassCondition = true;
                        //}
                        QC_UnitCheckList.UpdateDate = DateTime.Now;
                        QC_UnitCheckList.UpdateBy = model.UserID;

                        var QC_UnitCheckList_Action = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(d => d.ID == model.QCUnitCheckListActionID);

                        if (QC_UnitCheckList_Action != null)
                        {
                            QC_UnitCheckList_Action.ActionType = model.ActionType;
                            if (!string.IsNullOrEmpty(model.QCRemark))
                            {
                                if (QC_UnitCheckList_Action.Remark != model.QCRemark)
                                {
                                    QC_UnitCheckList_Action.Remark = model.QCRemark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now);
                                }
                            }
                            else
                            {
                                QC_UnitCheckList_Action.Remark = "";
                            }
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
                                        IsSign = false,
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

                        //if (model.Sign != null)
                        //{
                        //    SaveSignature(model.Sign, model.ApplicationPath, model.QCUnitCheckListID, model.UserID);
                        //}

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

        public void SelectedQCUnitCheckListDefectStatus(QC5IUDModel model)
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
                        existingDefect.StatusID = model.StatusID;
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



        public void SaveSignature(SignatureQC5 signData, string? appPath, Guid? QCUnitCheckListID, Guid? userID)
        {
            var resource = new ResourcesSignatureQC5
            {
                MimeType = signData.MimeType,
                ResourceStorageBase64 = signData.StorageBase64
            };

            Guid guidId = Guid.NewGuid();
            string fileName = guidId + ".jpg";
            var folder = DateTime.Now.ToString("yyyyMM");
            var dirPath = Path.Combine(appPath, "wwwroot", "Upload", "document", folder, "signQC5");
            var filePath = Path.Combine(dirPath, fileName);

            resource.PhysicalPathServer = dirPath;
            resource.ResourceStoragePath = Path.Combine("Upload", "document", folder, "signQC5", fileName).Replace("\\", "/");

            ConvertByteToImage(resource);
            InsertResource(guidId, fileName, resource.ResourceStoragePath, "image/jpeg", userID);
            InsertOrUpdateQCUnitCheckListResource(guidId, QCUnitCheckListID, userID);

        }
        private void ConvertByteToImage(ResourcesSignatureQC5 item)
        {
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
        public bool InsertOrUpdateQCUnitCheckListResource(Guid ResourceID, Guid? QCUnitCheckListID, Guid? userID)
        {

            var QCUnitCheckListResource = _context.tr_QC_UnitCheckList_Resource.FirstOrDefault(r => r.QCUnitCheckListID == QCUnitCheckListID);

            if (QCUnitCheckListResource != null)
            {
                QCUnitCheckListResource.ResourceID = ResourceID;
                QCUnitCheckListResource.IsSign = true;
                QCUnitCheckListResource.FlagActive = true;
                QCUnitCheckListResource.UpdateDate = DateTime.Now;
                QCUnitCheckListResource.UpdateBy = userID;

                _context.tr_QC_UnitCheckList_Resource.Update(QCUnitCheckListResource);
            }
            else
            {
                var newResource = new tr_QC_UnitCheckList_Resource
                {
                    QCUnitCheckListID = QCUnitCheckListID,
                    ResourceID = ResourceID,
                    IsSign = true,
                    FlagActive = true,
                    CreateDate = DateTime.Now,
                    CreateBy = userID,
                    UpdateDate = DateTime.Now,
                    UpdateBy = userID
                };

                _context.tr_QC_UnitCheckList_Resource.Add(newResource);
            }

            _context.SaveChanges();

            return true;
        }
    }
}
