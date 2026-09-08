using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_ProjectImage
    {
        [Key]
        public Guid ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? ResourceID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("ProjectID")]
        [InverseProperty("tr_ProjectImage")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("ResourceID")]
        [InverseProperty("tr_ProjectImage")]
        public virtual tm_Resource? Resource { get; set; }
    }
}
