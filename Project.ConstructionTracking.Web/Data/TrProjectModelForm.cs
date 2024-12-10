using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrProjectModelForm
    {
        public int Id { get; set; }
        public Guid? ProjectId { get; set; }
        public int? ModelTypeId { get; set; }
        public int? FormTypeId { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
