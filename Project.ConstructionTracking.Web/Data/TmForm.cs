using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmForm
    {
        public TmForm()
        {
            TmFormGroups = new HashSet<TmFormGroup>();
            TrFormQccheckLists = new HashSet<TrFormQccheckList>();
            TrUnitFormCheckLists = new HashSet<TrUnitFormCheckList>();
            TrUnitFormPackages = new HashSet<TrUnitFormPackage>();
            TrUnitForms = new HashSet<TrUnitForm>();
        }

        public int Id { get; set; }
        public int? FormTypeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Progress { get; set; }
        public int? DurationDay { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmFormType? FormType { get; set; }
        public virtual ICollection<TmFormGroup> TmFormGroups { get; set; }
        public virtual ICollection<TrFormQccheckList> TrFormQccheckLists { get; set; }
        public virtual ICollection<TrUnitFormCheckList> TrUnitFormCheckLists { get; set; }
        public virtual ICollection<TrUnitFormPackage> TrUnitFormPackages { get; set; }
        public virtual ICollection<TrUnitForm> TrUnitForms { get; set; }
    }
}
