using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class SummeryUnitFormService : ISummeryUnitFormService
    {
        private readonly ISummeryUnitFormRepo _ISummeryUnitFormRepoRepo;

        public SummeryUnitFormService(ISummeryUnitFormRepo SummeryUnitFormRepoRepo)
        {
            _ISummeryUnitFormRepoRepo = SummeryUnitFormRepoRepo;
        }
        public List<SummeryUnitForm> GetSummeryUnitFormList(SummeryUnitForm Model)
        {
            var ListSummeryUnitForm = _ISummeryUnitFormRepoRepo.GetSummeryUnitFormList(Model);
            return ListSummeryUnitForm;
        }
    }
}
