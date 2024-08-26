using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitFormPackage
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? FormID { get; set; }
        public int? GroupID { get; set; }
        public int? PackageID { get; set; }
        [Unicode(false)]
        public string? Remark { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("FormID")]
        [InverseProperty("tr_UnitFormPackage")]
        public virtual tm_Form? Form { get; set; }
        [ForeignKey("GroupID")]
        [InverseProperty("tr_UnitFormPackage")]
        public virtual tm_FormGroup? Group { get; set; }
        [ForeignKey("PackageID")]
        [InverseProperty("tr_UnitFormPackage")]
        public virtual tm_FormPackage? Package { get; set; }
        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitFormPackage")]
        public virtual tr_UnitForm? UnitForm { get; set; }
    }
}
