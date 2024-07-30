using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IFormGroupService
    {
        List<FormGroupModel> GetFormGroupList(FormGroupModel Model);
    }
}
