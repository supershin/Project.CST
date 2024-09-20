namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class UnitStatusModel
    {
        public int index { get; set; }
        public string? act { get; set; }
        public string? unit_id { get; set; }
        public string? project_id { get; set; }
        public string? unit_status { get; set; }
        public string? build_status { get; set; }
        public string? unit_code { get; set; }
        public string? model_type_str { get; set; }
        public string? unit_type_str { get; set; }
        public string? unit_status_str { get; set; }
        public string? unit_build_status_str { get; set; }
        public string? date_start_plan_str { get; set; }
        public string? date_end_plan_str { get; set; }
        public string? formname_plan_str { get; set; }
        public string? formname_true_str { get; set; }
        public string? process_plan_str { get; set; }
        public string? process_true_str { get; set; }
        public string? delay_ahead_str { get; set; }
        public string? allday_str { get; set; }
        public string? realday_use_str { get; set; }
    }
}
