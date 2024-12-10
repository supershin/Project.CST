using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmFormPackage
    {
        public TmFormPackage()
        {
            TrUnitFormCheckLists = new HashSet<TrUnitFormCheckList>();
            TrUnitFormPackages = new HashSet<TrUnitFormPackage>();
        }

        public int Id { get; set; }
        public int? GroupId { get; set; }
        public string? Name { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmFormGroup? Group { get; set; }
        public virtual ICollection<TrUnitFormCheckList> TrUnitFormCheckLists { get; set; }
        public virtual ICollection<TrUnitFormPackage> TrUnitFormPackages { get; set; }
    }
}
