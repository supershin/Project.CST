using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrProjectPermission
    {
        public int Id { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmProject? Project { get; set; }
        public virtual TmUser? User { get; set; }
    }
}
