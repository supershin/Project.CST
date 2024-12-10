using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmVendor
    {
        public TmVendor()
        {
            TmUnits = new HashSet<TmUnit>();
            TrCompanyVendors = new HashSet<TrCompanyVendor>();
            TrUnitForms = new HashSet<TrUnitForm>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual ICollection<TmUnit> TmUnits { get; set; }
        public virtual ICollection<TrCompanyVendor> TrCompanyVendors { get; set; }
        public virtual ICollection<TrUnitForm> TrUnitForms { get; set; }
    }
}
