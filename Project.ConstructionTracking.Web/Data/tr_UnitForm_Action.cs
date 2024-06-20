using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitForm_Action
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SaveDate_PE { get; set; }
        public Guid? SaveBy_PE { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SubmitDate_PE { get; set; }
        public Guid? SubmitBy_PE { get; set; }
        [Unicode(false)]
        public string? Remark_PE { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SaveDate_QC { get; set; }
        public Guid? SaveBy_QC { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SubmitDate_QC { get; set; }
        public Guid? SubmitBy_QC { get; set; }
        [Unicode(false)]
        public string? Remark_QC { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SaveDate_PM { get; set; }
        public Guid? SaveBy_PM { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ApproveDate_PM { get; set; }
        public Guid? ApproveBy_PM { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RejectDate_PM { get; set; }
        public Guid? RejectBy_PM { get; set; }
        [Unicode(false)]
        public string? Remark_PM { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CalcelDate { get; set; }
        public Guid? CancelBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }

        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitForm_Action")]
        public virtual tr_UnitForm? UnitForm { get; set; }
    }
}
