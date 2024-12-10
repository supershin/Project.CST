using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmFormGroup
    {
        public TmFormGroup()
        {
            TmFormPackages = new HashSet<TmFormPackage>();
            TrUnitFormActionLogs = new HashSet<TrUnitFormActionLog>();
            TrUnitFormCheckLists = new HashSet<TrUnitFormCheckList>();
            TrUnitFormPackages = new HashSet<TrUnitFormPackage>();
            TrUnitFormPassConditions = new HashSet<TrUnitFormPassCondition>();
        }

        public int Id { get; set; }
        public int? FormId { get; set; }
        public string? Name { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmForm? Form { get; set; }
        public virtual ICollection<TmFormPackage> TmFormPackages { get; set; }
        public virtual ICollection<TrUnitFormActionLog> TrUnitFormActionLogs { get; set; }
        public virtual ICollection<TrUnitFormCheckList> TrUnitFormCheckLists { get; set; }
        public virtual ICollection<TrUnitFormPackage> TrUnitFormPackages { get; set; }
        public virtual ICollection<TrUnitFormPassCondition> TrUnitFormPassConditions { get; set; }
    }
}
