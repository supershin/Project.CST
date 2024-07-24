using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class GetDDLService : IGetDDLService
    {
        private readonly IGetDDLRepo _getDDLRepo;

        public GetDDLService(IGetDDLRepo getDDLRepo)
        {
            _getDDLRepo = getDDLRepo;
        }

        public List<GetDDL> GetDDLList(GetDDL Model)
        {
            var DDLList = _getDDLRepo.GetDDLList(Model);
            return DDLList;
        }
    }
}
