using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_ModelType
    {
        public tm_ModelType()
        {
            tm_Unit = new HashSet<tm_Unit>();
        }

        [Key]
        public int ID { get; set; }
        public Guid? ProjectID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Code { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? Name { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("ProjectID")]
        [InverseProperty("tm_ModelType")]
        public virtual tm_Project? Project { get; set; }
        [InverseProperty("ModelType")]
        public virtual ICollection<tm_Unit> tm_Unit { get; set; }
    }
}
