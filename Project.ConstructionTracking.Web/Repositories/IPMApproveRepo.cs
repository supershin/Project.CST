using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IPMApproveRepo
    {
        List<PMApproveModel> GetPMApproveFormList();
        List<ApproveFormcheckModel> GetApproveFormcheckList(ApproveFormcheckModel model);
    }
}
