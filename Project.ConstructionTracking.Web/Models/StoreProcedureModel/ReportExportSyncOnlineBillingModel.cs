namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportExportSyncOnlineBillingModel
    {
        public string? act { get; set; }
        public string? unit_id { get; set; }
        public string? project_id { get; set; }
        public string? unit_status { get; set; }
        public string? user_id { get; set; }

        public int index { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? FormName { get; set; }
        public string? GRNO { get; set; }
        public string? PONO { get; set; }
        public string? CompanyVenderName { get; set; }
        public string? VenderName { get; set; }
        public string? PercentPayment { get; set; }
        public string? Remark { get; set; }
        public string? SyncStatusName { get; set; }
        public string? SyncMessage { get; set; }
        public string? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }
    }
}
