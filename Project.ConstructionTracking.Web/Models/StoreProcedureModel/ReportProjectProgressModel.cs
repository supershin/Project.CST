namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportProjectProgressModel
    {      
        public string? act { get; set; }
        public string? project_id { get; set; }
        public string? unit_id { get; set; }
        public string? unit_status { get; set; }
        public string? build_status { get; set; }

        public int index { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? CompanyName { get; set; }
        public string? PEName { get; set; }
        public string? UnitStatus { get; set; }
        public string? TransferDate { get; set; } 
        public string? StartDatePlan { get; set; }
        public string? EndDatePlan { get; set; }
        public string? FormActual { get; set; }
        public string? ProgressPlan { get; set; }
        public string? ProgressActual { get; set; }
        public string? DelayAhead { get; set; }
        public string? LastFormTransfer { get; set; }
    }
}
