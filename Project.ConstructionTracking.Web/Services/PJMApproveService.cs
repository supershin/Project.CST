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

    }
}
