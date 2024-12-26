using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IUnitFormPaymentService
    {
        UnitFormPaymentModel.getUnitFormGRDetail getUnitFormGRDetail(UnitFormPaymentModel.getUnitFormGRDetail Model);

        List<UnitFormPaymentModel.getListUnitFormGRPaymentTable> GetListUnitFormGRPaymentTable(UnitFormPaymentModel.getListUnitFormGRPaymentTable Model);

        UnitPaymentMail getUnitFormSendmMailDetail(Guid UnitFormID);

        int InsertNewGRPayment(UnitFormPaymentModel.IUDGRPayment Model);

        string RemoveGRPayment(UnitFormPaymentModel.IUDGRPayment Model);

        int SyncGRPayment(UnitFormPaymentModel.IUDGRPayment Model);
    }
}
