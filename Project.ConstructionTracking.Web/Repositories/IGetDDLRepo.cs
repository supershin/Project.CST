using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IGetDDLRepo
    {
        List<GetDDL> GetDDLList(GetDDL Model);
    }
}
