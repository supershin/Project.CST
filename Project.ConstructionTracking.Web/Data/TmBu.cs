using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmBu
    {
        public TmBu()
        {
            TmProjects = new HashSet<TmProject>();
            TmUsers = new HashSet<TmUser>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        public virtual ICollection<TmProject> TmProjects { get; set; }
        public virtual ICollection<TmUser> TmUsers { get; set; }
    }
}
