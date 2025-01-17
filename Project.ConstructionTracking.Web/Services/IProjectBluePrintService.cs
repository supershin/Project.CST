using Project.ConstructionTracking.Web.Models.ProjectBluePrint;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IProjectBluePrintService
    {
        public List<ProjectBluePrintModel.BlueprintElementModel> GetListProjectBlueprintElements(Guid ProjectID);
        void SaveBlueprintElements(List<ProjectBluePrintModel.BlueprintElementModel> elements);
        void InsertImageProjectFloorPlan(ProjectBluePrintModel.InsertImageProjectFloorPlanModel mode);
        public List<ProjectBluePrintModel.GetListImageProjectFloorPlanModel> GetListImageProjectFloorPlan(Guid ProjectID);
    }
}
