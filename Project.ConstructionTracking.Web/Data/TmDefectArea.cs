using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmDefectArea
    {
        public TmDefectArea()
        {
            TmDefectAreaTypeMappings = new HashSet<TmDefectAreaTypeMapping>();
        }

        public int Id { get; set; }
        public int? ProjectTypeId { get; set; }
        public string? Name { get; set; }
        public int? LineOrder { get; set; }
        public int? Revise { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }
        public int? IsContactCenter { get; set; }

        public virtual TmExt? ProjectType { get; set; }
        public virtual ICollection<TmDefectAreaTypeMapping> TmDefectAreaTypeMappings { get; set; }
    }
}
