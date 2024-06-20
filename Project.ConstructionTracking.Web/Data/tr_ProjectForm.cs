using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_ProjectForm
    {
        public tr_ProjectForm()
        {
            tr_ProjectFormGroup = new HashSet<tr_ProjectFormGroup>();
            tr_UnitForm = new HashSet<tr_UnitForm>();
            tr_UnitForm_Detail = new HashSet<tr_UnitForm_Detail>();
        }

        [Key]
        public int ID { get; set; }
        public Guid? ProjectID { get; set; }
        public int? UnitTypeID { get; set; }
        public int? FormTypeID { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? Name { get; set; }
        public int? FormRefID { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Progress { get; set; }
        public int? Duration { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("FormTypeID")]
        [InverseProperty("tr_ProjectForm")]
        public virtual tm_Ext? FormType { get; set; }
        [ForeignKey("ProjectID")]
        [InverseProperty("tr_ProjectForm")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("UnitTypeID")]
        [InverseProperty("tr_ProjectForm")]
        public virtual tm_UnitType? UnitType { get; set; }
        [InverseProperty("Form")]
        public virtual ICollection<tr_ProjectFormGroup> tr_ProjectFormGroup { get; set; }
        [InverseProperty("Form")]
        public virtual ICollection<tr_UnitForm> tr_UnitForm { get; set; }
        [InverseProperty("Form")]
        public virtual ICollection<tr_UnitForm_Detail> tr_UnitForm_Detail { get; set; }
    }
}
