using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IUnitFormPaymentRepo
    {
        UnitFormPaymentModel.getUnitFormGRDetail getUnitFormGRDetail(UnitFormPaymentModel.getUnitFormGRDetail Model);
        string InsertNewGRPayment(UnitFormPaymentModel.insertGRPayment Model);
    }
}
