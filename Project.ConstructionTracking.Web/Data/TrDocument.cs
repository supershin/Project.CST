using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrDocument
    {
        public Guid Id { get; set; }
        public Guid? UnitFormId { get; set; }
        public Guid? QcunitCheckListId { get; set; }
        public Guid? ResourceId { get; set; }
        public string? DocumentNo { get; set; }
        public string? DocumentPrefix { get; set; }
        public string? DocuementRunning { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
