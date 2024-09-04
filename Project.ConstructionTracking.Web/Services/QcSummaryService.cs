using System;
using Project.ConstructionTracking.Web.Models.QCModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IQcSummaryService
	{
		QcSummaryResp GetQcSummary(Guid projectId, Guid userId);
	}
	public class QcSummaryService : IQcSummaryService
	{
		private readonly IQcSummaryRepo _qcSummaryRepo;
		public QcSummaryService(IQcSummaryRepo qcSummaryRepo)
		{
			_qcSummaryRepo = qcSummaryRepo;
		}

		public QcSummaryResp GetQcSummary(Guid projectId, Guid userId)
		{
			var query = _qcSummaryRepo.GetQcSummaryList(projectId, userId);

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
					};

					resp.QcSummaryLists.Add(list);
				}

            }

			return resp;
		}
	}
}

