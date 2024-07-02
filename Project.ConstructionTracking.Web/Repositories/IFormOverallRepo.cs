using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IFormOverallRepo
    {
        List<ProjectFormList> ProjectFormLists(Guid formId, int typeId);
    }
}
