using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class ProjectFormService : IProjectFormService
    {
        private readonly IProjectFormRepo _ProjectFormRepo;

        public ProjectFormService(IProjectFormRepo ProjectFormRepo)
        {
            _ProjectFormRepo = ProjectFormRepo;
        }
        public FormCheckListUnitView GetFormCheckListUnit(int formID)
        {
            var formCheckListUnitView = new FormCheckListUnitView();
            formCheckListUnitView.FormCheckListUnitList = _ProjectFormRepo.GetFormCheckUnitList(formID);
            return formCheckListUnitView;
        }
    }
}
