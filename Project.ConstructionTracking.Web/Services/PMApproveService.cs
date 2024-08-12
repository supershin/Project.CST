using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class PMApproveService : IPMApproveService
    {
        private readonly IPMApproveRepo _IPMApprovelistRepo;

        public PMApproveService(IPMApproveRepo PMApprovelistRepo)
        {
            _IPMApprovelistRepo = PMApprovelistRepo;
        }

        public List<PMApproveModel> GetPMApproveFormList()
        {
            var ListPMApprove = _IPMApprovelistRepo.GetPMApproveFormList();
            return ListPMApprove;
        }

        public List<ApproveFormcheckModel> GetApproveFormcheckList(ApproveFormcheckModel model)
        {
            var ListPMApprove = _IPMApprovelistRepo.GetApproveFormcheckList(model);
            return ListPMApprove;
        }
    }
}
