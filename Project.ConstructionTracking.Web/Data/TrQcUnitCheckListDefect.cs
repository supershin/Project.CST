using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrQcUnitCheckListDefect
    {
        public TrQcUnitCheckListDefect()
        {
            TrQcUnitCheckListResources = new HashSet<TrQcUnitCheckListResource>();
        }

        public int Id { get; set; }
        public Guid? QcunitCheckListId { get; set; }
        public int? RefSeq { get; set; }
        public int? Seq { get; set; }
        public int? DefectAreaId { get; set; }
        public int? DefectTypeId { get; set; }
        public int? DefectDescriptionId { get; set; }
        public int? StatusId { get; set; }
        public string? Remark { get; set; }
        public bool? IsMajorDefect { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual ICollection<TrQcUnitCheckListResource> TrQcUnitCheckListResources { get; set; }
    }
}
