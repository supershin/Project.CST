using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_DefectAreaType_Mapping
    {
        [Key]
        public int ID { get; set; }
        public int DefectAreaID { get; set; }
        public int DefectTypeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }

        [ForeignKey("DefectAreaID")]
        [InverseProperty("tm_DefectAreaType_Mapping")]
        public virtual tm_DefectArea DefectArea { get; set; } = null!;
        [ForeignKey("DefectTypeID")]
        [InverseProperty("tm_DefectAreaType_Mapping")]
        public virtual tm_DefectType DefectType { get; set; } = null!;
    }
}
