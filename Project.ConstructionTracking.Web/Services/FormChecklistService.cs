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

        public FormCheckListModel.Form_getUnitFormData GetUnitFormData(FormCheckListModel.Form_getUnitFormData filterData)
        {
            var UnitFormData = _IFormChecklistRepo.GetUnitFormData(filterData);
            return UnitFormData;
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

        public void InsertOrUpdate(FormChecklistIUDModel model, Guid? userID, int RoleID)
        {
            _IFormChecklistRepo.InsertOrUpdate(model , userID , RoleID);
        }

        public bool DeleteImage(Guid resourceId, string ApplicationPath)
        {
            var Result = _IFormChecklistRepo.DeleteImage(resourceId, ApplicationPath);
            return Result;
        }

        //public async Task InsertOrUpdate(FormChecklistIUDModel model, IFormFileCollection files)
        //{
        //    var unitFormIDUse = _IFormChecklistRepo.InsertOrUpdate(model);
        //    if (files != null && files.Count > 0)
        //    {
        //        await _IFormChecklistRepo.UploadFiles(unitFormIDUse, files);
        //    }
        //}
    }
}
