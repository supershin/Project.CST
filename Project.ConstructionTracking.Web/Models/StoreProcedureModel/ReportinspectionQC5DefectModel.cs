namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportinspectionQC5DefectModel
    {
        public string? act { get; set; }
        public string? project_id { get; set; }
        public string? unit_id { get; set; }
        public string? unit_status { get; set; }
        public string? build_status { get; set; }
        public string? vender_id { get; set; }
        public string? qctype_id { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }


        public int index { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? CompanyVendorName { get; set; }
        public string? ActionDate { get; set; }
        public string? DefectAreaName { get; set; }
        public string? DefectTypeName { get; set; }
        public string? DefectDescription { get; set; }
        public string? IsMajorDefect { get; set; }
        public string? StatusPresent { get; set; }
        public string? SeqPass { get; set; }
        public string? QCUser { get; set; }
        public string? Remark { get; set; }
    }
}
