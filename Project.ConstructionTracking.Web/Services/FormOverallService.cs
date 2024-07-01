using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class FormOverallService : IFormOverallService
    {
        private readonly IFormOverallRepo _formOverallRepo;

        public FormOverallService(IFormOverallRepo formOverallRepo)
        {
            _formOverallRepo = formOverallRepo;
        }

        public ProjectFormListView GetProjectFormList(Guid formId,int typeId)
        {
            var ProjectFormList = new ProjectFormListView();
            ProjectFormList.ProjectFromView = _formOverallRepo.ProjectFormLists(formId, typeId);
            return ProjectFormList;
        }
    }
}
