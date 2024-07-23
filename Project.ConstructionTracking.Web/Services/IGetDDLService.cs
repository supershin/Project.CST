using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IGetDDLService
    {
        List<GetDDL> GetDDLList(GetDDL Model);
    }
}
