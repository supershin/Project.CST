namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportinspectionQC5Model
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
        public string? ProjectID { get; set; }            // Project ID
        public string? ProjectName { get; set; }          // Project Name
        public string? UnitCode { get; set; }             // Unit Code
        public string? ModelTypeName { get; set; }        // Model Type Name
        public string? CompanyVendorName { get; set; }    // Company Vendor Name
        public string? MAXSeq { get; set; }                  // Maximum Sequence
        public string? FirstDateCheck { get; set; }       // First Date Check
        public string? DatePass { get; set; }             // Date Pass
        public string? CNTMajorDefect { get; set; }          // Count of Major Defects
        public string? CNTDefect { get; set; }               // Count of Defects
    }
}
