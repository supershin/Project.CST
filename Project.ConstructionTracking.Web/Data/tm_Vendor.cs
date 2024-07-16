using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Vendor
    {
        public tm_Vendor()
        {
            tm_Unit = new HashSet<tm_Unit>();
            tr_UnitForm = new HashSet<tr_UnitForm>();
        }

        [Key]
        public int ID { get; set; }
        [StringLength(200)]
        [Unicode(false)]
        public string? Name { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [InverseProperty("Vendor")]
        public virtual ICollection<tm_Unit> tm_Unit { get; set; }
        [InverseProperty("Vendor")]
        public virtual ICollection<tr_UnitForm> tr_UnitForm { get; set; }
    }
}
