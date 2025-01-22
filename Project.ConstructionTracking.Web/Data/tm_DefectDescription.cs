using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_DefectDescription
    {
        [Key]
        public int ID { get; set; }
        public int DefectTypeID { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string Name { get; set; } = null!;
        public int? LineOrder { get; set; }
        public int? Revise { get; set; }
        public int FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }

        [ForeignKey("DefectTypeID")]
        [InverseProperty("tm_DefectDescription")]
        public virtual tm_DefectType DefectType { get; set; } = null!;
    }
}
