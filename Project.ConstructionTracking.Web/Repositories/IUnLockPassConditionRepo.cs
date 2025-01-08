using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;
using static Project.ConstructionTracking.Web.Models.UnLockPassConditionModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IUnLockPassConditionRepo
    {
        UnLockPassConditionModel.GetDataUnlockDetail GetListUnlockDetail(UnLockPassConditionModel.GetDataUnlockDetail model);
        List<UnLockPassConditionModel.GetDataUnlockPC> GetListUnlockPC(UnLockPassConditionModel.GetDataUnlockPC filterData);
        List<UnLockPassConditionModel.GetImageUnlock> GetImage(UnLockPassConditionModel.GetImageUnlock filterData);
        List<PERequesUnlockModel> PERequestUnlockSendMail(int PC_ID, Guid UnitFormID);
        PMRespondUnlockModel PMRespondUnlockSendMail(int PC_ID, Guid UnitFormID);
        void RequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model);
    }
}
