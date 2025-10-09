using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IPMApproveService
    {
        List<PMApproveModel> GetPMApproveFormList();
        ApproveFormcheckModel GetApproveFormcheck(ApproveFormcheckModel model);
        List<UnitFormResourceModel> GetImage(UnitFormResourceModel model);
        PMRespond GetPMRespondSendEmailData(Guid unitFormId);
        List<PMRequestModel> GetListPMRequesSendEmailData(Guid unitFormId);
        List<QCnotifyPMSubmit> GetListQCnotifyPMSubmitlData(int FormID, Guid UnitID, Guid ProjectID);
        string SaveOrUpdateUnitFormAction(ApproveFormcheckIUDModel model);
        public int CheckQCbyFormID(int FormID);
        public string GetProjectcodeByID(Guid ProjectID);
        public int CheckQCSync(Guid UnitID);
        public AdminRespond GetAdminRespond(Guid unitFormId);
    }
}
