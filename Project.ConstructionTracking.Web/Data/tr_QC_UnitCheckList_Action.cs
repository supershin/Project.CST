using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_QC_UnitCheckList_Action
    {
        [Key]
        public int ID { get; set; }
        public Guid? QCUnitCheckListID { get; set; }
        public int? RoleID { get; set; }
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

        [ForeignKey("QCUnitCheckListID")]
        [InverseProperty("tr_QC_UnitCheckList_Action")]
        public virtual tr_QC_UnitCheckList? QCUnitCheckList { get; set; }
        [ForeignKey("RoleID")]
        [InverseProperty("tr_QC_UnitCheckList_Action")]
        public virtual tm_Role? Role { get; set; }
        [ForeignKey("StatusID")]
        [InverseProperty("tr_QC_UnitCheckList_Action")]
        public virtual tr_RoleActionStatus? Status { get; set; }
    }
}
