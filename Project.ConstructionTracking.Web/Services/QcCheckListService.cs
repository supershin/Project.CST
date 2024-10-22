using System;
using Project.ConstructionTracking.Web.Models.MUserModel;
using System.Transactions;
using Project.ConstructionTracking.Web.Models.QCModel;
using Project.ConstructionTracking.Web.Repositories;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Microsoft.CodeAnalysis;
using System.Data;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IQcCheckListService
	{
		MasterQcCheckListDetailResp GetMasterQcCheckList(Guid projectID, int qcTypeID);

        QcCheckListResp ValidateQcCheckList(QcActionModel model);
		VerifyQcCheckList VerifyQcCheckList(QcActionModel model);

		QcCheckListDetailResp GetQcCheckListDetail(GetDetailModel model);

        DuplicateModelResp CreateDuplicateQcCheckList(Guid qcCheckListID, int seq, Guid userID);

		dynamic SaveQcCheckList(SaveTransQCCheckListModel model, Guid userId, int roleID);
		bool DeleteImage(Guid qcID, int? detailID, Guid resourceID, Guid userID);
        //string SubmitQcCheckList(SubmitQcCheckListModel model, Guid userID, int roleID);
        SubmitQcCheckListModel SubmitQcCheckList(SaveTransQCCheckListModel model, Guid userID, int roleID);
    }
	public class QcCheckListService : IQcCheckListService
    {
		private readonly IQcCheckListRepo _qcCheckListRepo;
		private readonly IMasterCommonRepo _masterCommonRepo;
		private readonly IGeneratePDFRepo _generatePDFRepo;
        public QcCheckListService(IQcCheckListRepo qcCheckListRepo,
            IMasterCommonRepo masterCommonRepo
			, IGeneratePDFRepo generatePDFRepo)
		{
			_qcCheckListRepo = qcCheckListRepo;
			_masterCommonRepo = masterCommonRepo;
			_generatePDFRepo = generatePDFRepo;
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

			if(model.ID == null)
			{
				model.ID = Guid.Empty;
			}
			var anotherValue = _qcCheckListRepo.GetAnotherValue(model.ID, model.ProjectID, model.UnitID, model.QcTypeID);
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

		public DuplicateModelResp CreateDuplicateQcCheckList(Guid qcCheckListID, int seq, Guid userID)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    DuplicateModelResp resp = _qcCheckListRepo.CreateDuplicateQcCheckList(qcCheckListID, seq, userID);
					
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

		public dynamic SaveQcCheckList(SaveTransQCCheckListModel model, Guid userId, int roleID)
		{
			TransactionOptions option = new TransactionOptions();
			option.Timeout = new TimeSpan(1, 0, 0);
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
			{
				try
				{
					Guid imageID = Guid.Empty;
                    //user sign resource
                    if (!string.IsNullOrEmpty(model.PeSignResource))
                    {
                        imageID = _qcCheckListRepo.UploadSignResource(model.QcID ,model.PeSignResource, model.ApplicationPath, userId);
                    }
					if (imageID != Guid.Empty)
					{
						model.PeSignResourceID = imageID;
					}

					var resp = _qcCheckListRepo.SaveQcCheckList(model, userId, roleID);

					scope.Complete();

					return resp;
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

		public bool DeleteImage(Guid qcID, int? detailID, Guid resourceID, Guid userID)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
			{
				try
				{
					var resp = _qcCheckListRepo.DeleteImage(qcID, detailID, resourceID, userID);

                    scope.Complete();

					return resp;
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

        public SubmitQcCheckListModel SubmitQcCheckList(SaveTransQCCheckListModel model, Guid userID, int roleID)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var saveData = SaveQcCheckList(model, userID, roleID);

                    SubmitQcCheckListModel submitModel = new SubmitQcCheckListModel()
                    {
                        QcID = saveData.QcID,
                        ProjectID = saveData.ProjectID,
                        UnitID = saveData.UnitID,
                        CheckListID = saveData.CheckListID,
                        QcTypeID = saveData.QcTypeID,
                        Seq = saveData.Seq
                    };

                    var boolResp = _qcCheckListRepo.SubmitQcCheckList(submitModel, userID, roleID);

					DataToGenerateModel data = new DataToGenerateModel()
					{
						ProjectID = submitModel.ProjectID,
						UnitID = submitModel.UnitID,
						QCUnitCheckListID = submitModel.QcID,
						QCTypeID = submitModel.QcTypeID
					};

                    DataGenerateQCPDFResp getData = _generatePDFRepo.GetDataQC1To4ForGeneratePDF(data);

					DataDocumentModel genDocumentNo = _generatePDFRepo.GenerateDocumentNO(model.ProjectID);

					Guid guid = Guid.NewGuid();
                    string pathUrl =  _generatePDFRepo.GenerateQCPDF2(guid, getData, genDocumentNo);

					submitModel.DocumentUrl = pathUrl;

                    scope.Complete();

					return submitModel;
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

