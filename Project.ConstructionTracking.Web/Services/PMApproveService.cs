using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;
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

        public PMRespond GetPMRespondSendEmailData(Guid unitFormId)
        {
            var PMRespondSendEmailData = _IPMApprovelistRepo.GetPMRespondSendEmailData(unitFormId);
            return PMRespondSendEmailData;
        }

        public string SaveOrUpdateUnitFormAction(ApproveFormcheckIUDModel model)
        {
            try
            {
                return _IPMApprovelistRepo.SaveOrUpdateUnitFormAction(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
