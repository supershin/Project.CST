using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_UnitType
    {
        public tm_UnitType()
        {
            tr_ProjectForm = new HashSet<tr_ProjectForm>();
        }

        [Key]
        public int ID { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [InverseProperty("UnitType")]
        public virtual ICollection<tr_ProjectForm> tr_ProjectForm { get; set; }
    }
}
