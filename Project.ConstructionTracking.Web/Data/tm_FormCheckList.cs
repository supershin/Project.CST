using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_FormCheckList
    {
        [Key]
        public int ID { get; set; }
        public int? PackageID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Name { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("PackageID")]
        [InverseProperty("tm_FormCheckList")]
        public virtual tm_FormPackage? Package { get; set; }
    }
}
