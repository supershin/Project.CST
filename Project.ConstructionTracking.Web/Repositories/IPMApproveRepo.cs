using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IPMApproveRepo
    {
        List<PMApproveModel> GetPMApproveFormList();
        ApproveFormcheckModel GetApproveFormcheck(ApproveFormcheckModel model);
        List<UnitFormResourceModel> GetImage(UnitFormResourceModel model);
        string SaveOrUpdateUnitFormAction(ApproveFormcheckIUDModel model);
    }
}
