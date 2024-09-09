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
            try
            {
                _IFormChecklistRepo.InsertOrUpdate(model, userID, RoleID);
            }
            catch (Exception ex)
            {
                throw new Exception("บันทึกลงฐานข้อมูลไม่สำเร็จ", ex);
            }
        }

        public bool DeleteImage(Guid resourceId, string ApplicationPath)
        {
            try
            {
                var result = _IFormChecklistRepo.DeleteImage(resourceId, ApplicationPath);
                return result;
            }
            catch (Exception ex)
            {
                // Handle any exceptions, log if necessary
                throw new Exception("ลบรูปภาพไม่สำเร็จ", ex);
            }
        }

    }
}
