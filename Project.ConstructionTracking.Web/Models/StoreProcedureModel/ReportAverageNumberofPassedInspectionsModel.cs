namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportAverageNumberofPassedInspectionsModel
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
        public string? ProjectName { get; set; }
        public string? CompanyVendorName { get; set; }
        public string? AverageQC1Pass { get; set; }
        public string? CntUnitQC1Pass { get; set; }
        public string? SumQC1MaxPass { get; set; }
        public string? AverageQC2Pass { get; set; }
        public string? CntUnitQC2Pass { get; set; }
        public string? SumQC2MaxPass { get; set; }
        public string? AverageQC3Pass { get; set; }
        public string? CntUnitQC3Pass { get; set; }
        public string? SumQC3MaxPass { get; set; }
        public string? AverageQC4Pass { get; set; }
        public string? CntUnitQC4Pass { get; set; }
        public string? SumQC4MaxPass { get; set; }
        public string? AverageQC5Pass { get; set; }
        public string? CntUnitQC5Pass { get; set; }
        public string? SumQC5MaxPass { get; set; }

    }
}
