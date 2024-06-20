using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitForm_Action_Log
    {
        [Key]
        public int ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? ActionTypeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActionDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CraeteDate { get; set; }
        public Guid? CreateBy { get; set; }

        [ForeignKey("UnitFormID")]
        [InverseProperty("tr_UnitForm_Action_Log")]
        public virtual tr_UnitForm? UnitForm { get; set; }
    }
}
