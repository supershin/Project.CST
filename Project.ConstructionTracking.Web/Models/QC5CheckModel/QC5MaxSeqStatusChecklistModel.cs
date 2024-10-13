namespace Project.ConstructionTracking.Web.Models.QC5CheckModel
{
    public class QC5MaxSeqStatusChecklistModel
    {
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? CheckListID { get; set; }
        public int? QCTypeID { get; set; }
        public int? Seq { get; set; }
        public DateTime? CheckListDate { get; set; }
        public int? QCStatusID { get; set; }
        public string? ActionType { get; set; }
        public Guid? PESignResourceID { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
