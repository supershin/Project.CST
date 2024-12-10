using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmCompanyVendor
    {
        public TmCompanyVendor()
        {
            TmUnits = new HashSet<TmUnit>();
            TrCompanyVendorProjects = new HashSet<TrCompanyVendorProject>();
            TrCompanyVendors = new HashSet<TrCompanyVendor>();
            TrUnitForms = new HashSet<TrUnitForm>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual ICollection<TmUnit> TmUnits { get; set; }
        public virtual ICollection<TrCompanyVendorProject> TrCompanyVendorProjects { get; set; }
        public virtual ICollection<TrCompanyVendor> TrCompanyVendors { get; set; }
        public virtual ICollection<TrUnitForm> TrUnitForms { get; set; }
    }
}
