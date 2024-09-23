namespace Project.ConstructionTracking.Web.Models.QC5CheckModel
{
    public class QC5DefectModel
    {
        public int? DefectID { get; set; }
        public int? DefectAreaID { get; set; }
        public string? DefectAreaName { get; set; }
        public int? DefectTypeID { get; set; }
        public string? DefectTypeName { get; set; }
        public int? DefectDescriptionID { get; set; }
        public string? DefectDescriptionName { get; set; }
        public int? StatusID { get; set; }
        public string? Remark { get; set; }
        public bool? IsMajorDefect { get; set; }
        public bool? FlagActive { get; set; }
        public int? Seq { get; set; }
        public List<QC5DefactListImageNotPass>? listImageNotpass { get; set; }
    }
    public class QC5DefactListImageNotPass
    {
        public Guid? ResourceID { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
    }
}
