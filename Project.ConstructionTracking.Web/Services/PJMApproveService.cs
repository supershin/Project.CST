using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class PJMApproveService : IPJMApproveService
    {
        private readonly IPJMApproveRepo _IPJMApproveRepo;

        public PJMApproveService(IPJMApproveRepo PJMApproveRepo)
        {
            _IPJMApproveRepo = PJMApproveRepo;
        }
        public List<PJMApproveModel.GetlistUnitDetail> GetListPJMApprove(PJMApproveModel.filterData filterData)
        {
            var ListPJMApprove = _IPJMApproveRepo.GetListPJMApprove(filterData);
            return ListPJMApprove;
        }
        public List<PJMApproveModel.GetlistChecklistPC> GetChecklistPJMApprove(PJMApproveModel.GetlistChecklistPC filterData)
        {
            var ListChecklistPJMApprove = _IPJMApproveRepo.GetChecklistPJMApprove(filterData);
            return ListChecklistPJMApprove;
        }
        public void SaveOrUpdateUnitFormAction(PJMApproveModel.PJMApproveIU model)
        {
            _IPJMApproveRepo.SaveOrUpdateUnitFormAction(model);
        }
    }
}
