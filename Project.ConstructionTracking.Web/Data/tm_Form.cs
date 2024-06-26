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
        }

        [Key]
        public int ID { get; set; }
        public int? ProjectTypeID { get; set; }
        public int? UnitTypeID { get; set; }
        public int? FormTypeID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Name { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? Description { get; set; }
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
        [InverseProperty("tm_FormFormType")]
        public virtual tm_Ext? FormType { get; set; }
        [ForeignKey("ProjectTypeID")]
        [InverseProperty("tm_FormProjectType")]
        public virtual tm_Ext? ProjectType { get; set; }
        [ForeignKey("UnitTypeID")]
        [InverseProperty("tm_FormUnitType")]
        public virtual tm_Ext? UnitType { get; set; }
        [InverseProperty("Form")]
        public virtual ICollection<tm_FormGroup> tm_FormGroup { get; set; }
    }
}
