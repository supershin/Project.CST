using System;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models.QCModel;
using QuestPDF.Infrastructure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IQcCheckListRepo
	{
        MasterQcCheckListDetailResp GetMasterQcCheckList(Guid projectID, int qcTypeID);

        QcCheckListResp ValidateQcCheckList(QcActionModel model);

        VerifyQcCheckList VerifyQcCheckList(QcActionModel model);

        QcCheckListDetailResp GetQcCheckListDetail(GetDetailModel model);

        DuplicateModelResp CreateDuplicateQcCheckList(Guid qcCheckListID, int seq, Guid userID);

        dynamic GetAnotherValue(Guid? id, Guid projectID, Guid unitID, int qcTypeID);

        dynamic SaveQcCheckList(SaveTransQCCheckListModel model, Guid userID, int roleID);

        Guid UploadSignResource(Guid? QcID, string model, string appPath, Guid userId);

        bool DeleteImage(Guid qcID, int? detailID, Guid resourceID, Guid userID);
        bool SubmitQcCheckList(SubmitQcCheckListModel model, Guid userID, int roleID);

        string OpenFilePDF(Guid QCCheckListID);
    }

	public class QcCheckListRepo : IQcCheckListRepo
	{
        private readonly ContructionTrackingDbContext _context;
        public QcCheckListRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

        public QcCheckListResp ValidateQcCheckList(QcActionModel model)
        {
            QcCheckListResp resp = new QcCheckListResp();

            var checkQcUnit = from qc in _context.tr_QC_UnitCheckList
                              where qc.ProjectID == model.ProjectID
                              && qc.UnitID == model.UnitID
                              && qc.CheckListID == model.QcCheckListID
                              && qc.QCTypeID == model.QcTypeID
                              && qc.FlagActive == true
                              select new
                              {
                                  qc
                              };
            // validate by click status qc
            if (model.Seq != null)
            {
                var query = checkQcUnit.Where(o => o.qc.Seq == model.Seq).FirstOrDefault();

                // get value follow seq
                resp = new QcCheckListResp()
                {
                    ID = query.qc.ID,
                    ProjectID = query.qc.ProjectID.GetValueOrDefault(),
                    UnitID = query.qc.UnitID.GetValueOrDefault(),
                    QcCheckListID = query.qc.CheckListID.GetValueOrDefault(),
                    QcTypeID = query.qc.QCTypeID.GetValueOrDefault(),
                    Seq = query.qc.Seq.GetValueOrDefault(),

                    QcStatusID = query.qc.QCStatusID.GetValueOrDefault(),
                };
            }
            // validate by click button qc
            else
            {
                var query = checkQcUnit.OrderByDescending(o => o.qc.Seq).FirstOrDefault();
                if (query != null)
                {
                    var action = _context.tr_QC_UnitCheckList_Action.Where(o => o.QCUnitCheckListID == query.qc.ID).FirstOrDefault();

                    // get value last update
                    resp = new QcCheckListResp()
                    {
                        ID = query.qc.ID,
                        ProjectID = query.qc.ProjectID.GetValueOrDefault(),
                        UnitID = query.qc.UnitID.GetValueOrDefault(),
                        QcCheckListID = query.qc.CheckListID.GetValueOrDefault(),
                        QcTypeID = query.qc.QCTypeID.GetValueOrDefault(),
                        Seq = query.qc.Seq.GetValueOrDefault(),

                        QcStatusID = query.qc.QCStatusID.GetValueOrDefault(),
                        QcActionType = action.ActionType,
                    };  
                }
                else
                {
                    // get new value
                    resp = new QcCheckListResp()
                    {
                        ProjectID = model.ProjectID,
                        UnitID = model.UnitID,
                        QcCheckListID = model.QcCheckListID,
                        QcTypeID = model.QcTypeID,
                        Seq = 1,
                    };
                }
            }

            return resp;
        }

        public MasterQcCheckListDetailResp GetMasterQcCheckList(Guid projectID, int qcTypeID)
        {
            tm_Project? project = _context.tm_Project
                .Where(o => o.ProjectID == projectID && o.FlagActive == true)
                .FirstOrDefault();

            MasterQcCheckListDetailResp resp = new MasterQcCheckListDetailResp();

            if (project != null)
            {
                tm_QC_CheckList? qcCheckList = _context.tm_QC_CheckList
                    .Where(o => o.ProjectTypeID == project.ProjectTypeID
                    && o.QCTypeID == qcTypeID && o.FlagActive == true)
                    .FirstOrDefault();
                
                if (qcCheckList != null)
                {
                    resp = new MasterQcCheckListDetailResp()
                    {
                        QcCheckListID = qcCheckList.ID,
                        QcTypeID = qcCheckList.QCTypeID.GetValueOrDefault(),
                        CheckListDetails = new List<CheckListDetail>() // Changed to a list
                    };

                    // Get checklist details with no parent (root level)
                    List<tm_QC_CheckListDetail> qcCheckListDetail = _context.tm_QC_CheckListDetail
                        .Where(o => o.QCCheckListID == qcCheckList.ID
                        && o.FlagActive == true && o.ParentID == null)
                        .ToList();

                    foreach (var data in qcCheckListDetail)
                    {
                        CheckListDetail detail = new CheckListDetail()
                        {
                            CheckListDetailID = data.ID,
                            Name = data.Name,
                            LineOrder = data.LineOrder.GetValueOrDefault(),
                            ParentDetails = new List<ParentCheckListDetail>() // Changed to a list
                        };

                        // Fetch child checklist details for the current parent
                        List<tm_QC_CheckListDetail> validateDetail = _context.tm_QC_CheckListDetail
                            .Where(o => o.ParentID == data.ID && o.FlagActive == true)
                            .ToList();

                        foreach (var validate in validateDetail)
                        {
                            ParentCheckListDetail parent = new ParentCheckListDetail()
                            {
                                CheckListDetailID = validate.ID,
                                ParentID = validate.ParentID.GetValueOrDefault(),
                                Name = validate.Name,
                                LineOrder = validate.LineOrder.GetValueOrDefault()
                            };

                            // Add the parent checklist detail to the list
                            detail.ParentDetails.Add(parent);
                        }

                        // Add the constructed detail to the response list
                        resp.CheckListDetails.Add(detail);
                    }
                }
            }
            return resp;
        }

        public VerifyQcCheckList VerifyQcCheckList(QcActionModel model)
        {
            VerifyQcCheckList resp = new VerifyQcCheckList();
            //{
            //    MasterQcCheckListDetail = GetMasterQcCheckList(model.ProjectID, model.QcTypeID)
            //};

            tr_QC_UnitCheckList? transQc = _context.tr_QC_UnitCheckList
                .Where(o => o.ProjectID == model.ProjectID && o.UnitID == model.UnitID
                && o.CheckListID == model.QcCheckListID && o.QCTypeID == model.QcTypeID).OrderByDescending(o => o.Seq).FirstOrDefault();

            if( transQc == null)
            {
                //get Default Qc CheckList
                QcCheckListResp QcCheckList = ValidateQcCheckList(model);

                //return Default Qc CheckList
                //resp = new VerifyQcCheckList()
                //{
                //    QcCheckList = QcCheckList,
                //    MasterQcCheckListDetail = masterQcCheckList
                //};
                resp = new VerifyQcCheckList()
                {
                    QcCheckList = QcCheckList
                };
            }
            else
            {
                //get Previous Qc CheckList -> Last Seq
                QcCheckListResp QcCheckList = ValidateQcCheckList(model);

                if(QcCheckList.QcStatusID == SystemConstant.UnitQCStatus.Pass)
                {
                    throw new Exception("รายการตรวจQC นี้ผ่านแล้ว");
                }

                if(QcCheckList.Seq == 5)
                {
                    throw new Exception("ไม่สามารถสร้างรายการตรวจเพิ่มได้เนื่องจากครบจำนวนครั้งที่กำหนด");
                }

                //verify submit Previous Qc CheckList
                tr_QC_UnitCheckList_Action? qcAction = _context.tr_QC_UnitCheckList_Action
                    .Where(o => o.QCUnitCheckListID == QcCheckList.ID && o.ActionType == SystemConstant.ActionType.SAVE)
                    .FirstOrDefault();

                //throw error if data not submit 
                if (qcAction != null)
                {
                    throw new Exception("กรุณา Submit รายการตรวจQC ก่อนหน้า");
                }
                //get draft Previous Qc CheckList
                else
                {
                    if(transQc.QCStatusID == SystemConstant.UnitQCStatus.NotPass
                        || transQc.QCStatusID == SystemConstant.UnitQCStatus.IsNotReadyInspect)
                    {
                        resp = new VerifyQcCheckList()
                        {
                            QcCheckList = QcCheckList
                        };
                    }
                }
            }
            return resp;
        }

        public QcCheckListDetailResp GetQcCheckListDetail(GetDetailModel model)
        {
            //get master qc checklist detail 
            QcCheckListDetailResp resp = new QcCheckListDetailResp();
            resp.MasterQcCheckListDetail = GetMasterQcCheckList(model.ProjectID, model.QcTypeID);

            //get QC CheckList if New Data 
            if (model.ID == Guid.Empty || model.ID == null)
            {
                resp.QcCheckList = new QcCheckListResp()
                {
                    ProjectID = model.ProjectID,
                    UnitID = model.UnitID,
                    QcCheckListID = model.QcCheckListID,
                    QcTypeID = model.QcTypeID,
                    Seq = 1
                };
            }
            // get QC CheckList and Detail if New Create or View Seq Only
            else
            {
                var query = (from qcl in _context.tr_QC_UnitCheckList
                             join qcla in _context.tr_QC_UnitCheckList_Action on qcl.ID equals qcla.QCUnitCheckListID
                             join r in _context.tm_Resource on qcl.PESignResourceID equals r.ID into rGroup
                             from r in rGroup.DefaultIfEmpty()
                             where qcl.ID == model.ID && qcl.ProjectID == model.ProjectID
                             && qcl.UnitID == model.UnitID && qcl.CheckListID == model.QcCheckListID
                             && qcl.QCTypeID == model.QcTypeID && qcl.Seq == model.Seq
                             && qcl.FlagActive == true
                             select new
                             {
                                 ID = qcl.ID,
                                 ProjectID = qcl.ProjectID,
                                 UnitID = qcl.UnitID,
                                 QcCheckListID = qcl.CheckListID,
                                 QcTypeID = qcl.QCTypeID,
                                 Seq = qcl.Seq,
                                 QcStatusID = qcl.QCStatusID,
                                 QcActionType = qcla.ActionType,
                                 UserQcResourceUrl = r.FilePath,
                                 MainRemark = qcla.Remark,
                                 MainImages = (from qcclr in _context.tr_QC_UnitCheckList_Resource
                                               join mr in _context.tm_Resource on qcclr.ResourceID equals mr.ID
                                               where qcclr.QCUnitCheckListID == qcl.ID && qcclr.FlagActive == true
                                               && qcclr.QCUnitCheckListDetailID == null
                                               select new
                                               {
                                                   ID = mr.ID,
                                                   ImageUrl = mr.FilePath
                                               }).ToList(),

                                 CheckListDetails = (from qcld in _context.tr_QC_UnitCheckList_Detail
                                                     where qcld.QCUnitCheckListID == qcl.ID && qcld.FlagActive == true
                                                     select new
                                                     {
                                                         CheckListDetailID = qcld.CheckListDetailID,
                                                         DetailStatusID = qcld.StatusID,
                                                         Remark = qcld.Remark,
                                                         ImageList = (from qclr in _context.tr_QC_UnitCheckList_Resource
                                                                      join r in _context.tm_Resource on qclr.ResourceID equals r.ID
                                                                      where qclr.QCUnitCheckListDetailID == qcld.ID && qclr.QCUnitCheckListID == qcl.ID
                                                                      && qclr.FlagActive == true
                                                                      select new
                                                                      {
                                                                          ResourceID = r.ID,
                                                                          FilePath = r.FilePath
                                                                      }).ToList()
                                                     }).ToList()
                             }).FirstOrDefault();
                            
                // Set Data Resp from Query 
                if (query != null)
                {
                    resp.QcCheckList = new QcCheckListResp()
                    {
                        ID = query.ID,
                        ProjectID = (Guid)query.ProjectID,
                        UnitID = (Guid)query.UnitID,
                        QcCheckListID = (int)query.QcCheckListID,
                        QcTypeID = (int)query.QcTypeID,
                        Seq = (int)query.Seq,
                        QcStatusID = query.QcStatusID,
                        UserQcResourceUrl = query.UserQcResourceUrl,
                        QcActionType = query.QcActionType,
                        Remark = query.MainRemark,
                        MainImages = new List<MainImage>()
                    };

                    foreach(var image in query.MainImages)
                    {
                        MainImage addImage = new MainImage()
                        {
                            ImageID = image.ID,
                            ImageUrl = image.ImageUrl
                        };

                        resp.QcCheckList.MainImages.Add(addImage);
                    }

                    resp.QcCheckListDetail = new List<QcCheckListDetail>();

                    foreach(var detail in query.CheckListDetails)
                    {
                        QcCheckListDetail qcDetail = new QcCheckListDetail()
                        {
                            QcCheckListDetailID = (int)detail.CheckListDetailID,
                            StatusID = detail.DetailStatusID,
                            Remark = detail.Remark,
                            Images = new List<ImageQcCheckListDetail>()
                        };

                        foreach( var image in detail.ImageList)
                        {
                            ImageQcCheckListDetail imageDetail = new ImageQcCheckListDetail()
                            {
                                ResourceID = image.ResourceID,
                                FilePath = image.FilePath
                            };

                            qcDetail.Images.Add(imageDetail);
                        }
                        resp.QcCheckListDetail.Add(qcDetail);
                    }
                }
            }
            return resp;
        }

        public DuplicateModelResp CreateDuplicateQcCheckList(Guid qcCheckListID, int seq, Guid userID)
        {
            DuplicateModelResp resp = new DuplicateModelResp();

            // query validate qc checklist 
            tr_QC_UnitCheckList? qcChecklist = _context.tr_QC_UnitCheckList
                .Where(o => o.ID == qcCheckListID && o.Seq == seq && o.FlagActive == true)
                .FirstOrDefault();

            if (qcChecklist != null)
            {
                tr_QC_UnitCheckList_Action? qcCheckListAction = _context.tr_QC_UnitCheckList_Action
                .Where(o => o.QCUnitCheckListID == qcChecklist.ID)
                .FirstOrDefault();

                List<tr_QC_UnitCheckList_Detail> qcCheckListDetail = _context.tr_QC_UnitCheckList_Detail
                    .Where(o => o.QCUnitCheckListID == qcChecklist.ID && o.FlagActive == true)
                    .ToList();

                List<tr_QC_UnitCheckList_Resource> qcCheckListResource = _context.tr_QC_UnitCheckList_Resource
                    .Where(o => o.QCUnitCheckListID == qcChecklist.ID && o.FlagActive == true)
                    .ToList();

                // create draft new qc checklist
                Guid guid = Guid.NewGuid();
                tr_QC_UnitCheckList createNew = new tr_QC_UnitCheckList()
                {
                    ID = guid,
                    ProjectID = qcChecklist.ProjectID,
                    UnitID = qcChecklist.UnitID,
                    CheckListID = qcChecklist.CheckListID,
                    QCTypeID = qcChecklist.QCTypeID,
                    Seq = qcChecklist.Seq + 1,
                    CheckListDate = qcChecklist.CheckListDate,
                    QCStatusID = qcChecklist.QCStatusID,
                    PESignResourceID = qcChecklist.PESignResourceID,
                    FlagActive = qcChecklist.FlagActive,
                    CreateDate = DateTime.Now,
                    CreateBy = userID,
                    UpdateDate = DateTime.Now,
                    UpdateBy = userID
                };

                _context.tr_QC_UnitCheckList.Add(createNew);
                _context.SaveChanges();

                List<tr_QC_UnitCheckList_Resource> selectMainResource = qcCheckListResource
                        .Where(o => o.QCUnitCheckListDetailID == null && o.FlagActive == true)
                        .ToList();

                foreach (var resource in selectMainResource)
                {
                    tr_QC_UnitCheckList_Resource newMainResource = new tr_QC_UnitCheckList_Resource();
                    newMainResource.QCUnitCheckListID = createNew.ID;
                    newMainResource.QCUnitCheckListDetailID = null;
                    newMainResource.ResourceID = resource.ResourceID;
                    newMainResource.FlagActive = true;
                    newMainResource.UpdateDate = DateTime.Now;
                    newMainResource.UpdateBy = userID;
                    newMainResource.CreateDate = DateTime.Now;
                    newMainResource.CreateBy = userID;

                    _context.tr_QC_UnitCheckList_Resource.Add(newMainResource);
                    _context.SaveChanges();
                }

                // create draft new qc checklist action
                tr_QC_UnitCheckList_Action createNewAction = new tr_QC_UnitCheckList_Action()
                {
                    QCUnitCheckListID = createNew.ID,
                    RoleID = qcCheckListAction.RoleID,
                    ActionType = SystemConstant.ActionType.SAVE,
                    StatusID = qcCheckListAction.StatusID,
                    Remark = qcCheckListAction.Remark,
                    ActionDate = qcCheckListAction.ActionDate,
                    CreateDate = DateTime.Now,
                    CreateBy = userID,
                    UpdateDate = DateTime.Now,
                    UpdateBy = userID
                };

                _context.tr_QC_UnitCheckList_Action.Add(createNewAction);
                _context.SaveChanges();

                // create draft new qc checklist list detail

                foreach(var list in qcCheckListDetail)
                {
                    tr_QC_UnitCheckList_Detail newDetail = new tr_QC_UnitCheckList_Detail();
                    newDetail.QCUnitCheckListID = createNew.ID;
                    newDetail.CheckListID = list.CheckListID;
                    newDetail.CheckListDetailID = list.CheckListDetailID;
                    newDetail.StatusID = list.StatusID;
                    newDetail.Remark = list.Remark;
                    newDetail.PassBySeq = list.PassBySeq;
                    newDetail.FlagActive = list.FlagActive;
                    newDetail.CreateDate = DateTime.Now;
                    newDetail.CreateBy = userID;
                    newDetail.UpdateDate = DateTime.Now;
                    newDetail.UpdateBy = userID;

                    _context.tr_QC_UnitCheckList_Detail.Add(newDetail);
                    _context.SaveChanges();

                    List<tr_QC_UnitCheckList_Resource> selectDetailResource = qcCheckListResource
                        .Where(o => o.QCUnitCheckListDetailID == list.ID && o.FlagActive == true)
                        .ToList();
                    
                    foreach(var resource in selectDetailResource)
                    {
                        tr_QC_UnitCheckList_Resource newDetailResource = new tr_QC_UnitCheckList_Resource();
                        newDetailResource.QCUnitCheckListID = createNew.ID;
                        newDetailResource.QCUnitCheckListDetailID = newDetail.ID;
                        newDetailResource.ResourceID = resource.ResourceID;
                        newDetailResource.FlagActive = true;
                        newDetailResource.UpdateDate = DateTime.Now;
                        newDetailResource.UpdateBy = userID;
                        newDetailResource.CreateDate = DateTime.Now;
                        newDetailResource.CreateBy = userID;

                        _context.tr_QC_UnitCheckList_Resource.Add(newDetailResource);
                        _context.SaveChanges();
                    }

                    _context.SaveChanges();
                }

                resp = new DuplicateModelResp()
                {
                    QCCheckListID = createNew.ID,
                    Seq = (int)createNew.Seq
                    
                };
            }
            else
            {
                throw new Exception("ไม่พบข้อมูลรายการตรวจสอบ QC");
            }

            return resp;
        }

        public dynamic GetAnotherValue(Guid? id, Guid projectID, Guid unitID, int qcTypeID)
        {
            var query = (from qccl in _context.tr_QC_UnitCheckList
                         join user in _context.tm_User on qccl.UpdateBy equals user.ID
                         where qccl.ID == id && qccl.FlagActive == true
                         && qccl.ProjectID == projectID && qccl.UnitID == unitID
                         select new
                         {
                             QCFirstName = user.FirstName,
                             QCLastName = user.LastName
                         }).FirstOrDefault();

            var query2 = (from ext in _context.tm_Ext
                          where ext.ID == qcTypeID && ext.FlagActive == true
                          select new
                          {
                              Name = ext.Name
                          }).FirstOrDefault();

            var query3 = (from peu in _context.tr_PE_Unit
                          join user in _context.tm_User on peu.UserID equals user.ID
                          where peu.UnitID == unitID && peu.FlagActive == true
                          select new
                          {
                              PEID = user.ID,
                              PEFirstName = user.FirstName,
                              PELastName = user.LastName
                          }).FirstOrDefault();

            var resp = new
            {
                QCName = query != null ? query.QCFirstName + " " + query.QCLastName : null,
                QCNumber = query2.Name,
                PEName = query3 != null ? query3.PEFirstName + " " + query3.PELastName : null,
                PEID = query3.PEID
            };

            return resp;
        }

        public dynamic SaveQcCheckList(SaveTransQCCheckListModel model, Guid userID, int roleID)
        {
            if(model.QcID == Guid.Empty || model.QcID == null)
            {
                // create new QC CheckList
                tr_QC_UnitCheckList createNewQcCheckList = new tr_QC_UnitCheckList();
                createNewQcCheckList.ID = Guid.NewGuid();
                createNewQcCheckList.ProjectID = model.ProjectID;
                createNewQcCheckList.UnitID = model.UnitID;
                createNewQcCheckList.CheckListID = model.CheckListID;
                createNewQcCheckList.QCTypeID = model.QCTypeID;
                createNewQcCheckList.Seq = 1;
                createNewQcCheckList.CheckListDate = DateTime.Now;
                if (model.IsNotReadyInspect == false)
                    createNewQcCheckList.QCStatusID = SystemConstant.UnitQCStatus.InProgress;
                else
                    createNewQcCheckList.QCStatusID = SystemConstant.UnitQCStatus.IsNotReadyInspect;
                if (model.PeSignResourceID != null)
                    createNewQcCheckList.PESignResourceID = model.PeSignResourceID;
                createNewQcCheckList.FlagActive = true;
                createNewQcCheckList.UpdateDate = DateTime.Now;
                createNewQcCheckList.UpdateBy = userID;
                createNewQcCheckList.CreateDate = DateTime.Now;
                createNewQcCheckList.CreateBy = userID;

                _context.tr_QC_UnitCheckList.Add(createNewQcCheckList);
                _context.SaveChanges();

                // create new QC CheckList Action
                tr_QC_UnitCheckList_Action createNewCheckListAction = new tr_QC_UnitCheckList_Action();
                createNewCheckListAction.QCUnitCheckListID = createNewQcCheckList.ID;
                createNewCheckListAction.RoleID = roleID;
                createNewCheckListAction.ActionType = SystemConstant.ActionType.SAVE;
                createNewCheckListAction.Remark = model.Remark;
                createNewCheckListAction.ActionDate = DateTime.Now;
                createNewCheckListAction.UpdateDate = DateTime.Now;
                createNewCheckListAction.UpdateBy = userID;
                createNewCheckListAction.CreateDate = DateTime.Now;
                createNewCheckListAction.CreateBy = userID;

                _context.tr_QC_UnitCheckList_Action.Add(createNewCheckListAction);
                _context.SaveChanges();

                // upload qc checklist info image update table qc resource
                UploadImageResource((Guid)createNewCheckListAction.QCUnitCheckListID, null, model.Images, model.ApplicationPath, userID);

                // create qc checklist detial
                foreach ( var dataList in model.CheckListItems)
                {
                    tr_QC_UnitCheckList_Detail createNewDetail = new tr_QC_UnitCheckList_Detail();
                    createNewDetail.QCUnitCheckListID = createNewQcCheckList.ID;
                    createNewDetail.CheckListID = createNewQcCheckList.CheckListID;
                    createNewDetail.CheckListDetailID = dataList.CheckListDetailID;
                    if (dataList.ConditionPass)
                    {
                        createNewDetail.StatusID = SystemConstant.Qc_CheckList_Status.PASS;
                        createNewDetail.PassBySeq = createNewQcCheckList.Seq;
                    }    
                    else if (dataList.ConditionNotPass)
                    {
                        createNewDetail.StatusID = SystemConstant.Qc_CheckList_Status.NOTPASS;
                        createNewDetail.PassBySeq = 0;
                    }
                    createNewDetail.Remark = dataList.DetailRemark;
                    createNewDetail.UpdateDate = DateTime.Now;
                    createNewDetail.UpdateBy = userID;
                    createNewDetail.CreateDate = DateTime.Now;
                    createNewDetail.CreateBy = userID;

                    _context.tr_QC_UnitCheckList_Detail.Add(createNewDetail);
                    _context.SaveChanges();

                    // upload qc checklist detail image and update table qc resource
                    UploadImageResource((Guid)createNewDetail.QCUnitCheckListID, createNewDetail.ID, dataList.DetailImage, model.ApplicationPath, userID);
                }

                var dataResp = new
                {
                    QcID = createNewQcCheckList.ID,
                    ProjectID = createNewQcCheckList.ProjectID,
                    UnitID = createNewQcCheckList.UnitID,
                    CheckListID = createNewQcCheckList.CheckListID,
                    QcTypeID = createNewQcCheckList.QCTypeID,
                    Seq = createNewQcCheckList.Seq,
                };

                return dataResp;
            }
            else
            {
                // update
                tr_QC_UnitCheckList? updateQcCheckList = _context.tr_QC_UnitCheckList
                    .Where(o => o.ID == model.QcID && o.FlagActive == true)
                    .FirstOrDefault();

                if (updateQcCheckList == null) throw new Exception("ไม่พบข้อมูลรายการตรวจสอบ QC");
                else
                {
                    if (model.IsNotReadyInspect == false)
                        updateQcCheckList.QCStatusID = SystemConstant.UnitQCStatus.InProgress;
                    else
                        updateQcCheckList.QCStatusID = SystemConstant.UnitQCStatus.IsNotReadyInspect;
                    if (model.PeSignResourceID != Guid.Empty && model.PeSignResourceID != null)
                        updateQcCheckList.PESignResourceID = model.PeSignResourceID;
                    updateQcCheckList.CheckListDate = DateTime.Now;
                    updateQcCheckList.UpdateDate = DateTime.Now;
                    updateQcCheckList.UpdateBy = userID;

                    _context.tr_QC_UnitCheckList.Update(updateQcCheckList);

                    tr_QC_UnitCheckList_Action? updateQcCheckListAction = _context.tr_QC_UnitCheckList_Action
                        .Where(o => o.QCUnitCheckListID == updateQcCheckList.ID)
                        .FirstOrDefault();

                    if (updateQcCheckListAction == null) throw new Exception("ไม่พบข้อมูลการทำงานของรายการตรวจสอบ QC");
                    else
                    {
                        updateQcCheckListAction.Remark = model.Remark;
                        updateQcCheckListAction.ActionDate = DateTime.Now;
                        updateQcCheckListAction.UpdateDate = DateTime.Now;
                        updateQcCheckListAction.UpdateBy = userID;

                        _context.tr_QC_UnitCheckList_Action.Update(updateQcCheckListAction);
                    }

                    // upload qc checklist info image update table qc resource
                    UploadImageResource((Guid)updateQcCheckList.ID, null, model.Images, model.ApplicationPath, userID);

                    List<int> selectMasterDetail = _context.tm_QC_CheckListDetail
                        .Where(o => o.QCCheckListID == updateQcCheckList.CheckListID)
                        .Select(o => o.ID)
                        .ToList();
                    List<int> detailList = model.CheckListItems.Select(o => o.CheckListDetailID).ToList();

                    List<int> exceptDetail = selectMasterDetail.Except(detailList).ToList();

                    foreach ( var dataDetail in model.CheckListItems)
                    {
                        tr_QC_UnitCheckList_Detail? qcCheckListDetail = _context.tr_QC_UnitCheckList_Detail
                            .Where(o => o.QCUnitCheckListID == updateQcCheckList.ID
                            && o.CheckListDetailID == dataDetail.CheckListDetailID && o.FlagActive == true)
                            .FirstOrDefault();

                        if(qcCheckListDetail == null)
                        {
                            tr_QC_UnitCheckList_Detail createNewDetail = new tr_QC_UnitCheckList_Detail();
                            createNewDetail.QCUnitCheckListID = updateQcCheckList.ID;
                            createNewDetail.CheckListID = updateQcCheckList.CheckListID;
                            createNewDetail.CheckListDetailID = dataDetail.CheckListDetailID;
                            if (dataDetail.ConditionPass)
                            {
                                createNewDetail.StatusID = SystemConstant.Qc_CheckList_Status.PASS;
                                createNewDetail.PassBySeq = updateQcCheckList.Seq;
                            }                                
                            else if (dataDetail.ConditionNotPass)
                            {
                                createNewDetail.StatusID = SystemConstant.Qc_CheckList_Status.NOTPASS;
                                createNewDetail.PassBySeq = 0;
                            }
                            else
                                createNewDetail.StatusID = null;
                            createNewDetail.Remark = dataDetail.DetailRemark;
                            createNewDetail.UpdateDate = DateTime.Now;
                            createNewDetail.UpdateBy = userID;
                            createNewDetail.CreateDate = DateTime.Now;
                            createNewDetail.CreateBy = userID;

                            _context.tr_QC_UnitCheckList_Detail.Add(createNewDetail);
                            _context.SaveChanges();

                            // upload qc checklist detail image and update table qc resource
                            UploadImageResource((Guid)createNewDetail.QCUnitCheckListID, createNewDetail.ID, dataDetail.DetailImage, model.ApplicationPath, userID);
                        }
                        else
                        {
                            if (dataDetail.ConditionPass)
                            {
                                if(qcCheckListDetail.StatusID != SystemConstant.Qc_CheckList_Status.PASS)
                                {
                                    qcCheckListDetail.StatusID = SystemConstant.Qc_CheckList_Status.PASS;
                                    qcCheckListDetail.PassBySeq = updateQcCheckList.Seq;
                                }
                                
                            }
                            else if (dataDetail.ConditionNotPass)
                            {
                                if (qcCheckListDetail.StatusID != SystemConstant.Qc_CheckList_Status.NOTPASS)
                                {
                                    qcCheckListDetail.StatusID = SystemConstant.Qc_CheckList_Status.NOTPASS;
                                    qcCheckListDetail.PassBySeq = 0;
                                }
                            }
                            else
                                qcCheckListDetail.StatusID = null;
                                
                            qcCheckListDetail.Remark = dataDetail.DetailRemark;
                            qcCheckListDetail.UpdateDate = DateTime.Now;
                            qcCheckListDetail.UpdateBy = userID;

                            _context.tr_QC_UnitCheckList_Detail.Update(qcCheckListDetail);
                            _context.SaveChanges();

                            // upload qc checklist detail image and update table qc resource
                            UploadImageResource((Guid)qcCheckListDetail.QCUnitCheckListID, qcCheckListDetail.ID, dataDetail.DetailImage, model.ApplicationPath, userID);

                        }
                    }

                    foreach ( var data in exceptDetail)
                    {
                        tr_QC_UnitCheckList_Detail? qcCheckListDetail = _context.tr_QC_UnitCheckList_Detail
                            .Where(o => o.QCUnitCheckListID == updateQcCheckList.ID
                            && o.CheckListDetailID == data && o.FlagActive == true)
                            .FirstOrDefault();

                        if(qcCheckListDetail != null)
                        {
                            qcCheckListDetail.StatusID = null;
                            qcCheckListDetail.UpdateDate = DateTime.Now;
                            qcCheckListDetail.UpdateBy = userID;
                            _context.tr_QC_UnitCheckList_Detail.Update(qcCheckListDetail);
                            _context.SaveChanges();
                        }
                    }
                    _context.SaveChanges();
                }

                var dataResp = new
                {
                    QcID = updateQcCheckList.ID,
                    ProjectID = updateQcCheckList.ProjectID,
                    UnitID = updateQcCheckList.UnitID,
                    CheckListID = updateQcCheckList.CheckListID,
                    QcTypeID = updateQcCheckList.QCTypeID,
                    Seq = updateQcCheckList.Seq,
                };

                return dataResp;
            }
        }

        void UploadImageResource(Guid qcUnitCheckListID, int? detailID, List<IFormFile> listImages, string applicationPath, Guid userID)
        {
            if(listImages != null && listImages.Count > 0)
            {
                var folder = DateTime.Now.ToString("yyyyMM");
                var dirPath = Path.Combine(applicationPath, "Upload", "document", folder, "QCImage");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                foreach( var image in listImages)
                {
                    if(image.Length > 0)
                    {
                        Guid guidId = Guid.NewGuid();
                        string fileName = guidId + ".jpg";
                        var filePath = Path.Combine(dirPath, fileName);

                        //// Resize and save the image
                        //using (var imageStream = image.OpenReadStream())
                        //{
                        //    using (var resizedImageStream = ResizeImage(imageStream, 0.7)) // Resize to 50%
                        //    {
                        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //        {
                        //            resizedImageStream.CopyTo(fileStream); // Save resized image
                        //        }
                        //    }
                        //}
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                        }

                        string relativeFilePath = Path.Combine("Upload", "document", folder, "QCImage", fileName).Replace("\\", "/");

                        var newResource = new tm_Resource
                        {
                            ID = Guid.NewGuid(),
                            FileName = fileName,
                            FilePath = relativeFilePath,
                            MimeType = "image/jpeg",
                            FlagActive = true,
                            CreateDate = DateTime.Now,
                            CreateBy = userID,
                            UpdateDate = DateTime.Now,
                            UpdateBy = userID
                        };
                        _context.tm_Resource.Add(newResource);

                        var newQCUnitCheckListResource = new tr_QC_UnitCheckList_Resource
                        {
                            QCUnitCheckListID = qcUnitCheckListID,
                            QCUnitCheckListDetailID = detailID != null ? detailID : null,
                            ResourceID = newResource.ID,
                            FlagActive = true,
                            CreateDate = DateTime.Now,
                            CreateBy = userID,
                            UpdateDate = DateTime.Now,
                            UpdateBy = userID
                        };
                        _context.tr_QC_UnitCheckList_Resource.Add(newQCUnitCheckListResource);
                    }
                }
                _context.SaveChanges();
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
        public Guid UploadSignResource(Guid? QcID, string model, string appPath, Guid userId)
        {
            Resources resource = new Resources();
            Guid guidId = Guid.NewGuid();
            string filePath = "";
            Guid returnID = Guid.Empty;
            if (model != null)
            {
                string fileName = guidId + ".jpg";

                var folder = DateTime.Now.ToString("yyyyMM");
                var dirPath = $"Upload/document/{folder}/sign/";
                filePath = dirPath + fileName;

                resource.PhysicalPathServer = appPath;
                resource.ResourceStorageBase64 = model;
                resource.ResourceStoragePath = filePath;
                resource.Directory = Path.Combine(appPath, dirPath);
                ConvertByteToImage(resource);

                returnID = CreateUploadSign(QcID ,fileName, filePath, userId);
            }

            return returnID;
        }
        private void ConvertByteToImage(Resources item)
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

        public Guid CreateUploadSign(Guid? QcID, string fileName, string filePath, Guid userID )
        {
            tr_QC_UnitCheckList? checkQc = _context.tr_QC_UnitCheckList
                .Where(o => o.ID == QcID && o.FlagActive == true).FirstOrDefault();

            if (checkQc == null)
            {
                tm_Resource createResource = new tm_Resource();
                createResource.ID = Guid.NewGuid();
                createResource.FileName = fileName;
                createResource.FilePath = filePath;
                createResource.MimeType = "image/jpg";
                createResource.FlagActive = true;
                createResource.CreateDate = DateTime.Now;
                createResource.UpdateDate = DateTime.Now;
                createResource.CreateBy = userID;
                createResource.UpdateBy = userID;

                _context.tm_Resource.Add(createResource);
                _context.SaveChanges();

                return createResource.ID;
            }
            else
            {
                Guid newGuid = Guid.Empty;

                tm_Resource? resource = _context.tm_Resource
                                        .Where(o => o.ID == checkQc.PESignResourceID
                                        && o.FlagActive == true).FirstOrDefault();
                if (resource != null)
                {
                    resource.FlagActive = false;
                    resource.UpdateDate = DateTime.Now;
                    resource.UpdateBy = userID;

                    _context.tm_Resource.Update(resource);

                    tm_Resource createResource = new tm_Resource();
                    createResource.ID = Guid.NewGuid();
                    createResource.FileName = fileName;
                    createResource.FilePath = filePath;
                    createResource.MimeType = "image/jpg";
                    createResource.FlagActive = true;
                    createResource.CreateDate = DateTime.Now;
                    createResource.UpdateDate = DateTime.Now;
                    createResource.CreateBy = userID;
                    createResource.UpdateBy = userID;

                    _context.tm_Resource.Add(createResource);
                    _context.SaveChanges();

                    newGuid = createResource.ID;
                }
                else
                {
                    //create new image 
                    tm_Resource createResource = new tm_Resource();
                    createResource.ID = Guid.NewGuid();
                    createResource.FileName = fileName;
                    createResource.FilePath = filePath;
                    createResource.MimeType = "image/jpg";
                    createResource.FlagActive = true;
                    createResource.CreateDate = DateTime.Now;
                    createResource.UpdateDate = DateTime.Now;
                    createResource.CreateBy = userID;
                    createResource.UpdateBy = userID;

                    _context.tm_Resource.Add(createResource);
                    _context.SaveChanges();

                    newGuid = createResource.ID;
                }
                return newGuid;
            }
        }

        public bool DeleteImage(Guid qcID, int? detailID, Guid resourceID, Guid userID)
        {
            bool resp = false;
            tm_Resource? resource = _context.tm_Resource
                .Where(o => o.ID == resourceID && o.FlagActive == true)
                .FirstOrDefault();
            if (resource == null) throw new Exception("ไม่พบข้อมูลรูปภาพ");
            else
            {
                tr_QC_UnitCheckList_Resource? qcResource = _context.tr_QC_UnitCheckList_Resource
                    .Where(o => o.QCUnitCheckListID == qcID
                    && o.ResourceID == resourceID && o.FlagActive == true)
                    .FirstOrDefault();
                if (qcResource == null) throw new Exception("ไม่พบข้อมูลรูปภาพที่เกี่ยวข้องกับ QC");
                else
                {
                    qcResource.FlagActive = false;
                    qcResource.UpdateDate = DateTime.Now;
                    qcResource.UpdateBy = userID;

                    _context.tr_QC_UnitCheckList_Resource.Update(qcResource);
                    _context.SaveChanges();
                    resp = true;
                }
            }
            return resp;
        }

        public bool SubmitQcCheckList(SubmitQcCheckListModel model, Guid userID, int roleID)
        {
            bool statusResp = false;

            tr_QC_UnitCheckList? checkList = _context.tr_QC_UnitCheckList
                .Where(o => o.ID == model.QcID && o.ProjectID == model.ProjectID
                && o.UnitID == model.UnitID && o.QCTypeID == model.QcTypeID
                && o.CheckListID == model.CheckListID && ( o.QCStatusID == SystemConstant.UnitQCStatus.InProgress || o.QCStatusID == SystemConstant.UnitQCStatus.IsNotReadyInspect )
                && o.FlagActive == true)
                .FirstOrDefault();

            if (checkList == null) throw new Exception("ไม่พบข้อมูลรายการตรวจสอบ QC");
            else
            {
                tr_QC_UnitCheckList_Action? action = _context.tr_QC_UnitCheckList_Action
                    .Where(o => o.QCUnitCheckListID == checkList.ID && o.ActionType == SystemConstant.ActionType.SAVE)
                    .FirstOrDefault();

                if (action == null) throw new Exception("ไม่พบข้อมูลรายการตรวจสอบ QC หรือ รายการตรวจสอบ QC ทำรายการผ่านแล้ว");
                else
                {
                    List<tr_QC_UnitCheckList_Detail> listDetail = _context.tr_QC_UnitCheckList_Detail
                        .Where(o => o.QCUnitCheckListID == checkList.ID && o.StatusID != null
                        && o.FlagActive == true).ToList();

                    if (checkList.QCStatusID == SystemConstant.UnitQCStatus.IsNotReadyInspect
                        && action.ActionType == SystemConstant.ActionType.SAVE)
                    {
                        checkList.QCStatusID = SystemConstant.UnitQCStatus.IsNotReadyInspect;
                        checkList.UpdateDate = DateTime.Now;
                        checkList.UpdateBy = userID;

                        _context.tr_QC_UnitCheckList.Update(checkList);

                        action.ActionType = SystemConstant.ActionType.SUBMIT;
                        action.RoleID = roleID;
                        action.ActionDate = DateTime.Now;
                        action.UpdateBy = userID;
                        action.UpdateDate = DateTime.Now;

                        _context.tr_QC_UnitCheckList_Action.Update(action);

                        statusResp = true;
                    }
                    else
                    {
                        if (listDetail.Any(o => o.StatusID == SystemConstant.Qc_CheckList_Status.NOTPASS))
                        {
                            checkList.QCStatusID = SystemConstant.UnitQCStatus.NotPass;
                            checkList.UpdateDate = DateTime.Now;
                            checkList.UpdateBy = userID;

                            _context.tr_QC_UnitCheckList.Update(checkList);

                            action.ActionType = SystemConstant.ActionType.SUBMIT;
                            action.RoleID = roleID;
                            action.ActionDate = DateTime.Now;
                            action.UpdateBy = userID;
                            action.UpdateDate = DateTime.Now;

                            _context.tr_QC_UnitCheckList_Action.Update(action);

                            statusResp = true;
                        }
                        else
                        {
                            checkList.QCStatusID = SystemConstant.UnitQCStatus.Pass;
                            checkList.UpdateDate = DateTime.Now;
                            checkList.UpdateBy = userID;

                            _context.tr_QC_UnitCheckList.Update(checkList);

                            action.ActionType = SystemConstant.ActionType.SUBMIT;
                            action.RoleID = roleID;
                            action.ActionDate = DateTime.Now;
                            action.UpdateBy = userID;
                            action.UpdateDate = DateTime.Now;

                            _context.tr_QC_UnitCheckList_Action.Update(action);

                            statusResp = true;
                        }
                    }
                    _context.SaveChanges();
                }
            }
            return statusResp;
        }
        public string OpenFilePDF(Guid QCCheckListID)
        {
            tr_Document? document = _context.tr_Document.Where(o => o.QCUnitCheckListID == QCCheckListID && o.FlagActive == true).FirstOrDefault();
            if (document == null) throw new Exception("ไม่พบข้อมูลเอกสาร PDF");
            else
            {
                tm_Resource? resourec = _context.tm_Resource
                    .Where(o => o.ID == document.ResourceID && o.FlagActive == true).FirstOrDefault();
                if (resourec == null) throw new Exception("แสดงข้อมูลเอกสาร PDF ผิดพลาด");
                else return resourec.FilePath;
            }
        }
    }
}

