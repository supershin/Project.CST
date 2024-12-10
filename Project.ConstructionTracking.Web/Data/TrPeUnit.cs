using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrPeUnit
    {
        public int Id { get; set; }
        public Guid? UnitId { get; set; }
        public Guid? UserId { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmUnit? Unit { get; set; }
        public virtual TmUser? User { get; set; }
    }
}
