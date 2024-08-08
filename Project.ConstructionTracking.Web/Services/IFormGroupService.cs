using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IFormGroupService
    {
        List<FormGroupModel> GetFormGroupList(FormGroupModel Model);
        FormGroupModel.FormGroupDetail GetFormGroupDetail(Guid unitFormId);
        void SubmitSaveFormGroup(FormGroupModel.FormGroupIUDModel model);
    }
}
