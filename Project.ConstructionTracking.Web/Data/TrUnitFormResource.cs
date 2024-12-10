using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrUnitFormResource
    {
        public int Id { get; set; }
        public Guid? UnitFormId { get; set; }
        public int? FormId { get; set; }
        public int? GroupId { get; set; }
        public int? PassConditionId { get; set; }
        public int? RoleId { get; set; }
        public Guid? ResourceId { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TrUnitFormPassCondition? PassCondition { get; set; }
        public virtual TmResource? Resource { get; set; }
        public virtual TmExt? Role { get; set; }
        public virtual TrUnitForm? UnitForm { get; set; }
    }
}
