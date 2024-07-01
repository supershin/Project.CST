using Project.ConstructionTracking.Web.Models;
using static Project.ConstructionTracking.Web.Models.ProjectFormModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IProjectFormRepo
    {
        List<ProjectFormModel.ProjectForm_getForm> GetFormCheckUnitList(int formID);
        ProjectFormDetail GetDetail(int unitID);
        void InsertFormCheckListUnit(UnitForm model);
    }
}
