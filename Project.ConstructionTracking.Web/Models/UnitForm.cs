namespace Project.ConstructionTracking.Web.Models
{
    public class UnitForm
    {
        public Guid ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? FormID { get; set; }
        public decimal? Progress { get; set; }
        public int? Duration { get; set; }
        public int? StatusID { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }
    }
}
