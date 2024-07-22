using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_RoleActionStatus
    {
        public tr_RoleActionStatus()
        {
            tr_UnitFormAction = new HashSet<tr_UnitFormAction>();
            tr_UnitFormActionLog = new HashSet<tr_UnitFormActionLog>();
        }

        [Key]
        public int ID { get; set; }
        public int? RoleID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Name { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CraeteBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("RoleID")]
        [InverseProperty("tr_RoleActionStatus")]
        public virtual tm_Role? Role { get; set; }
        [InverseProperty("Status")]
        public virtual ICollection<tr_UnitFormAction> tr_UnitFormAction { get; set; }
        [InverseProperty("Status")]
        public virtual ICollection<tr_UnitFormActionLog> tr_UnitFormActionLog { get; set; }
    }
}
