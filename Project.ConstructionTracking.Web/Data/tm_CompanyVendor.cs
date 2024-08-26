﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_CompanyVendor
    {
        public tm_CompanyVendor()
        {
            tr_CompanyVendor = new HashSet<tr_CompanyVendor>();
            tr_CompanyVendorProject = new HashSet<tr_CompanyVendorProject>();
        }

        [Key]
        public int ID { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? Name { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [InverseProperty("CompanyVendor")]
        public virtual ICollection<tr_CompanyVendor> tr_CompanyVendor { get; set; }
        [InverseProperty("CompanyVendor")]
        public virtual ICollection<tr_CompanyVendorProject> tr_CompanyVendorProject { get; set; }
    }
}