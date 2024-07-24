using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_QC_CheckListDetail
    {
        public tm_QC_CheckListDetail()
        {
            tr_QC_UnitCheckList_Detail = new HashSet<tr_QC_UnitCheckList_Detail>();
        }

        [Key]
        public int ID { get; set; }
        public int? QCCheckListID { get; set; }
        [StringLength(200)]
        [Unicode(false)]
        public string? Name { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("QCCheckListID")]
        [InverseProperty("tm_QC_CheckListDetail")]
        public virtual tm_QC_CheckList? QCCheckList { get; set; }
        [InverseProperty("CheckListDetail")]
        public virtual ICollection<tr_QC_UnitCheckList_Detail> tr_QC_UnitCheckList_Detail { get; set; }
    }
}
