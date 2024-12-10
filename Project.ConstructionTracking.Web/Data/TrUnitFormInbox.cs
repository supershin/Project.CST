using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrUnitFormInbox
    {
        public int Id { get; set; }
        public Guid UnitFormId { get; set; }
        public int FormId { get; set; }
        public int? RoleId { get; set; }
        public string? TextInbox { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? ActionDate { get; set; }
        public Guid? ActionBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CraeteBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
