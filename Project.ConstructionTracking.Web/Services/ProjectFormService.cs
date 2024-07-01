using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class ProjectFormService : IProjectFormService
    {
        private readonly IProjectFormRepo _projectFormRepo;

        public ProjectFormService(IProjectFormRepo projectFormRepo)
        {
            _projectFormRepo = projectFormRepo;
        }

        public FormCheckListUnitView GetFormCheckListUnit(int formID, int unitID)
        {
            var formCheckListUnitView = new FormCheckListUnitView
            {
                Detail = _projectFormRepo.GetDetail(unitID),
                FormCheckListUnitList = _projectFormRepo.GetFormCheckUnitList(formID)
            };
            return formCheckListUnitView;
        }

        public void InsertFormCheckListUnit(UnitForm model)
        {
            _projectFormRepo.InsertFormCheckListUnit(model);
        }


    }
}
