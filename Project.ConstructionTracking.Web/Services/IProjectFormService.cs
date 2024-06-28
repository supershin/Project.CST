using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IProjectFormService
    {
        FormCheckListUnitView GetFormCheckListUnit(int formID , int unitID);
        void InsertFormCheckListUnit(FormCheckListUnitView model);
    }
}
