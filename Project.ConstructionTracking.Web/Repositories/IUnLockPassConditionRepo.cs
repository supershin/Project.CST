using Project.ConstructionTracking.Web.Models;
using static Project.ConstructionTracking.Web.Models.UnLockPassConditionModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IUnLockPassConditionRepo
    {
        UnLockPassConditionModel.GetDataUnlockDetail GetListUnlockDetail(UnLockPassConditionModel.GetDataUnlockDetail model);
        List<UnLockPassConditionModel.GetDataUnlockPC> GetListUnlockPC(UnLockPassConditionModel.GetDataUnlockPC filterData);
        List<UnLockPassConditionModel.GetImageUnlock> GetImage(UnLockPassConditionModel.GetImageUnlock filterData);
        void RequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model);
    }
}
