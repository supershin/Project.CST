namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportProjectFloorPlanModel
    {
        public string? act { get; set; }
        public string? project_id { get; set; }
        public string? unit_id { get; set; }
        public string? unit_status { get; set; }
        public string? build_status { get; set; }
        public string? vender_id { get; set; }
        public string? qctype_id { get; set; }
        public string? project_floor_plan_id { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public int index { get; set; }

        public Guid ProjectFloorPlanID { get; set; }
        public int ElementType { get; set; } // "Marker" or "Polygon"
        public string? ElementTypeName { get; set; }
        public List<PointModel>? Coordinates { get; set; }
        public string PathProjectImage { get; set; }
        public Guid UnitID { get; set; }
        public string? UnitName { get; set; }
        public int? UnitStatus { get; set; }
        public Guid UserID { get; set; }
    }
    public class PointModel
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
