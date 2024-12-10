using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmExt
    {
        public TmExt()
        {
            TmDefectAreas = new HashSet<TmDefectArea>();
            TmFormTypes = new HashSet<TmFormType>();
            TmProjects = new HashSet<TmProject>();
            TmQcCheckListProjectTypes = new HashSet<TmQcCheckList>();
            TmQcCheckListQctypes = new HashSet<TmQcCheckList>();
            TmUnitUnitStatus = new HashSet<TmUnit>();
            TmUnitUnitTypes = new HashSet<TmUnit>();
            TmUsers = new HashSet<TmUser>();
            TrQcUnitCheckLists = new HashSet<TrQcUnitCheckList>();
            TrUnitFormCheckLists = new HashSet<TrUnitFormCheckList>();
            TrUnitFormPassConditions = new HashSet<TrUnitFormPassCondition>();
            TrUnitFormPayments = new HashSet<TrUnitFormPayment>();
            TrUnitFormResources = new HashSet<TrUnitFormResource>();
            TrUnitForms = new HashSet<TrUnitForm>();
        }

        public int Id { get; set; }
        public int? ExtTypeId { get; set; }
        public string? Name { get; set; }
        public string? OtherVal { get; set; }
        public int? LineOrder { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        public virtual TmExtType? ExtType { get; set; }
        public virtual ICollection<TmDefectArea> TmDefectAreas { get; set; }
        public virtual ICollection<TmFormType> TmFormTypes { get; set; }
        public virtual ICollection<TmProject> TmProjects { get; set; }
        public virtual ICollection<TmQcCheckList> TmQcCheckListProjectTypes { get; set; }
        public virtual ICollection<TmQcCheckList> TmQcCheckListQctypes { get; set; }
        public virtual ICollection<TmUnit> TmUnitUnitStatus { get; set; }
        public virtual ICollection<TmUnit> TmUnitUnitTypes { get; set; }
        public virtual ICollection<TmUser> TmUsers { get; set; }
        public virtual ICollection<TrQcUnitCheckList> TrQcUnitCheckLists { get; set; }
        public virtual ICollection<TrUnitFormCheckList> TrUnitFormCheckLists { get; set; }
        public virtual ICollection<TrUnitFormPassCondition> TrUnitFormPassConditions { get; set; }
        public virtual ICollection<TrUnitFormPayment> TrUnitFormPayments { get; set; }
        public virtual ICollection<TrUnitFormResource> TrUnitFormResources { get; set; }
        public virtual ICollection<TrUnitForm> TrUnitForms { get; set; }
    }
}
