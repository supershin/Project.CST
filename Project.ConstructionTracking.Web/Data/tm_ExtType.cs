using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_ExtType
    {
        public tm_ExtType()
        {
            tm_Ext = new HashSet<tm_Ext>();
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

        [InverseProperty("ExtType")]
        public virtual ICollection<tm_Ext> tm_Ext { get; set; }
    }
}
