using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_PE_Unit
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitID { get; set; }
        public Guid? UserID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("UnitID")]
        [InverseProperty("tr_PE_Unit")]
        public virtual tm_Unit? Unit { get; set; }
        [ForeignKey("UserID")]
        [InverseProperty("tr_PE_Unit")]
        public virtual tm_User? User { get; set; }
    }
}
