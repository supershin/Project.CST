using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class FormChecklistService : IFormChecklistService
    {
        private readonly IFormChecklistRepo _IFormChecklistRepo;

        public FormChecklistService(IFormChecklistRepo FormChecklistRepo)
        {
            _IFormChecklistRepo = FormChecklistRepo;
        }

        public List<FormCheckListModel.Form_getListPackages> GetFormCheckList(FormCheckListModel.Form_getFilterData filterData)
        {
            var ListFormChecklist = _IFormChecklistRepo.GetFormCheckList(filterData);
            return ListFormChecklist;
        }

        public List<FormCheckListModel.Form_getListStatus> GetFormCheckListStatus(FormCheckListModel.Form_getFilterData filterData)
        {
            var ListFormChecklistStatus = _IFormChecklistRepo.GetFormCheckListStatus(filterData);
            return ListFormChecklistStatus;
        }

        public void InsertOrUpdate(FormChecklistIUDModel model)
        {
            _IFormChecklistRepo.InsertOrUpdate(model);
        }
    }
}
