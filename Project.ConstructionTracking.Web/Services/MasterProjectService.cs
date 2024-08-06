using System;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MProjectModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IMasterProjectService
	{
		dynamic ListMasterProject(DTParamModel param);

    }

	public class MasterProjectService : IMasterProjectService
	{
		private readonly IMasterProjectRepo _masterProjectRepo;

		public MasterProjectService(IMasterProjectRepo masterProjectRepo)
		{
			_masterProjectRepo = masterProjectRepo;
		}

		public dynamic ListMasterProject(DTParamModel param)
		{
			var list = _masterProjectRepo.GetProjectList(param);

			return list;
		}
    }
}

