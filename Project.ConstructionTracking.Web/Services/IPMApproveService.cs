using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IPMApproveService
    {
        List<PMApproveModel> GetPMApproveFormList();
        ApproveFormcheckModel GetApproveFormcheck(ApproveFormcheckModel model);
        List<UnitFormResourceModel> GetImage(UnitFormResourceModel model);
        string SaveOrUpdateUnitFormAction(ApproveFormcheckIUDModel model);
    }
}
