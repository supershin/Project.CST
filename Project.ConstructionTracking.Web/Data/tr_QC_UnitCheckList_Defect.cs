using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_QC_UnitCheckList_Defect
    {
        public tr_QC_UnitCheckList_Defect()
        {
            tr_QC_UnitCheckList_Resource = new HashSet<tr_QC_UnitCheckList_Resource>();
        }

        [Key]
        public int ID { get; set; }
        public Guid? QCUnitCheckListID { get; set; }
        public int? Seq { get; set; }
        public int? DefectAreaID { get; set; }
        public int? DefectTypeID { get; set; }
        public int? DefectDescriptionID { get; set; }
        public int? StatusID { get; set; }
        [Unicode(false)]
        public string? Remark { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [InverseProperty("Defect")]
        public virtual ICollection<tr_QC_UnitCheckList_Resource> tr_QC_UnitCheckList_Resource { get; set; }
    }
}
