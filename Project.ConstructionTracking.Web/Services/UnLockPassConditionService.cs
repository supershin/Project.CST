using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;
using static Project.ConstructionTracking.Web.Models.UnLockPassConditionModel;

namespace Project.ConstructionTracking.Web.Services
{
    public class UnLockPassConditionService : IUnLockPassConditionService
    {
        private readonly IUnLockPassConditionRepo _IUnLockPassConditionRepo;

        public UnLockPassConditionService(IUnLockPassConditionRepo UnLockPassConditionRepo)
        {
            _IUnLockPassConditionRepo = UnLockPassConditionRepo;
        }

        public UnLockPassConditionModel.GetDataUnlockDetail GetListUnlockDetail(UnLockPassConditionModel.GetDataUnlockDetail model)
        {
            var UnlockDetaiData = _IUnLockPassConditionRepo.GetListUnlockDetail(model);
            return UnlockDetaiData;
        }

        public List<UnLockPassConditionModel.GetDataUnlockPC> GetListUnlockPC(UnLockPassConditionModel.GetDataUnlockPC filterData)
        {
            var ListUnLockPassCondition = _IUnLockPassConditionRepo.GetListUnlockPC(filterData);
            return ListUnLockPassCondition;
        }

        public List<UnLockPassConditionModel.GetImageUnlock> GetImage(UnLockPassConditionModel.GetImageUnlock filterData)
        {
            var ListGetImage = _IUnLockPassConditionRepo.GetImage(filterData);
            return ListGetImage;
        }
        public void RequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model)
        {
            try
            {
                _IUnLockPassConditionRepo.RequestUnlock(model);
            }
            catch (Exception ex)
            {
                throw new Exception("บันทึกลงฐานข้อมูลไม่สำเร็จ", ex);
            }
        }

    }
}
