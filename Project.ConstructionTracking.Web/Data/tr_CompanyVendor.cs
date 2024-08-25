using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_CompanyVendor
    {
        [Key]
        public int ID { get; set; }
        public int? CompanyVendorID { get; set; }
        public int? VendorID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("CompanyVendorID")]
        [InverseProperty("tr_CompanyVendor")]
        public virtual tm_CompanyVendor? CompanyVendor { get; set; }
        [ForeignKey("VendorID")]
        [InverseProperty("tr_CompanyVendor")]
        public virtual tm_Vendor? Vendor { get; set; }
    }
}
