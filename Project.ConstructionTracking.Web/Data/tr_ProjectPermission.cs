using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_ProjectPermission
    {
        [Key]
        public int ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UserID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey("ProjectID")]
        [InverseProperty("tr_ProjectPermission")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("UserID")]
        [InverseProperty("tr_ProjectPermission")]
        public virtual tm_User? User { get; set; }
    }
}
