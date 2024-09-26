using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IFormGroupRepo
    {
        List<FormGroupModel> GetFormGroupList(FormGroupModel Model);
        FormGroupModel.FormGroupDetail GetFormGroupDetail(Guid? unitFormId);
        bool ValidateUserSubmit(Guid? UserID, Guid? ProjectID);
        void SubmitSaveFormGroup(FormGroupModel.FormGroupIUDModel model);
    }
}
