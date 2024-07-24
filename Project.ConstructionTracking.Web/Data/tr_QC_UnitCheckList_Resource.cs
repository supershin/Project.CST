using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_QC_UnitCheckList_Resource
    {
        [Key]
        public int ID { get; set; }
        public Guid? QCUnitCheckListID { get; set; }
        public int? QCUnitCheckListDetailID { get; set; }
        public int? DefectID { get; set; }
        public Guid? ResourceID { get; set; }
        public bool? IsDocument { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [ForeignKey("DefectID")]
        [InverseProperty("tr_QC_UnitCheckList_Resource")]
        public virtual tr_QC_UnitCheckList_Defect? Defect { get; set; }
        [ForeignKey("QCUnitCheckListID")]
        [InverseProperty("tr_QC_UnitCheckList_Resource")]
        public virtual tr_QC_UnitCheckList? QCUnitCheckList { get; set; }
        [ForeignKey("QCUnitCheckListDetailID")]
        [InverseProperty("tr_QC_UnitCheckList_Resource")]
        public virtual tr_QC_UnitCheckList_Detail? QCUnitCheckListDetail { get; set; }
        [ForeignKey("ResourceID")]
        [InverseProperty("tr_QC_UnitCheckList_Resource")]
        public virtual tm_Resource? Resource { get; set; }
    }
}
