using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmDefectDescription
    {
        public int Id { get; set; }
        public int DefectTypeId { get; set; }
        public string Name { get; set; } = null!;
        public int? LineOrder { get; set; }
        public int? Revise { get; set; }
        public int FlagActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }

        public virtual TmDefectType DefectType { get; set; } = null!;
    }
}
