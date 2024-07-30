using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitFormAction
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? PassConditionID { get; set; }
        public int? RoleID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? ActionType { get; set; }
        public int? StatusID { get; set; }
        [Unicode(false)]
        public string? Remark { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActionDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }

        [ForeignKey("PassConditionID")]
        [InverseProperty("tr_UnitFormAction")]
        public virtual tr_UnitFormPassCondition? PassCondition { get; set; }
        [ForeignKey("RoleID")]
        [InverseProperty("tr_UnitFormAction")]
        public virtual tm_Role? Role { get; set; }
        [ForeignKey("StatusID")]
        [InverseProperty("tr_UnitFormAction")]
        public virtual tr_RoleActionStatus? Status { get; set; }
        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitFormAction")]
        public virtual tr_UnitForm? UnitForm { get; set; }
    }
}
