using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TmQcCheckListDetail
    {
        public TmQcCheckListDetail()
        {
            InverseParent = new HashSet<TmQcCheckListDetail>();
            TrQcUnitCheckListDetails = new HashSet<TrQcUnitCheckListDetail>();
        }

        public int Id { get; set; }
        public int? QccheckListId { get; set; }
        public int? ParentId { get; set; }
        public string? Name { get; set; }
        public int? LineOrder { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmQcCheckListDetail? Parent { get; set; }
        public virtual TmQcCheckList? QccheckList { get; set; }
        public virtual ICollection<TmQcCheckListDetail> InverseParent { get; set; }
        public virtual ICollection<TrQcUnitCheckListDetail> TrQcUnitCheckListDetails { get; set; }
    }
}
