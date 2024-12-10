using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmUnit
    {
        public TmUnit()
        {
            TrPeUnits = new HashSet<TrPeUnit>();
            TrQcUnitCheckLists = new HashSet<TrQcUnitCheckList>();
            TrUnitFormPayments = new HashSet<TrUnitFormPayment>();
            TrUnitForms = new HashSet<TrUnitForm>();
        }

        public Guid UnitId { get; set; }
        public Guid? ProjectId { get; set; }
        public int? ModelTypeId { get; set; }
        public int? UnitTypeId { get; set; }
        public int? CompanyVendorId { get; set; }
        public int? VendorId { get; set; }
        public string? Pono { get; set; }
        public string? UnitCode { get; set; }
        public int? UnitStatusId { get; set; }
        public string? AddreessNo { get; set; }
        public string? Build { get; set; }
        public string? Floor { get; set; }
        public string? Block { get; set; }
        public decimal? Area { get; set; }
        public decimal? TitledeedArea { get; set; }
        public string? PhaseName { get; set; }
        public string? SubPhaseName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? TransferDueDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmCompanyVendor? CompanyVendor { get; set; }
        public virtual TmModelType? ModelType { get; set; }
        public virtual TmProject? Project { get; set; }
        public virtual TmExt? UnitStatus { get; set; }
        public virtual TmExt? UnitType { get; set; }
        public virtual TmVendor? Vendor { get; set; }
        public virtual ICollection<TrPeUnit> TrPeUnits { get; set; }
        public virtual ICollection<TrQcUnitCheckList> TrQcUnitCheckLists { get; set; }
        public virtual ICollection<TrUnitFormPayment> TrUnitFormPayments { get; set; }
        public virtual ICollection<TrUnitForm> TrUnitForms { get; set; }
    }
}
