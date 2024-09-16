namespace Project.ConstructionTracking.Web.Models.QC5CheckModel
{
    public class QC5IUDModel
    {
        public int? ID { get; set; }
        public Guid? QCUnitCheckListID { get; set; }
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public int? Seq { get; set; }
        public int? DefectAreaID { get; set; }
        public int? DefectTypeID { get; set; }
        public int? DefectDescriptionID { get; set; }
        public int? StatusID { get; set; }
        public string? Remark { get; set; }
        public Guid? UserID { get; set; }
    }
}
