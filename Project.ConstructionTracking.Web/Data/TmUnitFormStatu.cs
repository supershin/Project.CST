using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmUnitFormStatu
    {
        public TmUnitFormStatu()
        {
            TrUnitForms = new HashSet<TrUnitForm>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? LineOrder { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        public virtual ICollection<TrUnitForm> TrUnitForms { get; set; }
    }
}
