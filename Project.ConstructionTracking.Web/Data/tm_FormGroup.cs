using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_FormGroup
    {
        public tm_FormGroup()
        {
            tm_FormPackage = new HashSet<tm_FormPackage>();
            tr_UnitFormActionLog = new HashSet<tr_UnitFormActionLog>();
            tr_UnitFormCheckList = new HashSet<tr_UnitFormCheckList>();
            tr_UnitFormPackage = new HashSet<tr_UnitFormPackage>();
            tr_UnitFormPassCondition = new HashSet<tr_UnitFormPassCondition>();
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
        [InverseProperty("tm_FormGroup")]
        public virtual tm_Form? Form { get; set; }
        [InverseProperty("Group")]
        public virtual ICollection<tm_FormPackage> tm_FormPackage { get; set; }
        [InverseProperty("Group")]
        public virtual ICollection<tr_UnitFormActionLog> tr_UnitFormActionLog { get; set; }
        [InverseProperty("Group")]
        public virtual ICollection<tr_UnitFormCheckList> tr_UnitFormCheckList { get; set; }
        [InverseProperty("Group")]
        public virtual ICollection<tr_UnitFormPackage> tr_UnitFormPackage { get; set; }
        [InverseProperty("Group")]
        public virtual ICollection<tr_UnitFormPassCondition> tr_UnitFormPassCondition { get; set; }
    }
}
