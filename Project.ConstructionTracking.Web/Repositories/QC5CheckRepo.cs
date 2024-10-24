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
using QuestPDF.Infrastructure;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using QuestPDF.Drawing;
using QuestPDF.Fluent;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class QC5CheckRepo : IQC5CheckRepo
    {
        private readonly ContructionTrackingDbContext _context;
        private readonly IGeneratePDFRepo _generatePDFRepo;

        public QC5CheckRepo(ContructionTrackingDbContext context, IGeneratePDFRepo generatePDFRepo)
        {
            _context = context;
            _generatePDFRepo = generatePDFRepo;
        }


        public QC5MaxSeqStatusChecklistModel CheckQC5MaxSeqStatusChecklist(QC5MaxSeqStatusChecklistModel filterData)
        {
            var result = (from t1 in _context.tr_QC_UnitCheckList
                          join t2 in _context.tr_QC_UnitCheckList_Action on t1.ID equals t2.QCUnitCheckListID into UnitCheckListAction
                          from t2 in UnitCheckListAction.DefaultIfEmpty()
                          where t1.ProjectID == filterData.ProjectID 
                             && t1.UnitID == filterData.UnitID 
                             && t1.FlagActive == true 
                             && t1.QCTypeID == SystemConstant.QcTypeID.QC5
                          orderby t1.Seq descending
                          select new QC5MaxSeqStatusChecklistModel
                          {
                              ProjectID = t1.ProjectID,
                              UnitID = t1.UnitID,
                              CheckListID = t1.CheckListID,
                              QCTypeID = t1.QCTypeID,
                              Seq = t1.Seq,
                              CheckListDate = t1.CheckListDate,
                              QCStatusID = t1.QCStatusID,
                              ActionType = t2.ActionType,
                              PESignResourceID = t1.PESignResourceID,
                              FlagActive = t1.FlagActive,
                              CreateDate = t1.CreateDate,
                              CreateBy = t1.CreateBy,
                              UpdateDate = t1.UpdateDate,
                              UpdateBy = t1.UpdateBy
                          }).FirstOrDefault();

            return result;
        }


        public QC5DetailModel GetQC5CheckDetail(QC5DetailModel filterData)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                var Chk_QC5_Previous = _context.tr_QC_UnitCheckList.FirstOrDefault(d => d.ProjectID == filterData.ProjectID && d.UnitID == filterData.UnitID && d.QCTypeID == SystemConstant.QcTypeID.QC5 && d.Seq == filterData.Seq - 1);

                var Chk_QC5 = _context.tr_QC_UnitCheckList.FirstOrDefault(d => d.ProjectID == filterData.ProjectID && d.UnitID == filterData.UnitID && d.QCTypeID == SystemConstant.QcTypeID.QC5 && d.Seq == filterData.Seq);

                Guid QCUnitCheckListID = Guid.NewGuid();

                if (Chk_QC5 == null && Chk_QC5_Previous != null)
                {
                   
                    Chk_QC5 = new tr_QC_UnitCheckList
                    {
                        ID = QCUnitCheckListID,
                        ProjectID = filterData.ProjectID,
                        UnitID = filterData.UnitID,
                        CheckListID = 6,
                        QCTypeID = SystemConstant.Unit_Form_QC.QC5,
                        Seq = filterData.Seq,
                        QCStatusID = Chk_QC5_Previous.QCStatusID,
                        PESignResourceID = Chk_QC5_Previous.PESignResourceID,
                        CheckListDate = DateTime.Now,
                        FlagActive = true,
                        CreateDate = DateTime.Now,
                        CreateBy = filterData.UserID,
                        UpdateDate = DateTime.Now,
                        UpdateBy = filterData.UserID
                    };

                    _context.tr_QC_UnitCheckList.Add(Chk_QC5);
                    _context.SaveChanges();

                    var existingAction_Previous = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(d => d.QCUnitCheckListID == Chk_QC5_Previous.ID);

                    var existingAction = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(x => x.QCUnitCheckListID == QCUnitCheckListID);

                    if (existingAction == null)
                    {
                        var newAction = new tr_QC_UnitCheckList_Action
                        {
                            QCUnitCheckListID = QCUnitCheckListID,
                            RoleID = SystemConstant.UserRole.QC,
                            ActionType = "save",
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

                    _context.SaveChanges();
                }
                else if (Chk_QC5 == null && Chk_QC5_Previous == null)
                {
                    Chk_QC5 = new tr_QC_UnitCheckList
                    {
                        ID = QCUnitCheckListID,
                        ProjectID = filterData.ProjectID,
                        UnitID = filterData.UnitID,
                        CheckListID = 6,
                        QCTypeID = SystemConstant.Unit_Form_QC.QC5,
                        Seq = 1,
                        CheckListDate = DateTime.Now,
                        FlagActive = true,
                        CreateDate = DateTime.Now,
                        CreateBy = filterData.UserID,
                        UpdateDate = DateTime.Now,
                        UpdateBy = filterData.UserID
                    };

                    _context.tr_QC_UnitCheckList.Add(Chk_QC5);
                   
                    var existingAction = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(x => x.QCUnitCheckListID == QCUnitCheckListID);

                    if (existingAction == null)
                    {
                        var newAction = new tr_QC_UnitCheckList_Action
                        {
                            QCUnitCheckListID = QCUnitCheckListID,
                            RoleID = SystemConstant.UserRole.QC,
                            ActionType = "save",
                            ActionDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            UpdateBy = filterData.UserID,
                            CreateDate = DateTime.Now,
                            CreateBy = filterData.UserID
                        };

                        _context.tr_QC_UnitCheckList_Action.Add(newAction);
                    }

                    _context.SaveChanges();
                }

                var query = (from t1 in _context.tm_Project
                             join t2 in _context.tm_Unit on t1.ProjectID equals t2.ProjectID into unitGroup
                             from t2 in unitGroup.DefaultIfEmpty()
                             join t3 in _context.tm_Ext on t2.UnitStatusID equals t3.ID into extGroup
                             from t3 in extGroup.DefaultIfEmpty()
                             join t4 in _context.tr_QC_UnitCheckList on new { ProjectID = (Guid?)t1.ProjectID, UnitID = (Guid?)t2.UnitID, filterData.Seq , QCTypeID = (int?)SystemConstant.QcTypeID.QC5 } equals new { t4.ProjectID, t4.UnitID, t4.Seq , t4.QCTypeID } into unitCheckListGroup
                             from t4 in unitCheckListGroup.DefaultIfEmpty()
                             join t5 in _context.tr_QC_UnitCheckList_Action on t4.ID equals t5.QCUnitCheckListID into actionGroup
                             from t5 in actionGroup.DefaultIfEmpty()
                             join t6 in _context.tm_User on t5.UpdateBy equals t6.ID into Users
                             from t6 in Users.DefaultIfEmpty()
                             join t7 in _context.tr_QC_UnitCheckList_Resource on new { QCUnitCheckListID = (Guid?)t4.ID, FlagActive = (bool?)true, IsSign = (bool?)true } equals new { t7.QCUnitCheckListID, t7.FlagActive, t7.IsSign } into QC5UnitResourceGroup
                             from t7 in QC5UnitResourceGroup.DefaultIfEmpty()
                             join t8 in _context.tm_Resource on t7.ResourceID equals t8.ID into ResourceGroups
                             from t8 in ResourceGroups.DefaultIfEmpty()
                             join t9 in _context.tr_Document on new { QCUnitCheckListID = (Guid?)t4.ID, FlagActive = (bool?)true} equals new { t9.QCUnitCheckListID, t9.FlagActive } into DocumentGroup
                             from t9 in DocumentGroup.DefaultIfEmpty()
                             join t10 in _context.tm_Resource on t9.ResourceID equals t10.ID into ResourcePDFGroups
                             from t10 in ResourcePDFGroups.DefaultIfEmpty()
                             join t11 in _context.tr_PE_Unit on t2.UnitID equals t11.UnitID into PEUNITGroup
                             from t11 in PEUNITGroup.DefaultIfEmpty()
                             where t1.ProjectID == filterData.ProjectID
                                   && t2.UnitID == filterData.UnitID
                                   //&& t4.QCTypeID == SystemConstant.QcTypeID.QC5
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
                                 // Set PathQC5SignatureImage to null if no matching resources are found
                                 PathQC5SignatureImage = t4 != null ? t8.FilePath : null,
                                 QC5SignatureDate = FormatExtension.FormatDateToDayMonthNameYearTime(t8.UpdateDate),
                                 QC5UpdateDate = FormatExtension.FormatDateToDayMonthNameYearTime(t4.UpdateDate),
                                 QC5UpdateByName = t6.FirstName + ' ' + t6.LastName,
                                 Seq = t4.Seq,
                                 ActionType = t5.ActionType,
                                 FilePathQCPDF = t10.FilePath,
                                 PEUnit = t11.UserID
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
                    var UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(x => x.ProjectID == model.ProjectID && x.UnitID == model.UnitID &&x.QCTypeID == SystemConstant.QcTypeID.QC5 && x.Seq == model.Seq.ToInt());

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
                            CheckListID = 6,
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
                        if (UnitCheckList.QCStatusID == 1)
                        {
                            UnitCheckList.QCStatusID = null;
                        }
                        else if (UnitCheckList.QCStatusID == 4 && model.isMajorDefect == true)
                        {
                            UnitCheckList.QCStatusID = null;
                        }
                        UnitCheckList.UpdateDate = DateTime.Now;
                        UnitCheckList.UpdateBy = model.UserID;
                        _context.tr_QC_UnitCheckList.Update(UnitCheckList);

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
                            ActionType = "save",
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

                    var UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(x => x.ProjectID == model.ProjectID && x.UnitID == model.UnitID && x.QCTypeID == SystemConstant.QcTypeID.QC5 && x.Seq == model.Seq.ToInt());
                    if (UnitCheckList != null)
                    {

                        if(UnitCheckList.QCStatusID == 4 && model.isMajorDefect == true)
                        {
                            UnitCheckList.QCStatusID = null;
                        }
                        UnitCheckList.UpdateDate = DateTime.Now;
                        UnitCheckList.UpdateBy = model.UserID;
                        _context.tr_QC_UnitCheckList.Update(UnitCheckList);
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

        //public void SaveSubmitQC5UnitCheckList(QC5SaveSubmitModel model)
        //{
        //    var transactionOptions = new TransactionOptions
        //    {
        //        IsolationLevel = IsolationLevel.ReadCommitted,
        //        Timeout = TimeSpan.FromMinutes(10)
        //    };

        //    using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
        //    {
        //        try
        //        {
        //            var QC_UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(d => d.ID == model.QCUnitCheckListID);

        //            if (QC_UnitCheckList != null)
        //            {
        //                QC_UnitCheckList.QCStatusID = model.QCStatusID;
        //                QC_UnitCheckList.UpdateDate = DateTime.Now;
        //                QC_UnitCheckList.UpdateBy = model.UserID;

        //                _context.tr_QC_UnitCheckList.Update(QC_UnitCheckList);

        //                var QC_UnitCheckList_Action = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(d => d.ID == model.QCUnitCheckListActionID);

        //                if (QC_UnitCheckList_Action != null)
        //                {
        //                    QC_UnitCheckList_Action.ActionType = model.ActionType;
        //                    if (!string.IsNullOrEmpty(model.QCRemark))
        //                    {
        //                        if (QC_UnitCheckList_Action.Remark != model.QCRemark)
        //                        {
        //                            QC_UnitCheckList_Action.Remark = model.QCRemark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        QC_UnitCheckList_Action.Remark = "";
        //                    }
        //                    QC_UnitCheckList_Action.ActionDate = DateTime.Now;
        //                    QC_UnitCheckList_Action.UpdateDate = DateTime.Now;
        //                    QC_UnitCheckList_Action.UpdateBy = model.UserID;
        //                    _context.tr_QC_UnitCheckList_Action.Update(QC_UnitCheckList_Action);
        //                }

        //                if (model.Images != null && model.Images.Count > 0)
        //                {
        //                    var folder = DateTime.Now.ToString("yyyyMM");
        //                    var dirPath = Path.Combine(model.ApplicationPath, "wwwroot", "Upload", "document", folder, "QC5Image");
        //                    if (!Directory.Exists(dirPath))
        //                    {
        //                        Directory.CreateDirectory(dirPath);
        //                    }

        //                    foreach (var image in model.Images)
        //                    {
        //                        if (image.Length > 0)
        //                        {
        //                            Guid guidId = Guid.NewGuid();
        //                            string fileName = guidId + ".jpg";
        //                            var filePath = Path.Combine(dirPath, fileName);

        //                            // Resize and save the image
        //                            using (var imageStream = image.OpenReadStream())
        //                            {
        //                                using (var resizedImageStream = ResizeImage(imageStream, 0.7)) // Resize to 50%
        //                                {
        //                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
        //                                    {
        //                                        resizedImageStream.CopyTo(fileStream); // Save resized image
        //                                    }
        //                                }
        //                            }

        //                            string relativeFilePath = Path.Combine("Upload", "document", folder, "QC5Image", fileName).Replace("\\", "/");

        //                            var newResource = new tm_Resource
        //                            {
        //                                ID = Guid.NewGuid(),
        //                                FileName = fileName,
        //                                FilePath = relativeFilePath,
        //                                MimeType = "image/jpeg",
        //                                FlagActive = true,
        //                                CreateDate = DateTime.Now,
        //                                CreateBy = model.UserID,
        //                                UpdateDate = DateTime.Now,
        //                                UpdateBy = model.UserID
        //                            };
        //                            _context.tm_Resource.Add(newResource);

        //                            var newQCUnitCheckListResource = new tr_QC_UnitCheckList_Resource
        //                            {
        //                                QCUnitCheckListID = QC_UnitCheckList.ID,
        //                                DefectID = null,
        //                                ResourceID = newResource.ID,
        //                                IsSign = false,
        //                                FlagActive = true,
        //                                CreateDate = DateTime.Now,
        //                                CreateBy = model.UserID,
        //                                UpdateDate = DateTime.Now,
        //                                UpdateBy = model.UserID
        //                            };
        //                            _context.tr_QC_UnitCheckList_Resource.Add(newQCUnitCheckListResource);
        //                        }
        //                    }
        //                }




        //                var filterModel = new DataToGenerateModel { ProjectID = FormatExtension.ConvertStringToGuid(QC_UnitCheckList.ProjectID) 
        //                                                          , UnitID = FormatExtension.ConvertStringToGuid(QC_UnitCheckList.UnitID), 
        //                                                            QCUnitCheckListID = FormatExtension.ConvertStringToGuid(model.QCUnitCheckListID) };

        //                if (model.ActionType == "submit")
        //                {
        //                    DataGenerateQCPDFResp dataForGenQCPdf = _generatePDFRepo.GetDataQCToGeneratePDF(filterModel);
        //                    DataDocumentModel genDocumentNo = _generatePDFRepo.GenerateDocumentNO(FormatExtension.ConvertStringToGuid(QC_UnitCheckList.ProjectID));
        //                    Guid NewGuid = Guid.NewGuid();
        //                    string result = _generatePDFRepo.GenerateQCPDF(NewGuid, dataForGenQCPdf, genDocumentNo);
        //                    var newResourcePDF = new tm_Resource
        //                    {
        //                        ID = Guid.NewGuid(),
        //                        FileName = FormatExtension.NullToString(NewGuid),
        //                        FilePath = result,
        //                        MimeType = "file/pdf",
        //                        FlagActive = true,
        //                        CreateDate = DateTime.Now,
        //                        CreateBy = model.UserID,
        //                        UpdateDate = DateTime.Now,
        //                        UpdateBy = model.UserID
        //                    };
        //                    _context.tm_Resource.Add(newResourcePDF);
        //                    var newFormResource = new tr_Document
        //                    {
        //                        ID = Guid.NewGuid(),
        //                        QCUnitCheckListID = model.QCUnitCheckListID,
        //                        ResourceID = newResourcePDF.ID,
        //                        DocumentNo = genDocumentNo.documentNo,
        //                        DocumentPrefix = genDocumentNo.documentPrefix,
        //                        DocuementRunning = genDocumentNo.documentRunning,
        //                        FlagActive = true,
        //                        CreateDate = DateTime.Now,
        //                        CreateBy = model.UserID,
        //                        UpdateDate = DateTime.Now,
        //                        UpdateBy = model.UserID
        //                    };
        //                    _context.tr_Document.Add(newFormResource);
        //                }

        //                _context.SaveChanges();

        //            }
        //            scope.Complete(); // Commit the transaction
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("แก้ไขข้อมูลลง tr_QC_UnitCheckList_Defect ไม่สำเร็จ", ex);
        //        }
        //    }
        //}

        public void SaveSubmitQC5UnitCheckList(QC5SaveSubmitModel model)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(10)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    // Step 1: Update the QC Unit CheckList and related entities
                    var QC_UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(d => d.ID == model.QCUnitCheckListID);

                    if (QC_UnitCheckList != null)
                    {
                        UpdateQCUnitCheckList(QC_UnitCheckList, model);
                        UpdateQCAction(QC_UnitCheckList, model);

                        if (model.Images != null && model.Images.Count > 0)
                        {
                            SaveImages(model, QC_UnitCheckList);
                        }

                        _context.SaveChanges();  // Commit the updates to the database before moving on

                        // Step 2: Generate the PDF after the database updates are successful
                        if (model.ActionType == "submit")
                        {
                            GenerateAndSaveQCPDF(model, QC_UnitCheckList);  // This must succeed or else roll back
                        }
                    }

                    scope.Complete();  // Commit the transaction if all is successful
                }
                catch (Exception ex)
                {
                    // Rollback everything if an error occurs
                    throw new Exception("Failed to update QC Unit CheckList or generate the PDF", ex);
                }
            }
        }

        private void UpdateQCUnitCheckList(tr_QC_UnitCheckList QC_UnitCheckList, QC5SaveSubmitModel model)
        {
            QC_UnitCheckList.QCStatusID = model.QCStatusID;
            QC_UnitCheckList.UpdateDate = DateTime.Now;
            QC_UnitCheckList.UpdateBy = model.UserID;
            _context.tr_QC_UnitCheckList.Update(QC_UnitCheckList);
        }

        private void UpdateQCAction(tr_QC_UnitCheckList QC_UnitCheckList, QC5SaveSubmitModel model)
        {
            var QC_UnitCheckList_Action = _context.tr_QC_UnitCheckList_Action.FirstOrDefault(d => d.ID == model.QCUnitCheckListActionID);

            if (QC_UnitCheckList_Action != null)
            {
                QC_UnitCheckList_Action.ActionType = model.ActionType;
                QC_UnitCheckList_Action.Remark = !string.IsNullOrEmpty(model.QCRemark)
                    ? model.QCRemark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now)
                    : "";
                QC_UnitCheckList_Action.ActionDate = DateTime.Now;
                QC_UnitCheckList_Action.UpdateDate = DateTime.Now;
                QC_UnitCheckList_Action.UpdateBy = model.UserID;
                _context.tr_QC_UnitCheckList_Action.Update(QC_UnitCheckList_Action);
            }
        }

        private void SaveImages(QC5SaveSubmitModel model, tr_QC_UnitCheckList QC_UnitCheckList)
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
                    var newResource = SaveImageAndGetResource(image, model, dirPath, folder);
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

        private tm_Resource SaveImageAndGetResource(IFormFile image, QC5SaveSubmitModel model, string dirPath, string folder)
        {
            Guid guidId = Guid.NewGuid();
            string fileName = guidId + ".jpg";
            var filePath = Path.Combine(dirPath, fileName);

            // Resize and save the image
            using (var imageStream = image.OpenReadStream())
            {
                using (var resizedImageStream = ResizeImage(imageStream, 0.7)) // Resize to 70%
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        resizedImageStream.CopyTo(fileStream); // Save resized image
                    }
                }
            }

            string relativeFilePath = Path.Combine("Upload", "document", folder, "QC5Image", fileName).Replace("\\", "/");

            return new tm_Resource
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
        }

        private void GenerateAndSaveQCPDF(QC5SaveSubmitModel model, tr_QC_UnitCheckList QC_UnitCheckList)
        {
            try
            {
                var filterModel = new DataToGenerateModel
                {
                    ProjectID = FormatExtension.ConvertStringToGuid(QC_UnitCheckList.ProjectID),
                    UnitID = FormatExtension.ConvertStringToGuid(QC_UnitCheckList.UnitID),
                    QCUnitCheckListID = FormatExtension.ConvertStringToGuid(model.QCUnitCheckListID)
                };

                DataGenerateQCPDFResp dataForGenQCPdf = _generatePDFRepo.GetDataQCToGeneratePDF(filterModel);
                DataDocumentModel genDocumentNo = _generatePDFRepo.GenerateDocumentNO(FormatExtension.ConvertStringToGuid(QC_UnitCheckList.ProjectID));
                Guid NewGuid = Guid.NewGuid();
                string result = _generatePDFRepo.GenerateQCPDF(NewGuid, dataForGenQCPdf, genDocumentNo);

                var newResourcePDF = new tm_Resource
                {
                    ID = Guid.NewGuid(),
                    FileName = FormatExtension.NullToString(NewGuid),
                    FilePath = result,
                    MimeType = "file/pdf",
                    FlagActive = true,
                    CreateDate = DateTime.Now,
                    CreateBy = model.UserID,
                    UpdateDate = DateTime.Now,
                    UpdateBy = model.UserID
                };
                _context.tm_Resource.Add(newResourcePDF);

                // Find the existing tr_Document where QCUnitCheckListID matches and FlagActive is true
                var existingDocument = _context.tr_Document.FirstOrDefault(d => d.QCUnitCheckListID == model.QCUnitCheckListID && d.FlagActive == true);

                if (existingDocument != null)
                {
                    // Update the existing document
                    existingDocument.ResourceID = newResourcePDF.ID;
                    existingDocument.DocumentNo = genDocumentNo.documentNo;
                    existingDocument.DocumentPrefix = genDocumentNo.documentPrefix;
                    existingDocument.DocuementRunning = genDocumentNo.documentRunning;
                    existingDocument.FlagActive = true;
                    existingDocument.UpdateDate = DateTime.Now;
                    existingDocument.UpdateBy = model.UserID;

                    // Update the document in the context
                    _context.tr_Document.Update(existingDocument);
                }
                else
                {
                    // If no existing document, insert a new one
                    var newFormResource = new tr_Document
                    {
                        ID = Guid.NewGuid(),
                        QCUnitCheckListID = model.QCUnitCheckListID,
                        ResourceID = newResourcePDF.ID,
                        DocumentNo = genDocumentNo.documentNo,
                        DocumentPrefix = genDocumentNo.documentPrefix,
                        DocuementRunning = genDocumentNo.documentRunning,
                        FlagActive = true,
                        CreateDate = DateTime.Now,
                        CreateBy = model.UserID,
                        UpdateDate = DateTime.Now,
                        UpdateBy = model.UserID
                    };

                    // Add the new document to the context
                    _context.tr_Document.Add(newFormResource);
                }


                _context.SaveChanges();  // Commit after generating the PDF
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate the PDF, rolling back transaction", ex);
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

                    var UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(x => x.ProjectID == model.ProjectID && x.UnitID == model.UnitID && x.QCTypeID == SystemConstant.QcTypeID.QC5 && x.Seq == model.Seq.ToInt());
                    if(UnitCheckList!= null && existingDefect != null)
                    {
                        if (UnitCheckList.QCStatusID == 1)
                        {
                            UnitCheckList.QCStatusID = null;
                        }
                        else if (UnitCheckList.QCStatusID == 4 && existingDefect.IsMajorDefect == true)
                        {
                            UnitCheckList.QCStatusID = null;
                        }
                        UnitCheckList.UpdateDate = DateTime.Now;
                        UnitCheckList.UpdateBy = model.UserID;
                        _context.tr_QC_UnitCheckList.Update(UnitCheckList);
                    }

                    scope.Complete(); // Commit the transaction
                }
                catch (Exception ex)
                {
                    throw new Exception("แก้ไขข้อมูลลง tr_QC_UnitCheckList_Defect ไม่สำเร็จ", ex);
                }
            }
        }


        public (string filePath, string currentDate) SaveSignature(SignatureQC5 signData, string? appPath, Guid? QCUnitCheckListID, Guid? userID)
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

            // Return file path and current date
            string currentDate = FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now);
            return (resource.ResourceStoragePath, currentDate);
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

            var QC_UnitCheckList = _context.tr_QC_UnitCheckList.FirstOrDefault(d => d.ID == QCUnitCheckListID);

            if (QC_UnitCheckList != null)
            {
                QC_UnitCheckList.PESignResourceID = ResourceID;
                _context.tr_QC_UnitCheckList.Update(QC_UnitCheckList);
            }


            var QCUnitCheckListResource = _context.tr_QC_UnitCheckList_Resource.FirstOrDefault(r => r.QCUnitCheckListID == QCUnitCheckListID && r.IsSign == true);

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

        public SummaryQCPdfData GetSummaryQC5(Guid QCUnitCheckListID)
        {
            var refSeqCounts = _context.tr_QC_UnitCheckList_Defect
                      .Where(t1 => t1.QCUnitCheckListID == QCUnitCheckListID && t1.FlagActive == true)
                      .GroupBy(t1 => t1.RefSeq)
                      .Select(g => new ListCalDefectBySeq
                      {
                          RefSeq = g.Key,
                          RefSeqCnt = g.Count()
                      }).ToList();

            // Status counts (Pass and NotPass)
            var statusCounts = _context.tr_QC_UnitCheckList_Defect
                .Where(t1 => t1.QCUnitCheckListID == QCUnitCheckListID && t1.FlagActive == true)
                .GroupBy(t1 => 1) // Single group to calculate total counts
                .Select(g => new
                {
                    Cnt_Pass = g.Count(t1 => t1.StatusID == 27),  // StatusID = 27 is Pass
                    Cnt_NotPass = g.Count(t1 => t1.StatusID == 28) // StatusID = 28 is Not Pass
                })
                .FirstOrDefault();

            // Total defect count
            var cntAll = _context.tr_QC_UnitCheckList_Defect
                .Where(t1 => t1.QCUnitCheckListID == QCUnitCheckListID && t1.FlagActive == true)
                .Count();

            if (refSeqCounts != null && statusCounts != null && cntAll > 0)
            {
                var resultSummary = new SummaryQCPdfData
                {
                    SumAllDefect = cntAll,
                    SumPassDefect = statusCounts.Cnt_Pass,
                    SumNotPassDefect = statusCounts.Cnt_NotPass,
                    CalDefectBySeq = refSeqCounts // List of RefSeq counts
                };

                return resultSummary;
            }
            else
            {
                return null;
            }
        }

    }
}
