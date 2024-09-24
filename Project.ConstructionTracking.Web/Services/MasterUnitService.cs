using System;
using System.Transactions;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MUnitModel;
using Project.ConstructionTracking.Web.Repositories;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IMasterUnitService
	{
		MasterUnitResp GetModelUnitResp(Guid userId, int userRole);

        dynamic ListMasterUnit(DTParamModel param, MasterUnitModel criteria);
        DetailUnitResp GetProjectID(Guid unitID);

        CreateUnitResp CreateUnit(CreateUnitModel model);
		EditUnitResp EditUnit(EditUnitModel model);
        bool DeleteUnit(Guid unitID, Guid requestUserID);

        GetPeFromProjectResp GetPEFromProject(Guid projectID);
        bool ActionMappingPE(ActionMappingPeModel model, Guid RequestUserID);
    }

	public class MasterUnitService : IMasterUnitService
	{
		private readonly IMasterUnitRepo _masterUnitRepo;
        private readonly IMasterCommonRepo _masterCommonRepo;

        public MasterUnitService(IMasterUnitRepo masterUnitRepo,
            IGetDDLService getDDLService,
            IMasterCommonRepo masterCommonRepo)
		{
			_masterUnitRepo = masterUnitRepo;
            _masterCommonRepo = masterCommonRepo;
		}

        public MasterUnitResp GetModelUnitResp(Guid userId, int userRole)
		{
			MasterUnitResp resp = new MasterUnitResp()
			{
				ProjectList = new List<Projects>(),
				UnitTypeList = new List<UnitTypes>()
			};

			var listProject = _masterCommonRepo.GetProjectList(userId , userRole);

            foreach (var project in listProject)
			{
				Projects data = new Projects()
				{
					ProjectID = project.ProjectID,
					ProjectName = project.ProjectName,
                    ModelList = new List<ModelTypes>(),
                };

				var listModel = _masterUnitRepo.GetModelTypeInProjectList(project.ProjectID);

				foreach( var model in listModel)
				{
					ModelTypes modelTypes = new ModelTypes()
					{
						ModelTypeID = model.ID,
						ModelTypeName = model.ModelName
                    };

					data.ModelList.Add(modelTypes);
				}

				resp.ProjectList.Add(data);

			}

			var listUnitType = _masterUnitRepo.GetUnitTypeList();

			foreach (var unitType in listUnitType)
			{
				UnitTypes unit = new UnitTypes()
				{
					UnitTypeID = unitType.ID,
					UnitTypeName = unitType.Name
				};

				resp.UnitTypeList.Add(unit);
            }

			return resp;
		}

        public dynamic ListMasterUnit(DTParamModel param, MasterUnitModel criteria)
        {
            var list = _masterUnitRepo.GetUnitList(param, criteria);

            return list;
        }

        public CreateUnitResp CreateUnit(CreateUnitModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var create = _masterUnitRepo.CreateUnit(model);

                    CreateUnitResp resp = new CreateUnitResp()
                    {
                        UnitID = create.UnitID,
                        ProjectID = create.ProjectID,
                        UnitTypeID = create.UnitTypeID,
                        ModelTypeID = create.ModelTypeID,
                        UnitCode = create.UnitCode,
                        UnitAddress = create.AddreessNo,
                        UnitArea = create.Area,
                        FlagActive = create.FlagActive,
                        CreateDate = create.CreateDate
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

        public DetailUnitResp GetProjectID(Guid unitID)
		{
            var data = _masterUnitRepo.GetProjectId(unitID);

			DetailUnitResp resp = new DetailUnitResp()
			{
				UnitID = data.UnitID,
				ProjectID = data.ProjectID,
				CompanyVendorID = data.CompanyVendorID,
				PONo = data.PONo,
				StartDate = data.StartDate,
				CompanyVendorList = new List<Companys>()
			};

            var listCompanyVendor = _masterUnitRepo.GetProjectCompanyVendor(data.ProjectID);

            foreach (var cv in listCompanyVendor)
            {
                Companys companyVendor = new Companys()
                {
                    VendorID = cv.ID,
                    VendorName = cv.Name
                };

                resp.CompanyVendorList.Add(companyVendor);
            }

            return resp;
		}

        public EditUnitResp EditUnit(EditUnitModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var data = _masterUnitRepo.EditUnit(model);

                    EditUnitResp resp = new EditUnitResp()
                    {
                        UnitID = data.UnitID,
                        ProjectID = data.ProjectID,
                        CompanyVendorID = data.CompanyVendorID,
                        PONo = data.PONo,
                        StartDate = data.StartDate,
                        EndDate = data.EndDate,
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

        public bool DeleteUnit(Guid unitID, Guid requestUserID)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    bool resp = false;
                    var data = _masterUnitRepo.DeleteUnit(unitID, requestUserID);

                    if (data != null) resp = true;

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

        public GetPeFromProjectResp GetPEFromProject(Guid projectID)
        {
            GetPeFromProjectResp resp = new GetPeFromProjectResp()
            {
                UserModelList = new List<UserModel>()
            };

            var queryList = _masterUnitRepo.GetPEFromProject(projectID);

            foreach (var data in queryList)
            {
                UserModel user = new UserModel()
                {
                    UserID = data.UserID,
                    FullName = data.FirstName + " " + data.LastName
                };

                resp.UserModelList.Add(user);
            }

            return resp;
        }

        public bool ActionMappingPE(ActionMappingPeModel model, Guid requestUserID)
        {
            bool query = _masterUnitRepo.ActionMappingPE(model, requestUserID);

            return query;
        }
    }
}

