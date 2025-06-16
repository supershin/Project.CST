using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_QC_Sync
    {
        [Key]
        public Guid ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? QCTypeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? QCAppointDate { get; set; }
        [StringLength(5)]
        [Unicode(false)]
        public string? QCAppointTimeFrom { get; set; }
        [StringLength(5)]
        [Unicode(false)]
        public string? QCAppointTimeTo { get; set; }
        public Guid? QCResponseUserID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? QCResponseDate { get; set; }
        public string? QCRemark { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SubmitDate { get; set; }

        [ForeignKey("ProjectID")]
        [InverseProperty("tr_QC_Sync")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("QCTypeID")]
        [InverseProperty("tr_QC_Sync")]
        public virtual tm_Ext? QCType { get; set; }
        [ForeignKey("UnitID")]
        [InverseProperty("tr_QC_Sync")]
        public virtual tm_Unit? Unit { get; set; }
    }
}
