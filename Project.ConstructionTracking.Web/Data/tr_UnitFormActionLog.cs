using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitFormActionLog
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? GroupID { get; set; }
        public int? RoleID { get; set; }
        public int? StatusID { get; set; }
        public int? Remark { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActionDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }

        [ForeignKey("RoleID")]
        [InverseProperty("tr_UnitFormActionLog")]
        public virtual tm_Role? Role { get; set; }
        [ForeignKey("StatusID")]
        [InverseProperty("tr_UnitFormActionLog")]
        public virtual tr_RoleActionStatus? Status { get; set; }
        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitFormActionLog")]
        public virtual tr_UnitForm? UnitForm { get; set; }
    }
}
