using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_FormType
    {
        public tm_FormType()
        {
            tm_Form = new HashSet<tm_Form>();
        }

        [Key]
        public int ID { get; set; }
        public int? ProjectTypeID { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? Name { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? Description { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("ProjectTypeID")]
        [InverseProperty("tm_FormType")]
        public virtual tm_Ext? ProjectType { get; set; }
        [InverseProperty("FormType")]
        public virtual ICollection<tm_Form> tm_Form { get; set; }
    }
}
