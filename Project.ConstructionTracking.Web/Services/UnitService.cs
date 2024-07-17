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

        public List<UnitModel> GetUnitList(string Search, UnitModel Model)
        {
            var units = _unitRepo.GetUnitList(Search, Model);
            return units;
        }

        public dynamic GetUnitTypeList()
        {
            var units = _unitRepo.GetUnitTypeList();
            return units;
        }
    }
}
