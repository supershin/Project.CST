using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmModelType
    {
        public TmModelType()
        {
            TmUnits = new HashSet<TmUnit>();
        }

        public int Id { get; set; }
        public Guid? ProjectId { get; set; }
        public string? ModelCode { get; set; }
        public string? ModelName { get; set; }
        public decimal? ModelArea { get; set; }
        public string? ModelTypeCode { get; set; }
        public string? ModelTypeName { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmProject? Project { get; set; }
        public virtual ICollection<TmUnit> TmUnits { get; set; }
    }
}
