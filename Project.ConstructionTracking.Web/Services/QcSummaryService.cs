using System;
using Project.ConstructionTracking.Web.Models.QCModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IQcSummaryService
	{
		QcSummaryResp GetQcSummary(Guid projectId, Guid unitId);
	}
	public class QcSummaryService : IQcSummaryService
	{
		private readonly IQcSummaryRepo _qcSummaryRepo;
		public QcSummaryService(IQcSummaryRepo qcSummaryRepo)
		{
			_qcSummaryRepo = qcSummaryRepo;
		}

		public QcSummaryResp GetQcSummary(Guid projectId, Guid unitId)
		{
			var query = _qcSummaryRepo.GetQcSummaryList(projectId, unitId);

			QcSummaryResp resp = new QcSummaryResp()
			{
				ProjectID = query.ProjectID,
				UnitID = query.UnitID,
				ModelTypeID = query.ModelType,
				UnitCode = query.UnitCode,
				UnitStatusID = query.UnitStatus,
				UnitStatusDesc = query.UnitStatusDesc,
				FormTypeID = query.FormTypeID,
				FormTypeName = query.FormTypeName,
				QcSummaryLists = new List<QcSummaryList>()
			};

			if (query.ListQcSummary != null)
			{
                foreach (var data in query.ListQcSummary)
				{
					QcSummaryList list = new QcSummaryList()
					{
						QcCheckListID = data.QcCheckListID,
						QcTypeID = data.QcTypeID,
						QcTypeName = data.QcTypeName,
						FormQcCheckList = data.FormQcCheckList,
						FormID = data.FormID,
						QcInspections = new List<QcInspection>()
					};

					QcStatusListSummaryResp qcStatusSummary = _qcSummaryRepo.VerifyStatusQc(projectId, unitId, data.QcCheckListID);

					foreach (var qcSum in qcStatusSummary.QcStatusLists)
					{
						QcInspection inspection = new QcInspection()
						{
							QcUnitCheckListID = qcSum.QcUnitCheckListID,
							Seq = qcSum.Seq,
							QcStatusID = qcSum.QcResultStatus,
							QcStatusDesc = qcSum.QcResultStatusDesc
						};
							
						list.QcInspections.Add(inspection);
					}

                    resp.QcSummaryLists.Add(list);
				}

            }

			return resp;
		}
	}
}

