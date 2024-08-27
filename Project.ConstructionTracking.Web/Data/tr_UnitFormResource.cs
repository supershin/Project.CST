using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitFormResource
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? FormID { get; set; }
        public int? GroupID { get; set; }
        public int? PassConditionID { get; set; }
        public int? RoleID { get; set; }
        public Guid? ResourceID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("PassConditionID")]
        [InverseProperty("tr_UnitFormResource")]
        public virtual tr_UnitFormPassCondition? PassCondition { get; set; }
        [ForeignKey("ResourceID")]
        [InverseProperty("tr_UnitFormResource")]
        public virtual tm_Resource? Resource { get; set; }
        [ForeignKey("RoleID")]
        [InverseProperty("tr_UnitFormResource")]
        public virtual tm_Ext? Role { get; set; }
        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitFormResource")]
        public virtual tr_UnitForm? UnitForm { get; set; }
    }
}
