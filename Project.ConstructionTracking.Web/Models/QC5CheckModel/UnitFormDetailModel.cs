namespace Project.ConstructionTracking.Web.Models.QC5CheckModel
{
    public class UnitFormDetailModel
    {
        public Guid? ID { get; set; }
        public Guid ProjectID { get; set; }
        public Guid UnitID { get; set; }
        public int? FormID { get; set; }
        public int StatusID { get; set; }
        public string? FormName { get; set; }
        public string? StatusName { get; set; }
    }
}
