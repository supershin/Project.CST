using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrQcUnitCheckListResource
    {
        public int Id { get; set; }
        public Guid? QcunitCheckListId { get; set; }
        public int? QcunitCheckListDetailId { get; set; }
        public int? DefectId { get; set; }
        public Guid? ResourceId { get; set; }
        public bool? IsSign { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TrQcUnitCheckListDefect? Defect { get; set; }
        public virtual TrQcUnitCheckList? QcunitCheckList { get; set; }
        public virtual TrQcUnitCheckListDetail? QcunitCheckListDetail { get; set; }
        public virtual TmResource? Resource { get; set; }
    }
}
