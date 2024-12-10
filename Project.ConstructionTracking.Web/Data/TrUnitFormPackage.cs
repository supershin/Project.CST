using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrUnitFormPackage
    {
        public int Id { get; set; }
        public Guid? UnitFormId { get; set; }
        public int? FormId { get; set; }
        public int? GroupId { get; set; }
        public int? PackageId { get; set; }
        public string? Remark { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmForm? Form { get; set; }
        public virtual TmFormGroup? Group { get; set; }
        public virtual TmFormPackage? Package { get; set; }
        public virtual TrUnitForm? UnitForm { get; set; }
    }
}
