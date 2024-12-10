using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class VwUnitFormAction
    {
        public Guid? ProjectId { get; set; }
        public Guid? UnitId { get; set; }
        public Guid? UnitFormId { get; set; }
        public int? FormId { get; set; }
        public string? Pe { get; set; }
        public string Pm { get; set; } = null!;
        public int? CntPc { get; set; }
        public string? UnitFormActionPmactionType { get; set; }
        public int? UnitFormActionPmstatusId { get; set; }
        public int? LockStatusId { get; set; }
        public string PassConditionStatus { get; set; } = null!;
    }
}
