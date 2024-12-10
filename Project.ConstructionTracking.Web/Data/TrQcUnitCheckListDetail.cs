using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrQcUnitCheckListDetail
    {
        public TrQcUnitCheckListDetail()
        {
            TrQcUnitCheckListResources = new HashSet<TrQcUnitCheckListResource>();
        }

        public int Id { get; set; }
        public Guid? QcunitCheckListId { get; set; }
        public int? CheckListId { get; set; }
        public int? CheckListDetailId { get; set; }
        public int? StatusId { get; set; }
        public string? Remark { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public int? PassBySeq { get; set; }

        public virtual TmQcCheckListDetail? CheckListDetail { get; set; }
        public virtual TrQcUnitCheckList? QcunitCheckList { get; set; }
        public virtual ICollection<TrQcUnitCheckListResource> TrQcUnitCheckListResources { get; set; }
    }
}
