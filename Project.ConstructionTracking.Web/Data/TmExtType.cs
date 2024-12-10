using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmExtType
    {
        public TmExtType()
        {
            TmExts = new HashSet<TmExt>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        public virtual ICollection<TmExt> TmExts { get; set; }
    }
}
