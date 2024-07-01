using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IUnitRepo
    {
        dynamic GetUnitList(Criteria criteria,DTParamModel param);

        dynamic GetUnitTypeList();
    }
}
