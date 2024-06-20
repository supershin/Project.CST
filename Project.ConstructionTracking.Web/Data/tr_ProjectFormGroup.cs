using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_ProjectFormGroup
    {
        public tr_ProjectFormGroup()
        {
            tr_ProjectFormPackage = new HashSet<tr_ProjectFormPackage>();
            tr_UnitForm_Detail = new HashSet<tr_UnitForm_Detail>();
        }

        [Key]
        public int ID { get; set; }
        public int? FormID { get; set; }
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

        [ForeignKey("FormID")]
        [InverseProperty("tr_ProjectFormGroup")]
        public virtual tr_ProjectForm? Form { get; set; }
        [InverseProperty("Group")]
        public virtual ICollection<tr_ProjectFormPackage> tr_ProjectFormPackage { get; set; }
        [InverseProperty("Group")]
        public virtual ICollection<tr_UnitForm_Detail> tr_UnitForm_Detail { get; set; }
    }
}
