using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IUnitRepo
    {
        List<UnitModel> GetUnitList(string Search, UnitModel Model);

        dynamic GetUnitTypeList();
    }
}
