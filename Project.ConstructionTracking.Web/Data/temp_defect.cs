using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    [Keyless]
    public partial class temp_defect
    {
        [StringLength(255)]
        public string? areaneme { get; set; }
        [StringLength(255)]
        public string? typename { get; set; }
        [StringLength(255)]
        public string? descname { get; set; }
    }
}
