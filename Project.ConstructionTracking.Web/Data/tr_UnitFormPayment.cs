using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitFormPayment
    {
        [Key]
        public Guid ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public Guid? UnitFormID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? GRNO { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? PONO { get; set; }
        [StringLength(2000)]
        public string? Remark { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? PercentPayment { get; set; }
        public int? SyncStatusID { get; set; }
        [StringLength(500)]
        public string? SyncMessage { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("ProjectID")]
        [InverseProperty("tr_UnitFormPayment")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("SyncStatusID")]
        [InverseProperty("tr_UnitFormPayment")]
        public virtual tm_Ext? SyncStatus { get; set; }
        [ForeignKey("UnitID")]
        [InverseProperty("tr_UnitFormPayment")]
        public virtual tm_Unit? Unit { get; set; }
        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitFormPayment")]
        public virtual tr_UnitForm? UnitForm { get; set; }
    }
}
