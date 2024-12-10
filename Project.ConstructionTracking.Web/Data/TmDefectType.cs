using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmDefectType
    {
        public TmDefectType()
        {
            TmDefectAreaTypeMappings = new HashSet<TmDefectAreaTypeMapping>();
            TmDefectDescriptions = new HashSet<TmDefectDescription>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? LineOrder { get; set; }
        public int? Revise { get; set; }
        public int IsProduct { get; set; }
        public int FlagActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }

        public virtual ICollection<TmDefectAreaTypeMapping> TmDefectAreaTypeMappings { get; set; }
        public virtual ICollection<TmDefectDescription> TmDefectDescriptions { get; set; }
    }
}
