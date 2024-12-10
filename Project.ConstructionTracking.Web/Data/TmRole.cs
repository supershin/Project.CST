using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmRole
    {
        public TmRole()
        {
            TmUsers = new HashSet<TmUser>();
            TrQcUnitCheckListActions = new HashSet<TrQcUnitCheckListAction>();
            TrRoleActionStatus = new HashSet<TrRoleActionStatu>();
            TrUnitFormActionLogs = new HashSet<TrUnitFormActionLog>();
            TrUnitFormActions = new HashSet<TrUnitFormAction>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? LineOrder { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        public virtual ICollection<TmUser> TmUsers { get; set; }
        public virtual ICollection<TrQcUnitCheckListAction> TrQcUnitCheckListActions { get; set; }
        public virtual ICollection<TrRoleActionStatu> TrRoleActionStatus { get; set; }
        public virtual ICollection<TrUnitFormActionLog> TrUnitFormActionLogs { get; set; }
        public virtual ICollection<TrUnitFormAction> TrUnitFormActions { get; set; }
    }
}
