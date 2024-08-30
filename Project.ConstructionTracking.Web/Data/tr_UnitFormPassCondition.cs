using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitFormPassCondition
    {
        public tr_UnitFormPassCondition()
        {
            tr_UnitFormResource = new HashSet<tr_UnitFormResource>();
        }

        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? GroupID { get; set; }
        public int? LockStatusID { get; set; }
        public int? StatusID { get; set; }
        public string? PE_Remark { get; set; }
        public string? PM_Remark { get; set; }
        public string? PJM_Remark { get; set; }
        public string? PEUnLock_Remark { get; set; }
        public string? PMUnLock_Remark { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActionDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }

        [ForeignKey("GroupID")]
        [InverseProperty("tr_UnitFormPassCondition")]
        public virtual tm_FormGroup? Group { get; set; }
        [ForeignKey("LockStatusID")]
        [InverseProperty("tr_UnitFormPassCondition")]
        public virtual tm_Ext? LockStatus { get; set; }
        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitFormPassCondition")]
        public virtual tr_UnitForm? UnitForm { get; set; }
        [InverseProperty("PassCondition")]
        public virtual ICollection<tr_UnitFormResource> tr_UnitFormResource { get; set; }
    }
}
