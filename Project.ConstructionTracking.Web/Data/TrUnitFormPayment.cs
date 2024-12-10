using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrUnitFormPayment
    {
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? UnitId { get; set; }
        public Guid? UnitFormId { get; set; }
        public string? Grno { get; set; }
        public string? Pono { get; set; }
        public string? Remark { get; set; }
        public decimal? PercentPayment { get; set; }
        public int? SyncStatusId { get; set; }
        public string? SyncMessage { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmProject? Project { get; set; }
        public virtual TmExt? SyncStatus { get; set; }
        public virtual TmUnit? Unit { get; set; }
        public virtual TrUnitForm? UnitForm { get; set; }
    }
}
