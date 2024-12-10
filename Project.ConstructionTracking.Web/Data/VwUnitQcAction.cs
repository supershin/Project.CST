using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class VwUnitQcAction
    {
        public Guid? ProjectId { get; set; }
        public Guid UnitId { get; set; }
        public int? FormId { get; set; }
        public string? Qc1 { get; set; }
        public string? Qc2 { get; set; }
        public string? Qc3 { get; set; }
        public string? Qc4 { get; set; }
        public string? Qc41 { get; set; }
        public string? Qc42 { get; set; }
        public string? Qc5 { get; set; }
    }
}
