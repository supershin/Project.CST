namespace Project.ConstructionTracking.Web.Models.QC5CheckModel
{
    public class QC5DetailModel
    {
        public Guid? ProjectID { get; set; }
        public string? ProjectsName { get; set; }
        public Guid? UnitID { get; set; }
        public string? UnitCode { get; set; }
        public int? UnitStatusID { get; set; }
        public string? UnitStatusName { get; set; }
        public Guid? QC5UnitChecklistID { get; set; }
        public int? QCTypeID { get; set; }
        public int? QC5UnitStatusID { get; set; }
        public int? Seq { get; set; }
        public Guid? UnitFormID { get; set; }
        public string? UnitFormName { get; set; }
        public int? UnitFormStatatusID { get; set; }
        public string? UnitFormStatatusName { get; set; }
        public int? FormID { get; set; }
        public string? FormName { get; set; }
        public string? QC5UpdateDate { get; set; }
        public string? QC5UpdateByName { get; set; }
        public string? ActionType { get; set; }
    }
}
