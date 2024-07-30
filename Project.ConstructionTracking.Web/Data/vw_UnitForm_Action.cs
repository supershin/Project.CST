using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Data
{
    [Keyless]
    public partial class vw_UnitForm_Action
    {
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public Guid? UnitFormID { get; set; }
        public int? FormID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? PE { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? PM { get; set; }
        [StringLength(7)]
        [Unicode(false)]
        public string? PassConditionStatus { get; set; }
        public int? LockStatusID { get; set; }
    }
}
