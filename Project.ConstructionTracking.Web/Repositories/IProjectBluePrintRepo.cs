using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.ProjectBluePrint;
using static Project.ConstructionTracking.Web.Models.ProjectBluePrint.ProjectBluePrintModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IProjectBluePrintRepo
    {
        public List<ProjectBluePrintModel.BlueprintElementModel> GetListProjectBlueprintElements(Guid ProjectFloorPlanID);
        void SaveBlueprintElements(List<ProjectBluePrintModel.BlueprintElementModel> elements);
        void InsertImageProjectFloorPlan(ProjectBluePrintModel.InsertImageProjectFloorPlanModel mode);
        public List<ProjectBluePrintModel.GetListImageProjectFloorPlanModel> GetListImageProjectFloorPlan(Guid ProjectID);
        void RemoveImageProjectFloorPlan(ProjectBluePrintModel.RemoveImageProjectFloorPlanModel model);
    }
}
