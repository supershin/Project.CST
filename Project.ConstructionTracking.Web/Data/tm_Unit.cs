using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Unit
    {
        public tm_Unit()
        {
            tr_UnitForm = new HashSet<tr_UnitForm>();
        }

        [Key]
        public Guid UnitID { get; set; }
        public Guid? ProjectID { get; set; }
        public int? UnitTypeID { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? UnitCode { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? Build { get; set; }
        public int? Floor { get; set; }
        public int? Block { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Area { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("ProjectID")]
        [InverseProperty("tm_Unit")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("UnitTypeID")]
        [InverseProperty("tm_Unit")]
        public virtual tm_Ext? UnitType { get; set; }
        [InverseProperty("Unit")]
        public virtual ICollection<tr_UnitForm> tr_UnitForm { get; set; }
    }
}
