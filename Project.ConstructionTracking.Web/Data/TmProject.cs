using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmProject
    {
        public TmProject()
        {
            TmModelTypes = new HashSet<TmModelType>();
            TmUnits = new HashSet<TmUnit>();
            TrCompanyVendorProjects = new HashSet<TrCompanyVendorProject>();
            TrProjectPermissions = new HashSet<TrProjectPermission>();
            TrQcUnitCheckLists = new HashSet<TrQcUnitCheckList>();
            TrUnitFormPayments = new HashSet<TrUnitFormPayment>();
            TrUnitForms = new HashSet<TrUnitForm>();
        }

        public Guid ProjectId { get; set; }
        public int? Buid { get; set; }
        public int? ProjectTypeId { get; set; }
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmBu? Bu { get; set; }
        public virtual TmExt? ProjectType { get; set; }
        public virtual ICollection<TmModelType> TmModelTypes { get; set; }
        public virtual ICollection<TmUnit> TmUnits { get; set; }
        public virtual ICollection<TrCompanyVendorProject> TrCompanyVendorProjects { get; set; }
        public virtual ICollection<TrProjectPermission> TrProjectPermissions { get; set; }
        public virtual ICollection<TrQcUnitCheckList> TrQcUnitCheckLists { get; set; }
        public virtual ICollection<TrUnitFormPayment> TrUnitFormPayments { get; set; }
        public virtual ICollection<TrUnitForm> TrUnitForms { get; set; }
    }
}
