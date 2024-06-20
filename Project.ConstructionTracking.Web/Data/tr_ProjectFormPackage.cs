using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_ProjectFormPackage
    {
        public tr_ProjectFormPackage()
        {
            tr_ProjectFormCheckList = new HashSet<tr_ProjectFormCheckList>();
            tr_UnitForm_Detail = new HashSet<tr_UnitForm_Detail>();
        }

        [Key]
        public int ID { get; set; }
        public int? GroupID { get; set; }
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

        [ForeignKey("GroupID")]
        [InverseProperty("tr_ProjectFormPackage")]
        public virtual tr_ProjectFormGroup? Group { get; set; }
        [InverseProperty("Package")]
        public virtual ICollection<tr_ProjectFormCheckList> tr_ProjectFormCheckList { get; set; }
        [InverseProperty("Package")]
        public virtual ICollection<tr_UnitForm_Detail> tr_UnitForm_Detail { get; set; }
    }
}
