using Project.ConstructionTracking.Web.Models.MCompanyModel;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;
using Project.ConstructionTracking.Web.Services;
using System.Transactions;
using Project.ConstructionTracking.Web.Models.WebAPIRest;
using static Project.ConstructionTracking.Web.Infras.Repositories.WebAPIRestRepo;

namespace Project.ConstructionTracking.Web.Infras.Services
{
    public class WebAPIRestService
    {
        public interface IGRVenderrportalService
        {
            Task<RequestPostModel.GRVenderrportal.Responds> UploadFileAsync(RequestPostModel.GRVenderrportal.Sends request);
        }
        public class GRVenderrportalService : IGRVenderrportalService
        {
            private readonly IGRVenderrportalRepo _GRVenderrportalRepo;

            public GRVenderrportalService(IGRVenderrportalRepo GRVenderrportalRepo)
            {
                _GRVenderrportalRepo = GRVenderrportalRepo;
            }

            public async Task<RequestPostModel.GRVenderrportal.Responds> UploadFileAsync(RequestPostModel.GRVenderrportal.Sends request)
            {
                var responds = await _GRVenderrportalRepo.UploadFileAsync(request);
                return responds;
            }
        }

    }
}
