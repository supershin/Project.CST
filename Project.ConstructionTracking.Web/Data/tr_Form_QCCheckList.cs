﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_Form_QCCheckList
    {
        [Key]
        public int ID { get; set; }
        public int? FormID { get; set; }
        public int? CheckListID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("CheckListID")]
        [InverseProperty("tr_Form_QCCheckList")]
        public virtual tm_QC_CheckList? CheckList { get; set; }
        [ForeignKey("FormID")]
        [InverseProperty("tr_Form_QCCheckList")]
        public virtual tm_Form? Form { get; set; }
    }
}
