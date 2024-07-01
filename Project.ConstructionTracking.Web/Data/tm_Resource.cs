using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tm_Resource
    {
        public tm_Resource()
        {
            tr_UnitForm_Resource = new HashSet<tr_UnitForm_Resource>();
        }

        [Key]
        public Guid ID { get; set; }
        [StringLength(500)]
        public string? FileName { get; set; }
        [StringLength(500)]
        public string? FilePath { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? MimeType { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [InverseProperty("Resource")]
        public virtual ICollection<tr_UnitForm_Resource> tr_UnitForm_Resource { get; set; }
    }
}
