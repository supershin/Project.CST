﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Unit
    {
        public tm_Unit()
        {
            tr_PE_Unit = new HashSet<tr_PE_Unit>();
            tr_QC_UnitCheckList = new HashSet<tr_QC_UnitCheckList>();
            tr_UnitForm = new HashSet<tr_UnitForm>();
        }

        [Key]
        public Guid UnitID { get; set; }
        public Guid? ProjectID { get; set; }
        public int? ModelTypeID { get; set; }
        public int? UnitTypeID { get; set; }
        public int? CompanyVendorID { get; set; }
        public int? VendorID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? PONo { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? GRNo { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? UnitCode { get; set; }
        public int? UnitStatusID { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? AddreessNo { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? Build { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? Floor { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? Block { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Area { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TitledeedArea { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? PhaseName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? SubPhaseName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("CompanyVendorID")]
        [InverseProperty("tm_Unit")]
        public virtual tm_CompanyVendor? CompanyVendor { get; set; }
        [ForeignKey("ModelTypeID")]
        [InverseProperty("tm_Unit")]
        public virtual tm_ModelType? ModelType { get; set; }
        [ForeignKey("ProjectID")]
        [InverseProperty("tm_Unit")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("UnitStatusID")]
        [InverseProperty("tm_UnitUnitStatus")]
        public virtual tm_Ext? UnitStatus { get; set; }
        [ForeignKey("UnitTypeID")]
        [InverseProperty("tm_UnitUnitType")]
        public virtual tm_Ext? UnitType { get; set; }
        [ForeignKey("VendorID")]
        [InverseProperty("tm_Unit")]
        public virtual tm_Vendor? Vendor { get; set; }
        [InverseProperty("Unit")]
        public virtual ICollection<tr_PE_Unit> tr_PE_Unit { get; set; }
        [InverseProperty("Unit")]
        public virtual ICollection<tr_QC_UnitCheckList> tr_QC_UnitCheckList { get; set; }
        [InverseProperty("Unit")]
        public virtual ICollection<tr_UnitForm> tr_UnitForm { get; set; }
    }
}
