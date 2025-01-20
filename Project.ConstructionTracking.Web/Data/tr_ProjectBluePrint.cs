using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class tr_ProjectBluePrint
    {
        [Key]
        public Guid ID { get; set; }
        public Guid ProjectFloorPlanID { get; set; }
        public int ElementType { get; set; }
        public string Coordinates { get; set; } = null!;
        public Guid UnitID { get; set; }
        public bool FlagActive { get; set; }
        public Guid CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        public Guid UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }

        [ForeignKey("ElementType")]
        [InverseProperty("tr_ProjectBluePrint")]
        public virtual tm_Ext ElementTypeNavigation { get; set; } = null!;
        [ForeignKey("UnitID")]
        [InverseProperty("tr_ProjectBluePrint")]
        public virtual tm_Unit Unit { get; set; } = null!;
    }
}
