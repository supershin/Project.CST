using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrFormQccheckList
    {
        public int Id { get; set; }
        public int? FormId { get; set; }
        public int? CheckListId { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmQcCheckList? CheckList { get; set; }
        public virtual TmForm? Form { get; set; }
    }
}
