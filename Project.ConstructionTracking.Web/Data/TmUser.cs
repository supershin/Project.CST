using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmUser
    {
        public TmUser()
        {
            TrPeUnits = new HashSet<TrPeUnit>();
            TrProjectPermissions = new HashSet<TrProjectPermission>();
        }

        public Guid Id { get; set; }
        public int? Buid { get; set; }
        public int? DepartmentId { get; set; }
        public int? RoleId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public int? PositionId { get; set; }

        public virtual TmBu? Bu { get; set; }
        public virtual TmExt? Department { get; set; }
        public virtual TmRole? Role { get; set; }
        public virtual ICollection<TrPeUnit> TrPeUnits { get; set; }
        public virtual ICollection<TrProjectPermission> TrProjectPermissions { get; set; }
    }
}
