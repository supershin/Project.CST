namespace Project.ConstructionTracking.Web.Models
{
    public class TrackingUnitView
    {
        public UnitModel Unit { get; set; }
        public List<TrackingUnitModel> TrackingUnitList { get; set; } = new List<TrackingUnitModel>();
    }
}
