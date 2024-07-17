using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitFormPassCondition
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? GroupID { get; set; }
        public int? LockStatusID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActionDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }
    }
}
