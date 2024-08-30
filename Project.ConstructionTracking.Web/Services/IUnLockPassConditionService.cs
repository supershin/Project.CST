using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IUnLockPassConditionService
    {
        List<UnLockPassConditionModel.GetDataUnlockPC> GetListUnlockPC(UnLockPassConditionModel.GetDataUnlockPC filterData);
        List<UnLockPassConditionModel.GetImageUnlock> GetImage(UnLockPassConditionModel.GetImageUnlock filterData);
        void RequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model);
    }
}
