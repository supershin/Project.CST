﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_BU
    {
        public tm_BU()
        {
            tm_Project = new HashSet<tm_Project>();
        }

        [Key]
        public int ID { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [InverseProperty("BU")]
        public virtual ICollection<tm_Project> tm_Project { get; set; }
    }
}
