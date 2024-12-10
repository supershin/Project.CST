using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmResource
    {
        public TmResource()
        {
            TrQcUnitCheckListResources = new HashSet<TrQcUnitCheckListResource>();
            TrQcUnitCheckLists = new HashSet<TrQcUnitCheckList>();
            TrUnitFormResources = new HashSet<TrUnitFormResource>();
            TrUnitForms = new HashSet<TrUnitForm>();
        }

        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? MimeType { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual ICollection<TrQcUnitCheckListResource> TrQcUnitCheckListResources { get; set; }
        public virtual ICollection<TrQcUnitCheckList> TrQcUnitCheckLists { get; set; }
        public virtual ICollection<TrUnitFormResource> TrUnitFormResources { get; set; }
        public virtual ICollection<TrUnitForm> TrUnitForms { get; set; }
    }
}
