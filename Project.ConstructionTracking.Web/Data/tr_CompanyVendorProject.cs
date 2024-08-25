using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_CompanyVendorProject
    {
        [Key]
        public int ID { get; set; }
        public int? CompanyVendorID { get; set; }
        public Guid? ProjectID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("CompanyVendorID")]
        [InverseProperty("tr_CompanyVendorProject")]
        public virtual tm_CompanyVendor? CompanyVendor { get; set; }
        [ForeignKey("ProjectID")]
        [InverseProperty("tr_CompanyVendorProject")]
        public virtual tm_Project? Project { get; set; }
    }
}
