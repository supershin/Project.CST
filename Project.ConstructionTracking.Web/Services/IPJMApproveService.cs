using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IPJMApproveService
    {
        List<PJMApproveModel.GetlistUnitDetail> GetListPJMApprove(PJMApproveModel.filterData filterData);
        List<PJMApproveModel.GetlistChecklistPC> GetChecklistPJMApprove(PJMApproveModel.GetlistChecklistPC filterData);
        List<PJMApproveModel.GetImageUnlock> GetImageUnlock(PJMApproveModel.GetImageUnlock filterData);
        List<PJMRespondModel> GetPJMRespondSendEmailData(Guid unitFormId);
        string SaveOrUpdateUnitFormAction(PJMApproveModel.PJMApproveIU model);
        public AdminRespond GetAdminPJMRespond(Guid unitFormId);
    }
}
