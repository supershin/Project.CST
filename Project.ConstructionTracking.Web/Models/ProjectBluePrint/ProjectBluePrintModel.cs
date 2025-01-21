namespace Project.ConstructionTracking.Web.Models.ProjectBluePrint
{
    public class ProjectBluePrintModel
    {
        public class BlueprintElementModel
        {
            public Guid ProjectFloorPlanID { get; set; }
            public int ElementType { get; set; } // "Marker" or "Polygon"
            public string? ElementTypeName { get; set; }
            public List<PointModel>? Coordinates { get; set; }
            public string PathProjectImage { get; set; }
            public Guid UnitID { get; set; }
            public string? UnitName { get; set; }
            public Guid UserID { get; set; }
        }

        public class PointModel
        {
            public float X { get; set; }
            public float Y { get; set; }
        }

        public class InsertImageProjectFloorPlanModel
        {
            public Guid ProjectID { get; set; }
            public Guid UserID { get; set; }
            public string? ApplicationPath { get; set; }
            public List<IFormFile>? Images { get; set; }
        }

        public class GetListImageProjectFloorPlanModel
        {
            public Guid? ProjectFloorPlanID { get; set; }
            public Guid? ResourceID { get; set; }
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
        }

        public class RemoveImageProjectFloorPlanModel
        {
            public Guid ProjectFloorPlanID { get; set; }
            public Guid UserID { get; set; }
        }
    }
}
