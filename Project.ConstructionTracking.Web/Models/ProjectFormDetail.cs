namespace Project.ConstructionTracking.Web.Models
{
    public class ProjectFormDetail
    {
        public string? SaveDate_PE { get; set; }
        public Guid? SaveBy_PE { get; set; }
        public string? SaveBy_PE_Name { get; set; }

        public string? SubmitDate_PE { get; set; }
        public Guid? SubmitBy_PE { get; set; }
        public string? SubmitBy_PE_Name { get; set; }

        public string? SaveDate_QC { get; set; }
        public Guid? SaveBy_QC { get; set; }
        public string? SaveBy_QC_Name { get; set; }

        public string? SubmitDate_QC { get; set; }
        public Guid? SubmitBy_QC { get; set; }
        public string? SubmitBy_QC_Name { get; set; }

        public string? SaveDate_PM { get; set; }
        public Guid? SaveBy_PM { get; set; }
        public string? SaveBy_PM_Name { get; set; }

        public string? ApproveDate_PM { get; set; }
        public Guid? ApproveBy_PM { get; set; }
        public string? ApproveBy_PM_Name { get; set; }

        public string? RejectDate_PM { get; set; }
        public Guid? RejectBy_PM { get; set; }
        public string? RejectBy_PM_Name { get; set; }

        public string? ApproveDate_VP { get; set; }
        public Guid? ApproveBy_VP { get; set; }
        public string? ApproveBy_VP_Name { get; set; }

        public string? RejectDate_VP { get; set; }
        public Guid? RejectBy_VP { get; set; }
        public string? RejectBy_VP_Name { get; set; }

        public string? Remark_PE { get; set; }
        public string? Remark_QC { get; set; }
        public string? Remark_PM { get; set; }
        public string? Remark_VP { get; set; }
    }
}
