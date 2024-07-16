using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Project
    {
        public tm_Project()
        {
            tm_ModelType = new HashSet<tm_ModelType>();
            tm_Unit = new HashSet<tm_Unit>();
            tr_UnitForm = new HashSet<tr_UnitForm>();
        }

        [Key]
        public Guid ProjectID { get; set; }
        public int? BUID { get; set; }
        public int? ProjectTypeID { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? ProjectCode { get; set; }
        [StringLength(500)]
        public string? ProjectName { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [ForeignKey("BUID")]
        [InverseProperty("tm_Project")]
        public virtual tm_BU? BU { get; set; }
        [ForeignKey("ProjectTypeID")]
        [InverseProperty("tm_Project")]
        public virtual tm_Ext? ProjectType { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<tm_ModelType> tm_ModelType { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<tm_Unit> tm_Unit { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<tr_UnitForm> tr_UnitForm { get; set; }
    }
}
