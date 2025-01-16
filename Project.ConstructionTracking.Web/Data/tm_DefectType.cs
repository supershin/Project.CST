using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_DefectType
    {
        public tm_DefectType()
        {
            tm_DefectAreaType_Mapping = new HashSet<tm_DefectAreaType_Mapping>();
            tm_DefectDescription = new HashSet<tm_DefectDescription>();
        }

        [Key]
        public int ID { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string Name { get; set; } = null!;
        public int? LineOrder { get; set; }
        public int? Revise { get; set; }
        public int IsProduct { get; set; }
        public int FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }

        [InverseProperty("DefectType")]
        public virtual ICollection<tm_DefectAreaType_Mapping> tm_DefectAreaType_Mapping { get; set; }
        [InverseProperty("DefectType")]
        public virtual ICollection<tm_DefectDescription> tm_DefectDescription { get; set; }
    }
}
