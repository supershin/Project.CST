using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_FormGroup
    {
        public tm_FormGroup()
        {
            tm_FormPackage = new HashSet<tm_FormPackage>();
        }

        [Key]
        public int ID { get; set; }
        public int? FormID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Name { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("FormID")]
        [InverseProperty("tm_FormGroup")]
        public virtual tm_Form? Form { get; set; }
        [InverseProperty("Group")]
        public virtual ICollection<tm_FormPackage> tm_FormPackage { get; set; }
    }
}
