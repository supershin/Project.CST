using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_QC_UnitCheckList_Detail
    {
        public tr_QC_UnitCheckList_Detail()
        {
            tr_QC_UnitCheckList_Resource = new HashSet<tr_QC_UnitCheckList_Resource>();
        }

        [Key]
        public int ID { get; set; }
        public Guid? QCUnitCheckListID { get; set; }
        public int? CheckListID { get; set; }
        public int? CheckListDetailID { get; set; }
        public int? StatusID { get; set; }
        [Unicode(false)]
        public string? Remark { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [ForeignKey("CheckListDetailID")]
        [InverseProperty("tr_QC_UnitCheckList_Detail")]
        public virtual tm_QC_CheckListDetail? CheckListDetail { get; set; }
        [ForeignKey("QCUnitCheckListID")]
        [InverseProperty("tr_QC_UnitCheckList_Detail")]
        public virtual tr_QC_UnitCheckList? QCUnitCheckList { get; set; }
        [InverseProperty("QCUnitCheckListDetail")]
        public virtual ICollection<tr_QC_UnitCheckList_Resource> tr_QC_UnitCheckList_Resource { get; set; }
    }
}
