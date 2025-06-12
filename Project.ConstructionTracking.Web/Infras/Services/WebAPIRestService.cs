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
        public interface IWebAPIRestService
        {
            Task<RequestPostModel.GRVenderrportal.Responds> UploadFileAsync(RequestPostModel.GRVenderrportal.Sends request);
            Task<RequestPostModel.Get_User_CRM.Responds> CentralizeGetUserCRM(RequestPostModel.Get_User_CRM.Sends request);
            Task<RequestPostModel.QC_Status_Update_QC5.Responds> QcStatusUpdateQc5(RequestPostModel.QC_Status_Update_QC5.Sends request);
        }
        public class _WebAPIRestService : IWebAPIRestService
        {
            private readonly IWebAPIRestRepo _WebAPIRestRepositorys;

            public _WebAPIRestService(IWebAPIRestRepo WebAPIRestRepositorys)
            {
                _WebAPIRestRepositorys = WebAPIRestRepositorys;
            }

            public async Task<RequestPostModel.GRVenderrportal.Responds> UploadFileAsync(RequestPostModel.GRVenderrportal.Sends request)
            {
                var responds = await _WebAPIRestRepositorys.UploadFileAsync(request);
                return responds;
            }
            public async Task<RequestPostModel.Get_User_CRM.Responds> CentralizeGetUserCRM(RequestPostModel.Get_User_CRM.Sends request)
            {
                var responds = await _WebAPIRestRepositorys.CentralizeGetUserCRM(request);
                return responds;
            }
            public async Task<RequestPostModel.QC_Status_Update_QC5.Responds> QcStatusUpdateQc5(RequestPostModel.QC_Status_Update_QC5.Sends request)
            {
                var responds = await _WebAPIRestRepositorys.QcStatusUpdateQc5(request);
                return responds;
            }
        }

    }
}
