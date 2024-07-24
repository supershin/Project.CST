using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Ext
    {
        public tm_Ext()
        {
            tm_DefectArea = new HashSet<tm_DefectArea>();
            tm_FormType = new HashSet<tm_FormType>();
            tm_Project = new HashSet<tm_Project>();
            tm_QC_CheckListProjectType = new HashSet<tm_QC_CheckList>();
            tm_QC_CheckListQCType = new HashSet<tm_QC_CheckList>();
            tm_UnitUnitStatus = new HashSet<tm_Unit>();
            tm_UnitUnitType = new HashSet<tm_Unit>();
            tm_User = new HashSet<tm_User>();
            tr_QC_UnitCheckList = new HashSet<tr_QC_UnitCheckList>();
            tr_UnitForm = new HashSet<tr_UnitForm>();
            tr_UnitFormCheckList = new HashSet<tr_UnitFormCheckList>();
            tr_UnitFormPassCondition = new HashSet<tr_UnitFormPassCondition>();
            tr_UnitFormResource = new HashSet<tr_UnitFormResource>();
        }

        [Key]
        public int ID { get; set; }
        public int? ExtTypeID { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
        public int? LineOrder { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [ForeignKey("ExtTypeID")]
        [InverseProperty("tm_Ext")]
        public virtual tm_ExtType? ExtType { get; set; }
        [InverseProperty("ProjectType")]
        public virtual ICollection<tm_DefectArea> tm_DefectArea { get; set; }
        [InverseProperty("ProjectType")]
        public virtual ICollection<tm_FormType> tm_FormType { get; set; }
        [InverseProperty("ProjectType")]
        public virtual ICollection<tm_Project> tm_Project { get; set; }
        [InverseProperty("ProjectType")]
        public virtual ICollection<tm_QC_CheckList> tm_QC_CheckListProjectType { get; set; }
        [InverseProperty("QCType")]
        public virtual ICollection<tm_QC_CheckList> tm_QC_CheckListQCType { get; set; }
        [InverseProperty("UnitStatus")]
        public virtual ICollection<tm_Unit> tm_UnitUnitStatus { get; set; }
        [InverseProperty("UnitType")]
        public virtual ICollection<tm_Unit> tm_UnitUnitType { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<tm_User> tm_User { get; set; }
        [InverseProperty("QCType")]
        public virtual ICollection<tr_QC_UnitCheckList> tr_QC_UnitCheckList { get; set; }
        [InverseProperty("Status")]
        public virtual ICollection<tr_UnitForm> tr_UnitForm { get; set; }
        [InverseProperty("Status")]
        public virtual ICollection<tr_UnitFormCheckList> tr_UnitFormCheckList { get; set; }
        [InverseProperty("LockStatus")]
        public virtual ICollection<tr_UnitFormPassCondition> tr_UnitFormPassCondition { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<tr_UnitFormResource> tr_UnitFormResource { get; set; }
    }
}
