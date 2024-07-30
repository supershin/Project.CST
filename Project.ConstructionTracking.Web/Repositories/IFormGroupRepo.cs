using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IFormGroupRepo
    {
        List<FormGroupModel> GetFormGroupList(FormGroupModel Model);
    }
}
