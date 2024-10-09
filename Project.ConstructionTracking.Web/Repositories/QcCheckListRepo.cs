using System;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models.QCModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IQcCheckListRepo
	{
        MasterQcCheckListDetailResp GetMasterQcCheckList(Guid projectID, int qcTypeID);

        QcCheckListResp ValidateQcCheckList(QcActionModel model);
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

                    //IsNotReadyInspect = (bool)query.qc.IsNotReadyInspect,
                    //IsPassCondition = (bool)query.qc.IsPassCondition,
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

                        //IsNotReadyInspect = (bool)query.qc.IsNotReadyInspect,
                        //IsPassCondition = (bool)query.qc.IsPassCondition,
                        QcStatusID = query.qc.QCStatusID.GetValueOrDefault(),
                    };
                }
                else
                {
                    // get new value
                    resp = new QcCheckListResp()
                    {
                        ID = query.qc.ID,
                        ProjectID = model.ProjectID,
                        UnitID = model.UnitID,
                        QcCheckListID = model.QcCheckListID,
                        QcTypeID = model.QcTypeID,
                        Seq = 1,

                        IsNotReadyInspect = false,
                        IsPassCondition = false,
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
    }
}

