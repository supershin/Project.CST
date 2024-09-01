namespace Project.ConstructionTracking.Web.Models.SummeryUnitModel
{
    public class SummeryUnitFormDetailModel
    {
        public Guid? ProjectID { get; set; }
        public string? ProjectsName { get; set; }
        public Guid? UnitID { get; set; }
        public string? UnitCode { get; set; }
        public int? UnitStatusID { get; set; }
        public string? UnitStatusName { get; set; }
        public Guid? UnitFormID { get; set; }
        public string? UnitFormName { get; set; }
        public int? FormID { get; set; }
        public string? FormName { get; set; }
    }
}
