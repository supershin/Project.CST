using Project.ConstructionTracking.Web.Models;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IFormChecklistRepo
    {
        FormCheckListModel.Form_getUnitFormData GetUnitFormData(FormCheckListModel.Form_getUnitFormData filterData);
        List<FormCheckListModel.Form_getListPackages> GetFormCheckList(FormCheckListModel.Form_getFilterData filterData);
        List<FormCheckListModel.Form_getListStatus> GetFormCheckListStatus(FormCheckListModel.Form_getFilterData filterData);
        void InsertOrUpdate(FormChecklistIUDModel model, Guid? userID, int RoleID);
        bool DeleteImage(Guid resourceId, string ApplicationPath);
        //Guid InsertOrUpdate(FormChecklistIUDModel model);
        //Task UploadFiles(Guid unitFormID, IFormFileCollection files); // เพิ่มเมธอดใหม่
    }
}
