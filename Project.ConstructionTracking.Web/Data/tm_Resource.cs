using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Resource
    {
        public tm_Resource()
        {
            tr_QC_UnitCheckList = new HashSet<tr_QC_UnitCheckList>();
            tr_QC_UnitCheckList_Resource = new HashSet<tr_QC_UnitCheckList_Resource>();
            tr_UnitForm = new HashSet<tr_UnitForm>();
            tr_UnitFormResource = new HashSet<tr_UnitFormResource>();
        }

        [Key]
        public Guid ID { get; set; }
        [StringLength(500)]
        public string? FileName { get; set; }
        [StringLength(500)]
        public string? FilePath { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? MimeType { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [InverseProperty("QCSignResource")]
        public virtual ICollection<tr_QC_UnitCheckList> tr_QC_UnitCheckList { get; set; }
        [InverseProperty("Resource")]
        public virtual ICollection<tr_QC_UnitCheckList_Resource> tr_QC_UnitCheckList_Resource { get; set; }
        [InverseProperty("VendorResource")]
        public virtual ICollection<tr_UnitForm> tr_UnitForm { get; set; }
        [InverseProperty("Resource")]
        public virtual ICollection<tr_UnitFormResource> tr_UnitFormResource { get; set; }
    }
}
