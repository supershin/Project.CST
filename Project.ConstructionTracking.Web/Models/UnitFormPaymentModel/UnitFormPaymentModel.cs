namespace Project.ConstructionTracking.Web.Models.UnitFormPaymentModel
{
    public class UnitFormPaymentModel
    {
        public class getUnitFormGRDetail
        {
            public Guid? ProjectID { get; set; }
            public string? ProjectName { get; set; }
            public Guid? UnitID { get; set; }
            public string? UnitCode { get; set; }
            public Guid? UnitFormID { get; set; }
            public string? FormName { get; set; }
            public string? CompanyVenderName { get; set; }
            public string? VenderName { get; set; }
        }

        public class IUDGRPayment
        {
            public Guid? ID { get; set; }
            public Guid? ProjectID { get; set; }
            public Guid? UnitID { get; set; }
            public Guid? UnitFormID { get; set; }
            public string? GRNO { get; set; }
            public string? PONO { get; set; }
            public string? Remark { get; set; }
            public decimal? PercentPayment { get; set; }
            public int? SyncStatusID { get; set; }
            public string? SyncMessage { get; set; }
            public Guid? UserID { get; set; }
        }

        public class getListUnitFormGRPaymentTable
        {
            public Guid? ID { get; set; }
            public Guid? ProjectID { get; set; }
            public string? ProjectName { get; set; }
            public Guid? UnitID { get; set; }
            public string? UnitCode { get; set; }
            public Guid? UnitFormID { get; set; }
            public string? GRNO { get; set; }
            public string? PONO { get; set; }
            public string? Remark { get; set; }
            public decimal? PercentPayment { get; set; }
            public int? SyncStatusID { get; set; }
            public string? SyncStatusName { get; set; }
            public string? SyncMessage { get; set; }
            public string? UpdateDate { get; set; }
            public string? CreateBy { get; set; }
        }

    }
}
