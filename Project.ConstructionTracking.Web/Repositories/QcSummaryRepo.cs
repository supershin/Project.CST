using System;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models.MProjectModel;
using Project.ConstructionTracking.Web.Models.QCModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IQcSummaryRepo
	{
		dynamic GetQcSummaryList(Guid projectID, Guid unitID);

        QcStatusListSummaryResp VerifyStatusQc(Guid projectID, Guid unitID, int checkListID);

	}

	public class QcSummaryRepo : IQcSummaryRepo
	{
		private readonly ContructionTrackingDbContext _context;
		public QcSummaryRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

        public dynamic GetQcSummaryList(Guid projectID, Guid unitID)
        {
            var query =
                (from u in _context.tm_Unit.AsNoTracking()

                 join pmf in _context.tr_ProjectModelForm.AsNoTracking()
                        .Where(x => x.FlagActive == true)
                     on u.ModelTypeID equals pmf.ModelTypeID

                 join ft in _context.tm_FormType.AsNoTracking()
                        .Where(x => x.FlagActive == true)
                     on pmf.FormTypeID equals ft.ID

                 join extUnit in _context.tm_Ext.AsNoTracking()
                     on u.UnitStatusID equals extUnit.ID

                 where u.ProjectID == projectID
                    && u.UnitID == unitID
                    && u.FlagActive == true

                 select new
                 {
                     ProjectID = u.ProjectID,
                     UnitID = u.UnitID,
                     ModelType = u.ModelTypeID,
                     UnitCode = u.UnitCode,
                     UnitStatus = u.UnitStatusID,
                     UnitStatusDesc = extUnit.Name,
                     FormTypeID = ft.ID,
                     FormTypeName = ft.Name,

                     ListQcSummary =
                         (from f in _context.tm_Form.AsNoTracking()
                                .Where(x => x.FlagActive == true)

                          join fqlJoin in _context.tr_Form_QCCheckList.AsNoTracking()
                                 .Where(x => x.FlagActive == true)
                              on f.ID equals fqlJoin.FormID into tfqlGroup

                          from fql in tfqlGroup.DefaultIfEmpty()

                          join qcl in _context.tm_QC_CheckList.AsNoTracking()
                                 .Where(x => x.FlagActive == true)
                              on fql.CheckListID equals qcl.ID

                          join extQcType in _context.tm_Ext.AsNoTracking()
                              on qcl.QCTypeID equals extQcType.ID

                          where f.FormTypeID == ft.ID

                          select new
                          {
                              QcCheckListID = qcl.ID,
                              QcTypeID = qcl.QCTypeID,
                              QcTypeName = extQcType.Name,
                              FormQcCheckList = fql.ID,
                              FormID = fql.FormID
                          }).ToList()
                 }).FirstOrDefault();

            return query;
        }

        public QcStatusListSummaryResp VerifyStatusQc(Guid projectID, Guid unitID, int checkListID)
		{
			List<tr_QC_UnitCheckList>? verifyQc = _context.tr_QC_UnitCheckList
							.Where(o => o.ProjectID == projectID
							&& o.UnitID == unitID && o.CheckListID == checkListID
							&& o.FlagActive == true).OrderBy(o => o.Seq).ToList();

			QcStatusListSummaryResp resp = new QcStatusListSummaryResp()
			{
				QcStatusLists = new List<QcStatusList>()
			};

            if (verifyQc != null)
			{

				foreach( var listData in verifyQc)
				{
					QcStatusList qcStatus = new QcStatusList();

					tr_QC_UnitCheckList_Action? qcAction = _context.tr_QC_UnitCheckList_Action
														.Where(o => o.QCUnitCheckListID == listData.ID
														).FirstOrDefault();
					if(qcAction != null)
					{
                        qcStatus.QcUnitCheckListID = (Guid)qcAction.QCUnitCheckListID;
                        qcStatus.Seq = listData.Seq.GetValueOrDefault();

                        if (qcAction.ActionType == SystemConstant.ActionType.SAVE)
						{
							// set status = wating
							qcStatus.QcResultStatus = SystemConstant.QcSummaryStatus.WORKING;
							qcStatus.QcResultStatusDesc = SystemConstant.QcSummaryStatus.Desc.WORKING;

							resp.QcStatusLists.Add(qcStatus);
						}
						else
						{
                            if (listData.QCStatusID == SystemConstant.UnitQCStatus.IsNotReadyInspect || listData.QCStatusID == SystemConstant.UnitQCStatus.NotPass)
                            {
                                //set status = suspend
                                qcStatus.QcResultStatus = SystemConstant.QcSummaryStatus.SUSPEND;
                                qcStatus.QcResultStatusDesc = SystemConstant.QcSummaryStatus.Desc.SUSPEND;

                                resp.QcStatusLists.Add(qcStatus);
                            }
                            else if (listData.QCStatusID == SystemConstant.UnitQCStatus.IsPassCondition)
                            {
                                //set status = pass
                                qcStatus.QcResultStatus = SystemConstant.QcSummaryStatus.PASSWITHCONDITION;
                                qcStatus.QcResultStatusDesc = SystemConstant.QcSummaryStatus.Desc.PASSWITHCONDITION;

                                resp.QcStatusLists.Add(qcStatus);
                            }
                            else if (listData.QCStatusID == SystemConstant.UnitQCStatus.Pass)
                            {
                                //set status = pass
                                qcStatus.QcResultStatus = SystemConstant.QcSummaryStatus.FINISH;
                                qcStatus.QcResultStatusDesc = SystemConstant.QcSummaryStatus.Desc.FINISH;

                                resp.QcStatusLists.Add(qcStatus);
                            }
                        }    
                    }
                }
            }

			return resp;
		}
    }
}

