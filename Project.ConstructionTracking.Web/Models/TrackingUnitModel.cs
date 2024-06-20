namespace Project.ConstructionTracking.Web.Models
{
    public class TrackingUnitModel
    {
        public int ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? FormID { get; set; }
        public string FormName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Progress { get; set; }
        public int? Duration { get; set; }
        public int? StatusID { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

       public TrackingUnitRef FormRef { get; set; } = new TrackingUnitRef();
    }
}
