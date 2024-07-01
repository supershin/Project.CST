using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_UnitForm
    {
        public tr_UnitForm()
        {
            tr_UnitForm_Action = new HashSet<tr_UnitForm_Action>();
            tr_UnitForm_Action_Log = new HashSet<tr_UnitForm_Action_Log>();
            tr_UnitForm_Detail = new HashSet<tr_UnitForm_Detail>();
            tr_UnitForm_Resource = new HashSet<tr_UnitForm_Resource>();
        }

        [Key]
        public Guid ID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? FormID { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Progress { get; set; }
        public int? Duration { get; set; }
        public int? StatusID { get; set; }
        public bool? FlagActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [ForeignKey("FormID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tr_ProjectForm? Form { get; set; }
        [ForeignKey("ProjectID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tm_Project? Project { get; set; }
        [ForeignKey("UnitID")]
        [InverseProperty("tr_UnitForm")]
        public virtual tm_Unit? Unit { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitForm_Action> tr_UnitForm_Action { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitForm_Action_Log> tr_UnitForm_Action_Log { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitForm_Detail> tr_UnitForm_Detail { get; set; }
        [InverseProperty("UnitForm")]
        public virtual ICollection<tr_UnitForm_Resource> tr_UnitForm_Resource { get; set; }
    }
}
