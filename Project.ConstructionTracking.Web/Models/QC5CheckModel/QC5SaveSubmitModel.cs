namespace Project.ConstructionTracking.Web.Models.QC5CheckModel
{
    public class QC5SaveSubmitModel
    {
        public Guid? ProjectID { get; set; }
        public Guid? UnitID { get; set; }
        public Guid? QCUnitCheckListID { get; set; }
        public int? QCUnitCheckListActionID { get; set; }
        public string? Seq { get; set; }
        public string? ActionType { get; set; }
        public int? QCStatusID { get; set; }
        public string? QCRemark { get; set; }
        public Guid? UserID { get; set; }
        public string? ApplicationPath { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
