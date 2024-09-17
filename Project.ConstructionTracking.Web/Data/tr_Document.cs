using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_Document
    {
        [Key]
        public Guid ID { get; set; }
        public Guid? UnitFormID { get; set; }
        public Guid? QCUnitCheckListID { get; set; }
        public Guid? ResourceID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? DocumentNo { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? DocumentPrefix { get; set; }
        [StringLength(4)]
        [Unicode(false)]
        public string? DocuementRunning { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
