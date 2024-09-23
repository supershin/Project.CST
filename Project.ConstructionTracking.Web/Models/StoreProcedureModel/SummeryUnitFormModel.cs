namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class SummeryUnitFormModel
    {
        public int index { get; set; }
        public string? act { get; set; }
        public string? unit_id { get; set; }
        public string? project_id { get; set; }
        public string? unit_status { get; set; }
        public string? user_id { get; set; }

        public Guid? UnitID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitFormID { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public int? FormID { get; set; }
        public string? FormName { get; set; }
        public int? StatusID { get; set; }
        public string? PEColor { get; set; }
        public string? QC1 { get; set; }
        public string? QC2 { get; set; }
        public string? QC3 { get; set; }
        public string? QC4_1 { get; set; }
        public string? QC4_2 { get; set; }
        public string? QC5 { get; set; }
        public string? PMColor { get; set; }
        public int? Cnt_PC { get; set; }
        public string? PCColor { get; set; }
        public string? PCIcon { get; set; }
    }
}
