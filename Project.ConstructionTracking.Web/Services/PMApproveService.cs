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

        public ApproveFormcheckModel GetApproveFormcheck(ApproveFormcheckModel model)
        {
            var PMApproveData = _IPMApprovelistRepo.GetApproveFormcheck(model);
            return PMApproveData;
        }

        public List<UnitFormResourceModel> GetImage(UnitFormResourceModel model)
        {
            var ListImage = _IPMApprovelistRepo.GetImage(model);
            return ListImage;
        }

        public void SaveOrUpdateUnitFormAction(ApproveFormcheckIUDModel model)
        {
            _IPMApprovelistRepo.SaveOrUpdateUnitFormAction(model);
        }
    }
}
