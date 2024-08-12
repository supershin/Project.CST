using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IPMApproveService
    {
        List<PMApproveModel> GetPMApproveFormList();
        List<ApproveFormcheckModel> GetApproveFormcheckList(ApproveFormcheckModel model);
    }
}
