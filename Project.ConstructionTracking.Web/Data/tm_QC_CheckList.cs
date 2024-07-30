using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_QC_CheckList
    {
        public tm_QC_CheckList()
        {
            tm_QC_CheckListDetail = new HashSet<tm_QC_CheckListDetail>();
            tr_Form_QCCheckList = new HashSet<tr_Form_QCCheckList>();
            tr_QC_UnitCheckList = new HashSet<tr_QC_UnitCheckList>();
        }

        [Key]
        public int ID { get; set; }
        public int? ProjectTypeID { get; set; }
        public int? QCTypeID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("ProjectTypeID")]
        [InverseProperty("tm_QC_CheckListProjectType")]
        public virtual tm_Ext? ProjectType { get; set; }
        [ForeignKey("QCTypeID")]
        [InverseProperty("tm_QC_CheckListQCType")]
        public virtual tm_Ext? QCType { get; set; }
        [InverseProperty("QCCheckList")]
        public virtual ICollection<tm_QC_CheckListDetail> tm_QC_CheckListDetail { get; set; }
        [InverseProperty("CheckList")]
        public virtual ICollection<tr_Form_QCCheckList> tr_Form_QCCheckList { get; set; }
        [InverseProperty("CheckList")]
        public virtual ICollection<tr_QC_UnitCheckList> tr_QC_UnitCheckList { get; set; }
    }
}
