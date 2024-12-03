namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportWorkloadAndInspectionResultsModel
    {
        public string? act { get; set; }
        public string? project_id { get; set; }
        public string? unit_id { get; set; }
        public string? unit_status { get; set; }
        public string? build_status { get; set; }
        public string? vender_id { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }


        public int index { get; set; }
        public string? MonthName { get; set; }
        public string? MonthNumber { get; set; }
        public string? QC1Pass { get; set; }
        public string? QC1NotPass { get; set; }
        public string? QC1NotReady { get; set; }
        public string? TotalQC1 { get; set; }
        public string? QC1PercentPass { get; set; }
        public string? QC2Pass { get; set; }
        public string? QC2NotPass { get; set; }
        public string? QC2NotReady { get; set; }
        public string? TotalQC2 { get; set; }
        public string? QC2PercentPass { get; set; }
        public string? QC3Pass { get; set; }
        public string? QC3NotPass { get; set; }
        public string? QC3NotReady { get; set; }
        public string? TotalQC3 { get; set; }
        public string? QC3PercentPass { get; set; }
        public string? QC4Pass { get; set; }
        public string? QC4NotPass { get; set; }
        public string? QC4NotReady { get; set; }
        public string? TotalQC4 { get; set; }
        public string? QC4PercentPass { get; set; }
        public string? QC5Pass { get; set; }
        public string? QC5NotPass { get; set; }
        public string? QC5NotReady { get; set; }
        public string? TotalQC5 { get; set; }
        public string? QC5PercentPass { get; set; }
        public string? QCALLPass { get; set; }
        public string? QCALLNotPass { get; set; }
        public string? QCALLNotReady { get; set; }
        public string? TotalQCALL { get; set; }
        public string? QCALLPercentPass { get; set; }

        public string? RowOrder { get; set; }
    }
}
