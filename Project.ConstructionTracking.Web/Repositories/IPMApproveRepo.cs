using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IPMApproveRepo
    {
        List<PMApproveModel> GetPMApproveFormList();
        ApproveFormcheckModel GetApproveFormcheck(ApproveFormcheckModel model);
        List<PMRequestModel> GetListPMRequesSendEmailData(Guid unitFormId);
        List<UnitFormResourceModel> GetImage(UnitFormResourceModel model);
        PMRespond GetPMRespondSendEmailData(Guid unitFormId);
        string SaveOrUpdateUnitFormAction(ApproveFormcheckIUDModel model);
    }
}
