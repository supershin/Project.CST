using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Role
    {
        public tm_Role()
        {
            tm_User = new HashSet<tm_User>();
            tr_QC_UnitCheckList_Action = new HashSet<tr_QC_UnitCheckList_Action>();
            tr_RoleActionStatus = new HashSet<tr_RoleActionStatus>();
            tr_UnitFormAction = new HashSet<tr_UnitFormAction>();
            tr_UnitFormActionLog = new HashSet<tr_UnitFormActionLog>();
        }

        [Key]
        public int ID { get; set; }
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

        [InverseProperty("Role")]
        public virtual ICollection<tm_User> tm_User { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<tr_QC_UnitCheckList_Action> tr_QC_UnitCheckList_Action { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<tr_RoleActionStatus> tr_RoleActionStatus { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<tr_UnitFormAction> tr_UnitFormAction { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<tr_UnitFormActionLog> tr_UnitFormActionLog { get; set; }
    }
}
