using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitForm_Detail
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? FormID { get; set; }
        public int? GroupID { get; set; }
        public int? PackageID { get; set; }
        public int? CheckListID { get; set; }
        public int? CheckListStatusID { get; set; }
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
        [InverseProperty("tr_UnitForm_Detail")]
        public virtual tr_ProjectFormCheckList? CheckList { get; set; }
        [ForeignKey("FormID")]
        [InverseProperty("tr_UnitForm_Detail")]
        public virtual tr_ProjectForm? Form { get; set; }
        [ForeignKey("GroupID")]
        [InverseProperty("tr_UnitForm_Detail")]
        public virtual tr_ProjectFormGroup? Group { get; set; }
        [ForeignKey("PackageID")]
        [InverseProperty("tr_UnitForm_Detail")]
        public virtual tr_ProjectFormPackage? Package { get; set; }
        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitForm_Detail")]
        public virtual tr_UnitForm? UnitForm { get; set; }
    }
}
