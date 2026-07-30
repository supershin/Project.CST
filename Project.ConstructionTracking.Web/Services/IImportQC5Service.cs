using Project.ConstructionTracking.Web.Models.ImportQC5Model;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IImportQC5Service
    {
        ImportQC5ResultModel PreviewExcel(IFormFile fileExcel);
        ImportQC5ResultModel ImportExcel(IFormFile fileExcel, Guid userID);
    }
}
