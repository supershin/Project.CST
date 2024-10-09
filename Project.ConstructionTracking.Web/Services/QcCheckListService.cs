using System;
using Project.ConstructionTracking.Web.Models.MUserModel;
using System.Transactions;
using Project.ConstructionTracking.Web.Models.QCModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IQcCheckListService
	{
		MasterQcCheckListDetailResp GetMasterQcCheckList(Guid projectID, int qcTypeID);

        QcCheckListResp ValidateQcCheckList(QcActionModel model);
		VerifyQcCheckList VerifyQcCheckList(QcActionModel model);

		QcCheckListDetailResp GetQcCheckListDetail(GetDetailModel model);

        DuplicateModelResp CreateDuplicateQcCheckList(Guid qcCheckListID, int seq);
    }
	public class QcCheckListService : IQcCheckListService
    {
		private readonly IQcCheckListRepo _qcCheckListRepo;
		private readonly IMasterCommonRepo _masterCommonRepo;
        public QcCheckListService(IQcCheckListRepo qcCheckListRepo,
            IMasterCommonRepo masterCommonRepo)
		{
			_qcCheckListRepo = qcCheckListRepo;
			_masterCommonRepo = masterCommonRepo;
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

		public VerifyQcCheckList VerifyQcCheckList(QcActionModel model)
		{
			VerifyQcCheckList resp = _qcCheckListRepo.VerifyQcCheckList(model);

			return resp;
        }

		private dynamic GetProjectAndUnit(Guid projectId, Guid unitId)
		{
			var resp = _masterCommonRepo.GetProjectAndUnit(projectId, unitId);
			return resp;
		}

		public QcCheckListDetailResp GetQcCheckListDetail(GetDetailModel model)
		{
			QcCheckListDetailResp resp = new QcCheckListDetailResp();
			resp = _qcCheckListRepo.GetQcCheckListDetail(model);

			var anotherValue = _qcCheckListRepo.GetAnotherValue((Guid)model.ID, model.ProjectID, model.UnitID, model.QcTypeID);
			var projectUnit = GetProjectAndUnit(model.ProjectID, model.UnitID);
			resp.ProjectUnit = new ProjectUnitModel()
			{
				ProjectName = projectUnit.ProjectName,
				UnitCode = projectUnit.UnitCode,
				UnitStatus = projectUnit.Name
			};

			resp.AnotherValue = new GetValueSetModel()
			{
				QCName = anotherValue.QCName,
				QCNumber = anotherValue.QCNumber,
				PEID = anotherValue.PEID,
				PEName = anotherValue.PEName
			};

			return resp;
		}

		public DuplicateModelResp CreateDuplicateQcCheckList(Guid qcCheckListID, int seq)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    DuplicateModelResp resp = _qcCheckListRepo.CreateDuplicateQcCheckList(qcCheckListID, seq);
					
                    scope.Complete();

					if (resp != null)
						return resp;
					else
						throw new Exception("ไม่พบข้อมูลรายการตรวจสอบ QC");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }
    }
}

