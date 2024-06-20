using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepo _unitRepo;

        public UnitService(IUnitRepo unitRepo)
        {
          _unitRepo = unitRepo;
        }
        public dynamic GetUnitList(Criteria criteria, DTParamModel param)
        {
            var units = _unitRepo.GetUnitList(criteria, param);
            return units;
        }
    }
}
