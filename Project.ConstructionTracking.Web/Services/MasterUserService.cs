using System;
using System.Transactions;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MUnitModel;
using Project.ConstructionTracking.Web.Models.MUserModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IMasterUserService
	{
		dynamic UserList(DTParamModel param, MasterUserModel criteria);

		UnitRespModel GetUnitResp();

		CreateUserResp CreateUser(CreateUserModel model);

		DetailUserResp DetailUser(Guid userID);

        EditUserResp EditUser(EditUserModel model);

		bool DeleteUser(Guid userId, Guid requestUserID);
    }

	public class MasterUserService : IMasterUserService
	{
		private readonly IMasterUserRepo _masterUserRepo;
		public MasterUserService(IMasterUserRepo masterUserRepo)
		{
			_masterUserRepo = masterUserRepo;
		}

		public dynamic UserList(DTParamModel param, MasterUserModel criteria)
		{
			var query = _masterUserRepo.GetUserList(param, criteria);

			return query;
		}

		public UnitRespModel GetUnitResp()
		{
			UnitRespModel resp = new UnitRespModel()
			{
				BUList = new List<BUModel>(),
				RoleList = new List<RoleModel>(),
				PositionList = new List<PositionModel>()
			};

			var bu = _masterUserRepo.GetBU();
			foreach ( var b in bu)
			{
                BUModel model = new BUModel()
				{
					ID = b.ID,
					Name = b.Name
				};

				resp.BUList.Add(model);
			}

			var role = _masterUserRepo.GetRole();
            foreach (var r in role)
            {
                RoleModel roles = new RoleModel()
                {
                    ID = r.ID,
                    Name = r.Name
                };

                resp.RoleList.Add(roles);
            }

            var position = _masterUserRepo.GetPosition();
			foreach (var p in position)
			{
				PositionModel positions = new PositionModel()
				{
					Name = p.Name
				};

                resp.PositionList.Add(positions);
			}

			return resp;
		}

		public CreateUserResp CreateUser(CreateUserModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var create = _masterUserRepo.CreateUser(model);

					CreateUserResp resp = new CreateUserResp()
					{
						UserID = create.ID,
						UserName = create.Username
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

		public DetailUserResp DetailUser(Guid userID)
		{
			var query = _masterUserRepo.DetailUser(userID);

			DetailUserResp resp = new DetailUserResp()
			{
				UserID = query.ID,
				FirstName = query.FirstName,
				LastName = query.LastName,
				Email = query.Email,
				MobileNo = query.Mobile,
				BUID = query.BUID,
				RoleID = query.RoleID,
				JobPosition = query.Name,
				ImageSign = query.FilePath
			};

			return resp;
        }

        public EditUserResp EditUser(EditUserModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var edit = _masterUserRepo.EditUser(model);

                    //user sign resource
                    if (!string.IsNullOrEmpty(model.SignUser))
                    {
                        _masterUserRepo.UploadSignResource(model.SignUser, model.ApplicationPath, model.UserID , model.RequestUserID);
                    }

                    EditUserResp resp = new EditUserResp()
                    {
                        UserID = edit.ID,
                        Username = edit.Username
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

		public bool DeleteUser(Guid userId, Guid requestUserID)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    bool resp = _masterUserRepo.DeleteUser(userId, requestUserID);

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

