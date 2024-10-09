using System;
using Microsoft.CodeAnalysis;
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

        DuplicateModelResp CreateDuplicateQcCheckList(Guid qcCheckListID, int seq);

        dynamic GetAnotherValue(Guid id, Guid projectID, Guid unitID, int qcTypeID);
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

                    IsNotReadyInspect = query.qc.IsNotReadyInspect,
                    IsPassCondition = query.qc.IsPassCondition,
                    QcStatusID = query.qc.QCStatusID.GetValueOrDefault(),
                };
            }
            else
            {
                var query = checkQcUnit.OrderByDescending(o => o.qc.Seq).FirstOrDefault();
                if (query != null)
                {
                    // get value last update
                    resp = new QcCheckListResp()
                    {
                        ID = query.qc.ID,
                        ProjectID = query.qc.ProjectID.GetValueOrDefault(),
                        UnitID = query.qc.UnitID.GetValueOrDefault(),
                        QcCheckListID = query.qc.CheckListID.GetValueOrDefault(),
                        QcTypeID = query.qc.QCTypeID.GetValueOrDefault(),
                        Seq = query.qc.Seq.GetValueOrDefault(),

                        IsNotReadyInspect = query.qc.IsNotReadyInspect,
                        IsPassCondition = query.qc.IsPassCondition,
                        QcStatusID = query.qc.QCStatusID.GetValueOrDefault(),
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

                        IsNotReadyInspect = null,
                        IsPassCondition = null,
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

                if(QcCheckList.IsNotReadyInspect == false && QcCheckList.QcStatusID == SystemConstant.QcStatus.PASS)
                {
                    throw new Exception("รายการตรวจQC นี้ผ่านแล้ว");
                }

                //verify submit Previous Qc CheckList
                tr_QC_UnitCheckList_Action? qcAction = _context.tr_QC_UnitCheckList_Action
                    .Where(o => o.QCUnitCheckListID == QcCheckList.ID && o.ActionType == SystemConstant.ActionType.SUBMIT)
                    .FirstOrDefault();

                //throw error if data not submit 
                if (qcAction == null)
                {
                    throw new Exception("กรุณา Submit รายการตรวจQC ก่อนหน้า");
                }
                //get draft Previous Qc CheckList
                else
                {
                    if(transQc.IsNotReadyInspect == true || transQc.IsNotReadyInspect == false)
                    {
                        resp = new VerifyQcCheckList()
                        {
                            QcCheckList = QcCheckList
                        };
                    }

                    //else if(transQc.IsPassCondition == true || (transQc.IsPassCondition == null && transQc.IsNotReadyInspect == null))
                    //{
                    //    if (transQc.QCStatusID == SystemConstant.QcStatus.FAILED)
                    //    {
                    //        // find detail checklist qc 
                    //        List<tr_QC_UnitCheckList_Detail> checkListDetail = _context.tr_QC_UnitCheckList_Detail
                    //            .Where(o => o.QCUnitCheckListID == transQc.ID &&
                    //            o.CheckListID == transQc.CheckListID).ToList();

                    //        if (checkListDetail.Count > 0 && checkListDetail != null)
                    //        {
                    //            foreach (var detail in checkListDetail)
                    //            {
                    //                resp = new VerifyQcCheckList()
                    //                {
                    //                    QcCheckListDetail = new QcCheckListDetail()
                    //                    {
                    //                        QcCheckListDetailID = detail.ID,
                    //                        StatusID = (int)detail.StatusID,
                    //                        Images = new List<ImageQcCheckListDetail>()
                    //                    }
                    //                };

                    //                // find image qc checklist detail 
                    //                List<tr_QC_UnitCheckList_Resource> qcResource = _context.tr_QC_UnitCheckList_Resource
                    //                    .Where(o => o.QCUnitCheckListDetailID == detail.ID && o.FlagActive == true)
                    //                    .ToList();

                    //                if (qcResource.Count > 0 && qcResource != null)
                    //                {
                    //                    foreach (var image in qcResource)
                    //                    {
                    //                        ImageQcCheckListDetail img = new ImageQcCheckListDetail();
                    //                        img.ResourceID = (Guid)image.ResourceID;
                    //                        img.FilePath = img.FilePath;

                    //                        resp.QcCheckListDetail.Images.Add(img);
                    //                    }
                    //                }
                                        
                    //            }
                    //        }
                    //    }
                    //}
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
            if (model.ID == Guid.Empty)
            {
                resp.QcCheckList = new QcCheckListResp()
                {
                    ProjectID = model.ProjectID,
                    UnitID = model.UnitID,
                    QcCheckListID = model.QcCheckListID,
                    QcTypeID = model.QcTypeID,
                    Seq = model.Seq
                };
            }
            // get QC CheckList and Detail if New Create or View Seq Only
            else
            {
                var query = (from qcl in _context.tr_QC_UnitCheckList
                             join qcla in _context.tr_QC_UnitCheckList_Action on qcl.ID equals qcla.QCUnitCheckListID
                             join r in _context.tm_Resource on qcl.QCSignResourceID equals r.ID into rGroup
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
                                 IsNotReadyInspect = qcl.IsNotReadyInspect,
                                 IsPassCondition = qcl.IsPassCondition,
                                 QcStatusID = qcl.QCStatusID,
                                 QcActionType = qcla.ActionType,
                                 UserQcResourceUrl = r.FilePath,
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
                        IsNotReadyInspect = query.IsNotReadyInspect,
                        IsPassCondition = query.IsPassCondition,
                        QcStatusID = query.QcStatusID,
                        UserQcResourceUrl = query.UserQcResourceUrl,
                        QcActionType = query.QcActionType
                    };

                    resp.QcCheckListDetail = new List<QcCheckListDetail>();

                    foreach(var detail in query.CheckListDetails)
                    {
                        QcCheckListDetail qcDetail = new QcCheckListDetail()
                        {
                            QcCheckListDetailID = (int)detail.CheckListDetailID,
                            StatusID = (int)detail.DetailStatusID,
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

        public DuplicateModelResp CreateDuplicateQcCheckList(Guid qcCheckListID, int seq)
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
                    IsNotReadyInspect = qcChecklist.IsNotReadyInspect,
                    IsPassCondition = qcChecklist.IsPassCondition,
                    QCStatusID = qcChecklist.QCStatusID,
                    QCSignID = qcChecklist.QCSignID,
                    QCSignResourceID = qcChecklist.QCSignResourceID,
                    FlagActive = qcChecklist.FlagActive,
                    CreateDate = qcChecklist.CreateDate,
                    CreateBy = qcChecklist.CreateBy,
                    UpdateDate = qcChecklist.UpdateDate,
                    UpdateBy = qcChecklist.UpdateBy
                };

                _context.tr_QC_UnitCheckList.Add(createNew);
                _context.SaveChanges();

                // create draft new qc checklist action
                tr_QC_UnitCheckList_Action createNewAction = new tr_QC_UnitCheckList_Action()
                {
                    QCUnitCheckListID = qcCheckListAction.QCUnitCheckListID,
                    RoleID = qcCheckListAction.RoleID,
                    ActionType = qcCheckListAction.ActionType,
                    StatusID = qcCheckListAction.StatusID,
                    Remark = qcCheckListAction.Remark,
                    ActionDate = qcCheckListAction.ActionDate,
                    UpdateDate = qcCheckListAction.UpdateDate,
                    UpdateBy = qcCheckListAction.UpdateBy,
                    CraeteDate = qcCheckListAction.CraeteDate,
                    CreateBy = qcCheckListAction.CreateBy
                };

                _context.tr_QC_UnitCheckList_Action.Add(createNewAction);
                _context.SaveChanges();

                // create draft new qc checklist list detail
                List<tr_QC_UnitCheckList_Detail> createNewListDetail = new List<tr_QC_UnitCheckList_Detail>(qcCheckListDetail);

                foreach(var list in createNewListDetail)
                {
                    list.ID = 0;
                    list.QCUnitCheckListID = createNew.ID;
                    list.UpdateDate = DateTime.Now;
                    list.UpdateBy = 1;
                    list.CreateDate = DateTime.Now;
                    list.CreateBy = 1;

                    _context.tr_QC_UnitCheckList_Detail.Add(list);
                    _context.SaveChanges();

                    List<tr_QC_UnitCheckList_Resource> selectDetailResource = qcCheckListResource
                        .Where(o => o.QCUnitCheckListDetailID == list.ID && o.FlagActive == true)
                        .ToList();

                    List<tr_QC_UnitCheckList_Resource> createNewResource = new List<tr_QC_UnitCheckList_Resource>(selectDetailResource);
                    foreach(var resource in createNewResource)
                    {
                        resource.ID = 0;
                        resource.QCUnitCheckListID = createNew.ID;
                        resource.QCUnitCheckListDetailID = list.ID;
                        resource.UpdateDate = DateTime.Now;
                        //resource.UpdateBy = ;
                        resource.CreateDate = DateTime.Now;
                        //resource.CreateBy = 1;
                    }

                    _context.tr_QC_UnitCheckList_Resource.AddRange(createNewResource);
                    _context.SaveChanges();
                }

                //foreach( var detail in qcCheckListDetail)
                //{
                //    tr_QC_UnitCheckList_Detail createNewDetail = new tr_QC_UnitCheckList_Detail()
                //    {

                //    };

                //    _context.tr_QC_UnitCheckList_Detail.Add(createNewDetail);
                //    _context.SaveChanges();

                //    int id = createNewDetail.ID;
                //}

                resp = new DuplicateModelResp()
                {
                    QCCheckListID = createNew.ID,
                    Seq = createNew.Seq
                    
                };
            }
            else
            {
                throw new Exception("ไม่พบข้อมูลรายการตรวจสอบ QC");
            }

            return resp;
        }

        public dynamic GetAnotherValue(Guid id, Guid projectID, Guid unitID, int qcTypeID)
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
    }
}

