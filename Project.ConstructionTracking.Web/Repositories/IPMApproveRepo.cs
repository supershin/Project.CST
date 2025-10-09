using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IPMApproveRepo
    {
        List<PMApproveModel> GetPMApproveFormList();
        ApproveFormcheckModel GetApproveFormcheck(ApproveFormcheckModel model);
        List<PMRequestModel> GetListPMRequesSendEmailData(Guid unitFormId);
        List<QCnotifyPMSubmit> GetListQCnotifyPMSubmitlData(int FormID, Guid UnitID, Guid ProjectID);
        List<UnitFormResourceModel> GetImage(UnitFormResourceModel model);
        PMRespond GetPMRespondSendEmailData(Guid unitFormId);
        string SaveOrUpdateUnitFormAction(ApproveFormcheckIUDModel model);
        public int CheckQCbyFormID(int FormID);
        public string GetProjectcodeByID(Guid ProjectID);
        public int CheckQCSync(Guid UnitID);
        public AdminRespond GetAdminRespond(Guid unitFormId);
    }
}
