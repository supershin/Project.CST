using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class UnitFormPaymentService : IUnitFormPaymentService
    {
        private readonly IUnitFormPaymentRepo _IUnitFormPaymentRepo;

        public UnitFormPaymentService(IUnitFormPaymentRepo UnitFormPaymentRepo)
        {
            _IUnitFormPaymentRepo = UnitFormPaymentRepo;
        }

        public UnitFormPaymentModel.getUnitFormGRDetail getUnitFormGRDetail(UnitFormPaymentModel.getUnitFormGRDetail Model)
        {
            var UnitFormGRDetail = _IUnitFormPaymentRepo.getUnitFormGRDetail(Model);
            return UnitFormGRDetail;
        }

        public string InsertNewGRPayment(UnitFormPaymentModel.insertGRPayment Model)
        {
            try
            {
                return _IUnitFormPaymentRepo.InsertNewGRPayment(Model);
            }
            catch (Exception ex)
            {
                throw new Exception("เกิดเหตุขัดข้องบันทึกไม่สำเร็จ", ex);
            }
        }

    }
}
