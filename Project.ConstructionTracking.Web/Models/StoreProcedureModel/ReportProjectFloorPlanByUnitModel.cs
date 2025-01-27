namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportProjectFloorPlanByUnitModel
    {
        public string? act { get; set; }
        public string? project_id { get; set; }
        public string? unit_id { get; set; }
        public string? unit_status { get; set; }
        public string? build_status { get; set; }
        public string? vender_id { get; set; }
        public string? qctype_id { get; set; }
        public string? project_floor_plan_id { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }

        public string? UnitCode { get; set; }
        public string? ModelTypeName { get; set; }
        public string? CompanyVendorName { get; set; }
        public string? UnitStatusName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? TransferDueDate { get; set; }
        public string? DelayAhead { get; set; }
        public int? FormID { get; set; }
        public string? FormName { get; set; }
        public Guid? ProjectID { get; set; }
        public string? ProjectName { get; set; }
    }
}
