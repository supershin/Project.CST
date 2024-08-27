using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    [Keyless]
    public partial class tr_UnitFormInbox
    {
        public int ID { get; set; }
        public Guid UnitFormID { get; set; }
        public int FormID { get; set; }
        public int? RoleID { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? TextInbox { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActionDate { get; set; }
        public Guid? ActionBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CraeteBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
