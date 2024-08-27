using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class UnLockPassConditionService : IUnLockPassConditionService
    {
        private readonly IUnLockPassConditionRepo _IUnLockPassConditionRepo;

        public UnLockPassConditionService(IUnLockPassConditionRepo UnLockPassConditionRepo)
        {
            _IUnLockPassConditionRepo = UnLockPassConditionRepo;
        }

        public List<UnLockPassConditionModel.GetDataUnlockPC> GetListUnlockPC(UnLockPassConditionModel.GetDataUnlockPC filterData)
        {
            var ListUnLockPassCondition = _IUnLockPassConditionRepo.GetListUnlockPC(filterData);
            return ListUnLockPassCondition;
        }
    }
}
