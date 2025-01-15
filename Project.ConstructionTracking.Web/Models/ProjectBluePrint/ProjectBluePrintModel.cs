namespace Project.ConstructionTracking.Web.Models.ProjectBluePrint
{
    public class ProjectBluePrintModel
    {
        public class BlueprintElementModel
        {
            public Guid? ProjectID { get; set; }
            public string? ElementType { get; set; } // "Marker" or "Polygon"
            public List<PointModel>? Coordinates { get; set; }
            public string? UnitName { get; set; }
        }

        public class PointModel
        {
            public float X { get; set; }
            public float Y { get; set; }
        }

    }
}
