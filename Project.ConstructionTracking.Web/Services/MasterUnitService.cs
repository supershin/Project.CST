using System;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MUnitModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IMasterUnitService
	{
		MasterUnitResp GetModelUnitResp();

        dynamic ListMasterUnit(DTParamModel param, MasterUnitModel criteria);
	}

	public class MasterUnitService : IMasterUnitService
	{
		private readonly IMasterUnitRepo _masterUnitRepo;
		public MasterUnitService(IMasterUnitRepo masterUnitRepo)
		{
			_masterUnitRepo = masterUnitRepo;
		}

        public MasterUnitResp GetModelUnitResp()
		{
			MasterUnitResp resp = new MasterUnitResp()
			{
				ProjectList = new List<Projects>()
			};
			var listProject = _masterUnitRepo.GetProjectList();

			foreach (var project in listProject)
			{
				Projects data = new Projects()
				{
					ProjectID = project.ProjectID,
					ProjectName = project.ProjectName
				};

				resp.ProjectList.Add(data);
			}


			return resp;
		}

        public dynamic ListMasterUnit(DTParamModel param, MasterUnitModel criteria)
        {
            var list = _masterUnitRepo.GetUnitList(param, criteria);

            return list;
        }
    }
}

