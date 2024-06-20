using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_ProjectFormCheckList
    {
        public tr_ProjectFormCheckList()
        {
            tr_UnitForm_Detail = new HashSet<tr_UnitForm_Detail>();
        }

        [Key]
        public int ID { get; set; }
        public int? PackageID { get; set; }
        [StringLength(100)]
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
        [InverseProperty("tr_ProjectFormCheckList")]
        public virtual tr_ProjectFormPackage? Package { get; set; }
        [InverseProperty("CheckList")]
        public virtual ICollection<tr_UnitForm_Detail> tr_UnitForm_Detail { get; set; }
    }
}
