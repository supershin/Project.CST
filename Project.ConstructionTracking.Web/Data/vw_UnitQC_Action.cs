using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    [Keyless]
    public partial class vw_UnitQC_Action
    {
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? FormID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? QC1 { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? QC2 { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? QC3 { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? QC4_1 { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? QC4_2 { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? QC5 { get; set; }
    }
}
