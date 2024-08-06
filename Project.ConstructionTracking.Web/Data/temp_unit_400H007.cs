using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    [Keyless]
    public partial class temp_unit_400H007
    {
        [StringLength(255)]
        public string? ProjectID { get; set; }
        [StringLength(255)]
        public string? UnitNumber { get; set; }
        [StringLength(255)]
        public string? AddressNo { get; set; }
        [StringLength(255)]
        public string? TowerID { get; set; }
        [StringLength(255)]
        public string? TowerName { get; set; }
        [StringLength(255)]
        public string? FloorID { get; set; }
        [StringLength(255)]
        public string? FloorName { get; set; }
        [StringLength(255)]
        public string? Block { get; set; }
        public double? SellingArea { get; set; }
        public double? TitledeedArea { get; set; }
        public double? UnitStatus { get; set; }
        [StringLength(255)]
        public string? ModelID { get; set; }
        [StringLength(255)]
        public string? ModelName { get; set; }
        public double? ModelArea { get; set; }
        [StringLength(255)]
        public string? ModelTypeID { get; set; }
        [StringLength(255)]
        public string? ModelTypeName { get; set; }
        public double? PhaseID { get; set; }
        [StringLength(255)]
        public string? PhaseName { get; set; }
        public double? SubPhaseID { get; set; }
        [StringLength(255)]
        public string? SubphaseName { get; set; }
    }
}
