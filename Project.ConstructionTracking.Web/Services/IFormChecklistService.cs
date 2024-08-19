using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IFormChecklistService
    {
        FormCheckListModel.Form_getUnitFormData GetUnitFormData(FormCheckListModel.Form_getUnitFormData filterData);

        List<FormCheckListModel.Form_getListPackages> GetFormCheckList(FormCheckListModel.Form_getFilterData filterData);

        List<FormCheckListModel.Form_getListStatus> GetFormCheckListStatus(FormCheckListModel.Form_getFilterData filterData);

        void InsertOrUpdate(FormChecklistIUDModel model , Guid? userID , int RoleID);

        bool DeleteImage(Guid resourceId, string ApplicationPath);
        
        //Task InsertOrUpdate(FormChecklistIUDModel model, IFormFileCollection files); // เพิ่มเมธอดใหม่
    }
}
