using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Form
    {
        public tm_Form()
        {
            tm_FormGroup = new HashSet<tm_FormGroup>();
            tr_Form_QCCheckList = new HashSet<tr_Form_QCCheckList>();
            tr_UnitForm = new HashSet<tr_UnitForm>();
            tr_UnitFormCheckList = new HashSet<tr_UnitFormCheckList>();
            tr_UnitFormPackage = new HashSet<tr_UnitFormPackage>();
        }

        [Key]
        public int ID { get; set; }
        public int? FormTypeID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Name { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Progress { get; set; }
        public int? DurationDay { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("FormTypeID")]
        [InverseProperty("tm_Form")]
        public virtual tm_FormType? FormType { get; set; }
        [InverseProperty("Form")]
        public virtual ICollection<tm_FormGroup> tm_FormGroup { get; set; }
        [InverseProperty("Form")]
        public virtual ICollection<tr_Form_QCCheckList> tr_Form_QCCheckList { get; set; }
        [InverseProperty("Form")]
        public virtual ICollection<tr_UnitForm> tr_UnitForm { get; set; }
        [InverseProperty("Form")]
        public virtual ICollection<tr_UnitFormCheckList> tr_UnitFormCheckList { get; set; }
        [InverseProperty("Form")]
        public virtual ICollection<tr_UnitFormPackage> tr_UnitFormPackage { get; set; }
    }
}
