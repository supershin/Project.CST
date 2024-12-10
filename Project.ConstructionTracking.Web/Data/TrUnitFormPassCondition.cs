using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrUnitFormPassCondition
    {
        public TrUnitFormPassCondition()
        {
            TrUnitFormResources = new HashSet<TrUnitFormResource>();
        }

        public int Id { get; set; }
        public Guid? UnitFormId { get; set; }
        public int? GroupId { get; set; }
        public int? LockStatusId { get; set; }
        public int? StatusId { get; set; }
        public string? PeRemark { get; set; }
        public string? PmRemark { get; set; }
        public string? PjmRemark { get; set; }
        public string? PeunLockRemark { get; set; }
        public string? PmunLockRemark { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }

        public virtual TmFormGroup? Group { get; set; }
        public virtual TmExt? LockStatus { get; set; }
        public virtual TrUnitForm? UnitForm { get; set; }
        public virtual ICollection<TrUnitFormResource> TrUnitFormResources { get; set; }
    }
}
