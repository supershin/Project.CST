using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IFormGroupService
    {
        List<FormGroupModel> GetFormGroupList(FormGroupModel Model);
        FormGroupModel.FormGroupDetail GetFormGroupDetail(Guid? unitFormId);
        List<PERequesModel> GetListPERequesSendEmailData(Guid unitFormId);
        List<QCnotifyPESubmit> GetListQCnotifyPESubmitlData(int FormID, Guid UnitID, Guid ProjectID);
        bool ValidateUserSubmit(Guid? UserID, Guid? UnitID);
        void SubmitSaveFormGroup(FormGroupModel.FormGroupIUDModel model);
    }
}
