using System;
using System.Transactions;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MCompanyModel;
using Project.ConstructionTracking.Web.Models.MProjectModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IMasterProjectService
	{
		dynamic ListMasterProject(DTParamModel param, MasterProjectModel criteria);

		InformationProjectResp InfomationForProejct();

		CreateProjectResp CreateProject(CreateProjectModel model);

		DetailProject DetailProjectInformation(Guid guid);

		EditProjectResp EditProject(EditProjectModel model);

		bool DeleteProject(Guid guid);
    }

	public class MasterProjectService : IMasterProjectService
	{
		private readonly IMasterProjectRepo _masterProjectRepo;

		public MasterProjectService(IMasterProjectRepo masterProjectRepo)
		{
			_masterProjectRepo = masterProjectRepo;
		}

		public dynamic ListMasterProject(DTParamModel param, MasterProjectModel criteria)
		{
			var list = _masterProjectRepo.GetProjectList(param, criteria);

			return list;
		}

		public InformationProjectResp InfomationForProejct()
		{
			var info = _masterProjectRepo.InfomationForProejct();

			InformationProjectResp resp = new InformationProjectResp()
			{
				BUList = new List<BUModel>(),
				ProjectTypeList = new List<ProjectType>()
			};

			foreach(var bu in info.bu)
			{
				BUModel model = new BUModel();
				model.BUID = bu.ID;
				model.BUName = bu.Name;

				resp.BUList.Add(model);
            }

			foreach(var pt in info.ext)
			{
				ProjectType project = new ProjectType();
				project.ProjectTypeID = pt.ID;
				project.ProjectTypeName = pt.Name;

				resp.ProjectTypeList.Add(project);
			}

			return resp;
        }

        public CreateProjectResp CreateProject(CreateProjectModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var query = _masterProjectRepo.CreateProject(model);

                    CreateProjectResp resp = new CreateProjectResp()
                    {
                        ProjectID = query.ProjectID,
						ProjectCode = query.ProjectCode,
						ProjectName = query.ProjectName
                    };

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

        public DetailProject DetailProjectInformation(Guid guid)
		{
			var query = _masterProjectRepo.DetailProjectInformation(guid);

			DetailProject resp = new DetailProject()
			{
				ProjectID = query.ProjectID,
				BUID = query.BUID,
				ProjectTypeID = query.ProjectTypeID,
				ProjectCode = query.ProjectCode,
				ProjectName = query.ProjectName,
				ModelTypeList = new List<ModelType>(),
                FormTypeList = new List<FormType>()
			};

			foreach ( var data in query.ListModelType)
			{
				ModelType model = new ModelType()
				{
					ModelID = data.ModelID,
					ModelCode = data.ModelCode,
					ModelName = data.ModelName,
					ModelTypeCode = data.ModelTypeCode,
					ModelTypeName = data.ModelTypeName,
					FormTypeID = data.MappingForm != null ? data.MappingForm.FormTypeID : null
                };

				resp.ModelTypeList.Add(model);
			}

			var queryFormTypeList = _masterProjectRepo.GetFormTypeList(query.ProjectTypeID);

			foreach( var value in queryFormTypeList)
			{
				FormType formType = new FormType()
				{
					FormTypeID = value.ID,
					FormTypeName = value.Name
				};

				resp.FormTypeList.Add(formType);
			}

			return resp;
		}

        public EditProjectResp EditProject(EditProjectModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    EditProjectResp resp = _masterProjectRepo.EditProject(model);

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

        public bool DeleteProject(Guid guid)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
					bool resp = false;
                    var query = _masterProjectRepo.DeleteProject(guid);

					if (query != null) resp = true;

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
    }
}

