using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrQcUnitCheckList
    {
        public TrQcUnitCheckList()
        {
            TrQcUnitCheckListActions = new HashSet<TrQcUnitCheckListAction>();
            TrQcUnitCheckListDetails = new HashSet<TrQcUnitCheckListDetail>();
            TrQcUnitCheckListResources = new HashSet<TrQcUnitCheckListResource>();
        }

        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? UnitId { get; set; }
        public int? CheckListId { get; set; }
        public int? QctypeId { get; set; }
        public int? Seq { get; set; }
        public DateTime? CheckListDate { get; set; }
        public int? QcstatusId { get; set; }
        public Guid? PesignResourceId { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmQcCheckList? CheckList { get; set; }
        public virtual TmResource? PesignResource { get; set; }
        public virtual TmProject? Project { get; set; }
        public virtual TrRoleActionStatu? Qcstatus { get; set; }
        public virtual TmExt? Qctype { get; set; }
        public virtual TmUnit? Unit { get; set; }
        public virtual ICollection<TrQcUnitCheckListAction> TrQcUnitCheckListActions { get; set; }
        public virtual ICollection<TrQcUnitCheckListDetail> TrQcUnitCheckListDetails { get; set; }
        public virtual ICollection<TrQcUnitCheckListResource> TrQcUnitCheckListResources { get; set; }
    }
}
