using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrUnitFormActionLog
    {
        public int Id { get; set; }
        public Guid? UnitFormId { get; set; }
        public int? GroupId { get; set; }
        public int? RoleId { get; set; }
        public int? StatusId { get; set; }
        public string? Remark { get; set; }
        public DateTime? ActionDate { get; set; }
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }

        public virtual TmFormGroup? Group { get; set; }
        public virtual TmRole? Role { get; set; }
        public virtual TrRoleActionStatu? Status { get; set; }
        public virtual TrUnitForm? UnitForm { get; set; }
    }
}
