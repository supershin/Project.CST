using System;
using Project.ConstructionTracking.Web.Models.QCModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IQcCheckListService
	{
		MasterQcCheckListDetailResp GetMasterQcCheckList(Guid projectID, int qcTypeID);

        QcCheckListResp ValidateQcCheckList(QcActionModel model);
    }
	public class QcCheckListService : IQcCheckListService
    {
		private readonly IQcCheckListRepo _qcCheckListRepo;
		public QcCheckListService(IQcCheckListRepo qcCheckListRepo)
		{
			_qcCheckListRepo = qcCheckListRepo;
		}

        public MasterQcCheckListDetailResp GetMasterQcCheckList(Guid projectID, int qcTypeID)
		{
			MasterQcCheckListDetailResp resp = _qcCheckListRepo.GetMasterQcCheckList(projectID, qcTypeID);

			return resp;
        }

		public QcCheckListResp ValidateQcCheckList(QcActionModel model)
		{
			QcCheckListResp resp = _qcCheckListRepo.ValidateQcCheckList(model);

			return resp;
        }
    }
}

