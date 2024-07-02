using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IUnitService
    {
        dynamic GetUnitList(Criteria criteria, DTParamModel param);

        dynamic GetUnitTypeList();
    }
}
