using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmFormCheckList
    {
        public TmFormCheckList()
        {
            TrUnitFormCheckLists = new HashSet<TrUnitFormCheckList>();
        }

        public int Id { get; set; }
        public int? PackageId { get; set; }
        public string? Name { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual ICollection<TrUnitFormCheckList> TrUnitFormCheckLists { get; set; }
    }
}
