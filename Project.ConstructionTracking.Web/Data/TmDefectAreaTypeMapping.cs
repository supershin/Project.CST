using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmDefectAreaTypeMapping
    {
        public int Id { get; set; }
        public int DefectAreaId { get; set; }
        public int DefectTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }

        public virtual TmDefectArea DefectArea { get; set; } = null!;
        public virtual TmDefectType DefectType { get; set; } = null!;
    }
}
