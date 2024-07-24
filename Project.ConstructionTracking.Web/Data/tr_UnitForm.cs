using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitForm
    {
        public tr_UnitForm()
        {
            tr_UnitFormAction = new HashSet<tr_UnitFormAction>();
            tr_UnitFormActionLog = new HashSet<tr_UnitFormActionLog>();
            tr_UnitFormCheckList = new HashSet<tr_UnitFormCheckList>();
            tr_UnitFormPackage = new HashSet<tr_UnitFormPackage>();
            tr_UnitFormPassCondition = new HashSet<tr_UnitFormPassCondition>();
            tr_UnitFormResource = new HashSet<tr_UnitFormResource>();
        }

        [Key]
        public Guid ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? VendorID { get; set; }
        public Guid? VendorResourceID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? PONo { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string? Grade { get; set; }
        public int? FormID { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Progress { get; set; }
        public int? Duration { get; set; }
        public int? StatusID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [ForeignKey("FormID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tm_Form? Form { get; set; }
        [ForeignKey("ProjectID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("StatusID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tm_Ext? Status { get; set; }
        [ForeignKey("StatusID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tm_UnitFormStatus? StatusNavigation { get; set; }
        [ForeignKey("UnitID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tm_Unit? Unit { get; set; }
        [ForeignKey("VendorID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tm_Vendor? Vendor { get; set; }
        [ForeignKey("VendorResourceID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tm_Resource? VendorResource { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitFormAction> tr_UnitFormAction { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitFormActionLog> tr_UnitFormActionLog { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitFormCheckList> tr_UnitFormCheckList { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitFormPackage> tr_UnitFormPackage { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitFormPassCondition> tr_UnitFormPassCondition { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitFormResource> tr_UnitFormResource { get; set; }
    }
}
