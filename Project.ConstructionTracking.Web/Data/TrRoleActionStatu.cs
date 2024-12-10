using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrRoleActionStatu
    {
        public TrRoleActionStatu()
        {
            TrQcUnitCheckListActions = new HashSet<TrQcUnitCheckListAction>();
            TrQcUnitCheckLists = new HashSet<TrQcUnitCheckList>();
            TrUnitFormActionLogs = new HashSet<TrUnitFormActionLog>();
            TrUnitFormActions = new HashSet<TrUnitFormAction>();
        }

        public int Id { get; set; }
        public int? RoleId { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public bool? UseGroup { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CraeteBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmRole? Role { get; set; }
        public virtual ICollection<TrQcUnitCheckListAction> TrQcUnitCheckListActions { get; set; }
        public virtual ICollection<TrQcUnitCheckList> TrQcUnitCheckLists { get; set; }
        public virtual ICollection<TrUnitFormActionLog> TrUnitFormActionLogs { get; set; }
        public virtual ICollection<TrUnitFormAction> TrUnitFormActions { get; set; }
    }
}
