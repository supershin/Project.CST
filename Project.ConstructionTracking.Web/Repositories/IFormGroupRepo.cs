using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Models.SendMail;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IFormGroupRepo
    {
        List<FormGroupModel> GetFormGroupList(FormGroupModel Model);
        FormGroupModel.FormGroupDetail GetFormGroupDetail(Guid? unitFormId);
        List<PERequesModel> GetListPERequesSendEmailData(Guid unitFormId);
        List<QCnotifyPESubmit> GetListQCnotifyPESubmitlData(int FormID, Guid UnitID, Guid ProjectID);
        bool ValidateUserSubmit(Guid? UserID, Guid? ProjectID);
        void SubmitSaveFormGroup(FormGroupModel.FormGroupIUDModel model);
    }
}
