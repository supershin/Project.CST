using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_DefectArea
    {
        public tm_DefectArea()
        {
            tm_DefectAreaType_Mapping = new HashSet<tm_DefectAreaType_Mapping>();
        }

        [Key]
        public int ID { get; set; }
        public int? ProjectTypeID { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? Name { get; set; }
        public int? LineOrder { get; set; }
        public int? Revise { get; set; }
        [Required]
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }
        public int? IsContactCenter { get; set; }

        [ForeignKey("ProjectTypeID")]
        [InverseProperty("tm_DefectArea")]
        public virtual tm_Ext? ProjectType { get; set; }
        [InverseProperty("DefectArea")]
        public virtual ICollection<tm_DefectAreaType_Mapping> tm_DefectAreaType_Mapping { get; set; }
    }
}
