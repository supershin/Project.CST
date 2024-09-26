namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class UnitFormStatusModel
    {
        public int index { get; set; }
        public string? act { get; set; }
        public string? unit_id { get; set; }
        public string? project_id { get; set; }
        public string? unit_status { get; set; }
        public string? build_status { get; set; }

        public string? UnitCode { get; set; }
        public Guid? UnitFormID { get; set; }
        public string? Form { get; set; }
        public string? Vender { get; set; }
        public string? Progress { get; set; }
        public string? DurationDay { get; set; }
        public string? QC { get; set; }
        public string? StartPlan { get; set; }
        public string? EndPlan { get; set; }
        public string? PESave { get; set; }
        public string? PMSubmit { get; set; }
        public string? PCStatus { get; set; }
        public string? DisbursementStatus { get; set; }
        public string? PCUnlock { get; set; }
        public string? UnitFormPDF { get; set; }
        public string? QCPDF { get; set; }
    }
}
