using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_FormPackage
    {
        public tm_FormPackage()
        {
            tm_FormCheckList = new HashSet<tm_FormCheckList>();
        }

        [Key]
        public int ID { get; set; }
        public int? GroupID { get; set; }
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

        [ForeignKey("GroupID")]
        [InverseProperty("tm_FormPackage")]
        public virtual tm_FormGroup? Group { get; set; }
        [InverseProperty("Package")]
        public virtual ICollection<tm_FormCheckList> tm_FormCheckList { get; set; }
    }
}
