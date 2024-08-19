using System;
using System.Transactions;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MFormModel;
using Project.ConstructionTracking.Web.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using PackageModel = Project.ConstructionTracking.Web.Models.MFormModel.PackageModel;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IMasterFormService
	{
		dynamic GetFormTypeList(DTParamModel param, MasterFormModel criteria);

		List<ProjectType> GetProjectTypeList();

		DetailFormType GetDetailFormType(int FormTypeId);

		FormTypeResp ActionFormType(FormTypeModel model);

		dynamic GetFormList(DTParamModel param, int formTypeID);
		dynamic GetFormGroupList(DTParamModel param, int formID);
		dynamic GetFormPackageList(DTParamModel param, int groupID);
		dynamic GetFormCheckList(DTParamModel param, int packageID);

		List<QcList> GetQcList(int formTypeID);

		FormDetail GetFormDetail(int formID);
		GroupDetail GetGroupDetail(int groupID);
		PackageDetail GetPackageDetail(int packageID);
		CheckDetail GetCheckListDetail(int checkID);

		FormResp ActionForm(FormModel model);
		GroupResp ActionFormGroup(GroupModel model);
		PackageResp ActionFormPackage(PackageModel model);
		CheckListResp ActionFormCheckList(CheckListModel model);

    }

	public class MasterFormService : IMasterFormService
	{
        private readonly IMasterFormRepo _masterForm;

		public MasterFormService(IMasterFormRepo masterForm)
		{
            _masterForm = masterForm;
		}

		public dynamic GetFormTypeList(DTParamModel param, MasterFormModel criteria)
        {
            var query = _masterForm.GetFormTypeList(param, criteria);

			return query;   
		}

		public List<ProjectType> GetProjectTypeList()
		{
			var query = _masterForm.GetProjectTypeList();

			List<ProjectType> projectTypeList = new List<ProjectType>();

			foreach(var i in query)
			{
				ProjectType projectType = new ProjectType();
				projectType.ID = i.ID;
				projectType.Name = i.Name;

				projectTypeList.Add(projectType);
			}

			return projectTypeList;
		}

        public DetailFormType GetDetailFormType(int FormTypeId)
		{
			var query = _masterForm.GetFormTypeDetial(FormTypeId);

			DetailFormType detail = new DetailFormType()
			{
				FormTypeId = query.FormTypeId,
				FormTypeName = query.FormTypeName,
				Description = query.Description,
				ProjectTypeId = query.ID,
				ProjectTypeName = query.Name
			};

			return detail;
		}

		public FormTypeResp ActionFormType(FormTypeModel model)
		{
			TransactionOptions option = new TransactionOptions();
			option.Timeout = new TimeSpan(1, 0, 0);
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
			{
				try
				{
					var data = _masterForm.ActionFormType(model);

					FormTypeResp resp = new FormTypeResp()
					{
						FormTypeID = data.FormTypeID,
						ProjectTypeID = data.ProjectTypeID,
						FormTypeName = data.FormTypeName,
						FormTypeDesc = data.FormTypeDesc
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

		public dynamic GetFormList(DTParamModel param, int formTypeID)
		{
            var query = _masterForm.GetFormList(param, formTypeID);

            return query;
        }

		public dynamic GetFormGroupList(DTParamModel param, int formID)
		{
            var query = _masterForm.GetFormGroupList(param, formID);

            return query;
        }

        public dynamic GetFormPackageList(DTParamModel param, int groupID)
        {
            var query = _masterForm.GetFormPackageList(param, groupID);

            return query;
        }

        public dynamic GetFormCheckList(DTParamModel param, int packageID)
        {
            var query = _masterForm.GetFormCheckList(param, packageID);

            return query;
        }

		public List<QcList> GetQcList(int formTypeID)
		{
			var query = _masterForm.GetQcList(formTypeID);

			List<QcList> qcLists = new List<QcList>();

			foreach( var i in query)
			{
				QcList qcList = new QcList();
				qcList.ID = i.ID;
				qcList.Name = i.Name;

				qcLists.Add(qcList);
			}

			return qcLists;
		}

		public FormDetail GetFormDetail(int formID)
		{
			var query = _masterForm.GetFormDetail(formID);

            List<QcList> list = new List<QcList>();
            if (query.QcLists.Count > 0)
			{
                foreach (var data in query.QcLists)
                {
                    QcList qc = new QcList();
                    qc.ID = data.ID;
                    qc.Name = data.Name;

                    list.Add(qc);
                }
            }
			
			FormDetail detail = new FormDetail()
			{
				ID = query.ID,
				Name = query.Name,
				Description = query.Description,
				Progress = query.Progress,
				DurationDay = query.DurationDay,
				QcLists = list
			};

			return detail;
		}

		public GroupDetail GetGroupDetail(int groupID)
		{
			var query = _masterForm.GetGroupDetail(groupID);

			GroupDetail detail = new GroupDetail()
			{
				ID = query.ID,
				Name = query.Name
			};

			return detail;
        }

		public PackageDetail GetPackageDetail(int packageID)
		{
			var query = _masterForm.GetPackageDetail(packageID);

			PackageDetail detail = new PackageDetail()
			{
				ID = query.ID,
				Name = query.Name
			};

			return detail;
		}

        public CheckDetail GetCheckListDetail(int checkID)
        {
            var query = _masterForm.GetCheckListDetail(checkID);

            CheckDetail detail = new CheckDetail()
            {
                ID = query.ID,
                Name = query.Name
            };

            return detail;
        }

		public FormResp ActionForm(FormModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var query = _masterForm.ActionForm(model);

                    FormResp resp = new FormResp()
                    {
                        FormID = query.FormID,
                        FormTypeID = query.FormTypeID,
                        FormName = query.FormName,
                        FormDesc = query.FormDesc,
                        Progress = query.Progress,
                        Duration = query.Duration,
                        QcList = query.QcList
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

        public GroupResp ActionFormGroup(GroupModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var query = _masterForm.ActionGroup(model);

                    GroupResp resp = new GroupResp()
                    {
                        GroupID = query.GroupID,
                        FormID = query.FormID,
                        GroupName = query.GroupName
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

        public PackageResp ActionFormPackage(PackageModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var query = _masterForm.ActionPacakge(model);

                    PackageResp resp = new PackageResp()
                    {
                        PackageID = query.PackageID,
                        GroupID = query.GroupID,
                        PackageName = query.PackageName
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

        public CheckListResp ActionFormCheckList(CheckListModel model)
		{
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
			{
				try
				{
                    var query = _masterForm.ActionCheckList(model);

                    CheckListResp resp = new CheckListResp()
                    {
                        CheckListID = query.CheckListID,
                        PackageID = query.PackageID,
                        CheckListName = query.CheckListName
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
    }
}

