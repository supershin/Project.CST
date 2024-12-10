using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmFormType
    {
        public TmFormType()
        {
            TmForms = new HashSet<TmForm>();
        }

        public int Id { get; set; }
        public int? ProjectTypeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmExt? ProjectType { get; set; }
        public virtual ICollection<TmForm> TmForms { get; set; }
    }
}
