using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class FormGroupService : IFormGroupService
    {
        private readonly IFormGroupRepo _IFormGroupRepo;

        public FormGroupService(IFormGroupRepo FormGroupRepo)
        {
            _IFormGroupRepo = FormGroupRepo;
        }

        public List<FormGroupModel> GetFormGroupList(FormGroupModel Model)
        {
            var ListFormGroup = _IFormGroupRepo.GetFormGroupList(Model);
            return ListFormGroup;
        }

        public FormGroupModel.FormGroupDetail GetFormGroupDetail(Guid? unitFormId)
        {
            var FormGroupDetail = _IFormGroupRepo.GetFormGroupDetail(unitFormId);
            return FormGroupDetail;
        }

        public bool ValidateUserSubmit(Guid? UserID, Guid? UnitID)
        {
            bool result = _IFormGroupRepo.ValidateUserSubmit(UserID , UnitID);
            return result;
        }

        public void SubmitSaveFormGroup(FormGroupModel.FormGroupIUDModel model)
        {
            try
            {
                _IFormGroupRepo.SubmitSaveFormGroup(model);
            }
            catch (Exception ex)
            {
                throw new Exception("บันทึกลงฐานข้อมูลไม่สำเร็จ", ex);
            }
        }

    }
}
