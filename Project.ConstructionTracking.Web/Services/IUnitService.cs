using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IUnitService
    {
        List<UnitModel> GetUnitList(string Search, UnitModel Model);

        dynamic GetUnitTypeList();
    }
}
