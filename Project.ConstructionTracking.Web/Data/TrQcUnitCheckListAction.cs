using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrQcUnitCheckListAction
    {
        public int Id { get; set; }
        public Guid? QcunitCheckListId { get; set; }
        public int? RoleId { get; set; }
        public string? ActionType { get; set; }
        public int? StatusId { get; set; }
        public string? Remark { get; set; }
        public DateTime? ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }

        public virtual TrQcUnitCheckList? QcunitCheckList { get; set; }
        public virtual TmRole? Role { get; set; }
        public virtual TrRoleActionStatu? Status { get; set; }
    }
}
