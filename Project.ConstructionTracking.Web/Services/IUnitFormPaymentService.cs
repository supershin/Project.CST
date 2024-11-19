using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IUnitFormPaymentService
    {
        UnitFormPaymentModel.getUnitFormGRDetail getUnitFormGRDetail(UnitFormPaymentModel.getUnitFormGRDetail Model);

        List<UnitFormPaymentModel.getListUnitFormGRPaymentTable> GetListUnitFormGRPaymentTable(UnitFormPaymentModel.getListUnitFormGRPaymentTable Model);

        string InsertNewGRPayment(UnitFormPaymentModel.IUDGRPayment Model);

        string RemoveGRPayment(UnitFormPaymentModel.IUDGRPayment Model);

        string SyncGRPayment(UnitFormPaymentModel.IUDGRPayment Model);
    }
}
