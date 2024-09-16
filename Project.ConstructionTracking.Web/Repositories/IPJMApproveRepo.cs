using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IPJMApproveRepo
    {
        List<PJMApproveModel.GetlistUnitDetail> GetListPJMApprove(PJMApproveModel.filterData filterData);
        List<PJMApproveModel.GetlistChecklistPC> GetChecklistPJMApprove(PJMApproveModel.GetlistChecklistPC filterData);
        List<PJMApproveModel.GetImageUnlock> GetImageUnlock(PJMApproveModel.GetImageUnlock filterData);
        void SaveOrUpdateUnitFormAction(PJMApproveModel.PJMApproveIU model);
    }
}
