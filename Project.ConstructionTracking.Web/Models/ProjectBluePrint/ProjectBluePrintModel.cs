namespace Project.ConstructionTracking.Web.Models.ProjectBluePrint
{
    public class ProjectBluePrintModel
    {
        public class BlueprintElementModel
        {
            public Guid ProjectID { get; set; }
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

    }
}
