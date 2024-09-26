namespace Project.ConstructionTracking.Web.Models.GeneratePDFModel
{
    public class DataSaveTableResource
    {
        public Guid UnitFormID { get; set; }
        public Guid QCUnitCheckListID { get; set; }
        public string? documentRunning { get; set; }
        public string? documentPrefix { get; set; }
        public string? documentNo { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public Guid UserID { get; set; }
    }
}
