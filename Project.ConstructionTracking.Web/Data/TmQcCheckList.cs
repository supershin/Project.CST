using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmQcCheckList
    {
        public TmQcCheckList()
        {
            TmQcCheckListDetails = new HashSet<TmQcCheckListDetail>();
            TrFormQccheckLists = new HashSet<TrFormQccheckList>();
            TrQcUnitCheckLists = new HashSet<TrQcUnitCheckList>();
        }

        public int Id { get; set; }
        public int? ProjectTypeId { get; set; }
        public int? QctypeId { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmExt? ProjectType { get; set; }
        public virtual TmExt? Qctype { get; set; }
        public virtual ICollection<TmQcCheckListDetail> TmQcCheckListDetails { get; set; }
        public virtual ICollection<TrFormQccheckList> TrFormQccheckLists { get; set; }
        public virtual ICollection<TrQcUnitCheckList> TrQcUnitCheckLists { get; set; }
    }
}
