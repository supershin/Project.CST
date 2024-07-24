using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitFormCheckList
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? FormID { get; set; }
        public int? GroupID { get; set; }
        public int? PackageID { get; set; }
        public int? CheckListID { get; set; }
        public int? StatusID { get; set; }
        [Unicode(false)]
        public string? Remark { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [ForeignKey("CheckListID")]
        [InverseProperty("tr_UnitFormCheckList")]
        public virtual tm_FormCheckList? CheckList { get; set; }
        [ForeignKey("FormID")]
        [InverseProperty("tr_UnitFormCheckList")]
        public virtual tm_Form? Form { get; set; }
        [ForeignKey("GroupID")]
        [InverseProperty("tr_UnitFormCheckList")]
        public virtual tm_FormGroup? Group { get; set; }
        [ForeignKey("PackageID")]
        [InverseProperty("tr_UnitFormCheckList")]
        public virtual tm_FormPackage? Package { get; set; }
        [ForeignKey("StatusID")]
        [InverseProperty("tr_UnitFormCheckList")]
        public virtual tm_Ext? Status { get; set; }
        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitFormCheckList")]
        public virtual tr_UnitForm? UnitForm { get; set; }
    }
}
