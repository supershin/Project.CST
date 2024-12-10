using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrUnitForm
    {
        public TrUnitForm()
        {
            TrUnitFormActionLogs = new HashSet<TrUnitFormActionLog>();
            TrUnitFormActions = new HashSet<TrUnitFormAction>();
            TrUnitFormCheckLists = new HashSet<TrUnitFormCheckList>();
            TrUnitFormPackages = new HashSet<TrUnitFormPackage>();
            TrUnitFormPassConditions = new HashSet<TrUnitFormPassCondition>();
            TrUnitFormPayments = new HashSet<TrUnitFormPayment>();
            TrUnitFormResources = new HashSet<TrUnitFormResource>();
        }

        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? UnitId { get; set; }
        public int? CompanyVendorId { get; set; }
        public int? VendorId { get; set; }
        public Guid? VendorResourceId { get; set; }
        public string? Grno { get; set; }
        public string? Pono { get; set; }
        public string? Grade { get; set; }
        public int? FormId { get; set; }
        public decimal? Progress { get; set; }
        public int? Duration { get; set; }
        public int? StatusId { get; set; }
        public DateTime? Grdate { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmCompanyVendor? CompanyVendor { get; set; }
        public virtual TmForm? Form { get; set; }
        public virtual TmProject? Project { get; set; }
        public virtual TmExt? Status { get; set; }
        public virtual TmUnitFormStatu? StatusNavigation { get; set; }
        public virtual TmUnit? Unit { get; set; }
        public virtual TmVendor? Vendor { get; set; }
        public virtual TmResource? VendorResource { get; set; }
        public virtual ICollection<TrUnitFormActionLog> TrUnitFormActionLogs { get; set; }
        public virtual ICollection<TrUnitFormAction> TrUnitFormActions { get; set; }
        public virtual ICollection<TrUnitFormCheckList> TrUnitFormCheckLists { get; set; }
        public virtual ICollection<TrUnitFormPackage> TrUnitFormPackages { get; set; }
        public virtual ICollection<TrUnitFormPassCondition> TrUnitFormPassConditions { get; set; }
        public virtual ICollection<TrUnitFormPayment> TrUnitFormPayments { get; set; }
        public virtual ICollection<TrUnitFormResource> TrUnitFormResources { get; set; }
    }
}
