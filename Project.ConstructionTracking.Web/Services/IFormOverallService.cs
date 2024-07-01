using Project.ConstructionTracking.Web.Models;
namespace Project.ConstructionTracking.Web.Services
{
    public interface IFormOverallService
    { 
        ProjectFormListView GetProjectFormList(Guid formId, int typeId);
    }
}
