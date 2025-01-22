using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    [Keyless]
    public partial class tr_UnitFormUnLockPassCondition
    {
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? PassConditionID { get; set; }
        public int? RoleID { get; set; }
        public int? StatusID { get; set; }
        [StringLength(500)]
        public string? Remark { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActionDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }
    }
}
