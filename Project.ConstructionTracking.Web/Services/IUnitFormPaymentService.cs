using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IUnitFormPaymentService
    {
        UnitFormPaymentModel.getUnitFormGRDetail getUnitFormGRDetail(UnitFormPaymentModel.getUnitFormGRDetail Model);
        string InsertNewGRPayment(UnitFormPaymentModel.insertGRPayment Model);
    }
}
