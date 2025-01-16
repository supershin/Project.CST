using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.ProjectBluePrint;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IProjectBluePrintRepo
    {
        public List<ProjectBluePrintModel.BlueprintElementModel> GetListProjectBlueprintElements(Guid ProjectID);
        void SaveBlueprintElements(List<ProjectBluePrintModel.BlueprintElementModel> elements);
    }
}
