using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_QC_UnitCheckList
    {
        public tr_QC_UnitCheckList()
        {
            tr_QC_UnitCheckList_Action = new HashSet<tr_QC_UnitCheckList_Action>();
            tr_QC_UnitCheckList_Detail = new HashSet<tr_QC_UnitCheckList_Detail>();
            tr_QC_UnitCheckList_Resource = new HashSet<tr_QC_UnitCheckList_Resource>();
        }

        [Key]
        public Guid ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? CheckListID { get; set; }
        public int? QCTypeID { get; set; }
        public int? Seq { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CheckListDate { get; set; }
        public bool? IsNotReadyInspect { get; set; }
        public bool? IsPassCondition { get; set; }
        public int? QCStatusID { get; set; }
        public Guid? QCSignID { get; set; }
        public Guid? QCSignResourceID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("CheckListID")]
        [InverseProperty("tr_QC_UnitCheckList")]
        public virtual tm_QC_CheckList? CheckList { get; set; }
        [ForeignKey("ProjectID")]
        [InverseProperty("tr_QC_UnitCheckList")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("QCSignID")]
        [InverseProperty("tr_QC_UnitCheckList")]
        public virtual tm_User? QCSign { get; set; }
        [ForeignKey("QCSignResourceID")]
        [InverseProperty("tr_QC_UnitCheckList")]
        public virtual tm_Resource? QCSignResource { get; set; }
        [ForeignKey("QCStatusID")]
        [InverseProperty("tr_QC_UnitCheckList")]
        public virtual tr_RoleActionStatus? QCStatus { get; set; }
        [ForeignKey("QCTypeID")]
        [InverseProperty("tr_QC_UnitCheckList")]
        public virtual tm_Ext? QCType { get; set; }
        [ForeignKey("UnitID")]
        [InverseProperty("tr_QC_UnitCheckList")]
        public virtual tm_Unit? Unit { get; set; }
        [InverseProperty("QCUnitCheckList")]
        public virtual ICollection<tr_QC_UnitCheckList_Action> tr_QC_UnitCheckList_Action { get; set; }
        [InverseProperty("QCUnitCheckList")]
        public virtual ICollection<tr_QC_UnitCheckList_Detail> tr_QC_UnitCheckList_Detail { get; set; }
        [InverseProperty("QCUnitCheckList")]
        public virtual ICollection<tr_QC_UnitCheckList_Resource> tr_QC_UnitCheckList_Resource { get; set; }
    }
}
