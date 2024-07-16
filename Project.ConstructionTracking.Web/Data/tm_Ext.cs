﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Ext
    {
        public tm_Ext()
        {
            tm_Project = new HashSet<tm_Project>();
            tm_Unit = new HashSet<tm_Unit>();
            tm_UserDepartment = new HashSet<tm_User>();
            tm_UserRole = new HashSet<tm_User>();
            tr_UnitFormResource = new HashSet<tr_UnitFormResource>();
        }

        [Key]
        public int ID { get; set; }
        public int? ExtTypeID { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
        public int? LineOrder { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [ForeignKey("ExtTypeID")]
        [InverseProperty("tm_Ext")]
        public virtual tm_ExtType? ExtType { get; set; }
        [InverseProperty("ProjectType")]
        public virtual ICollection<tm_Project> tm_Project { get; set; }
        [InverseProperty("UnitType")]
        public virtual ICollection<tm_Unit> tm_Unit { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<tm_User> tm_UserDepartment { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<tm_User> tm_UserRole { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<tr_UnitFormResource> tr_UnitFormResource { get; set; }
    }
}