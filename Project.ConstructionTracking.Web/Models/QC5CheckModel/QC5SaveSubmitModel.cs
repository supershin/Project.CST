using static Project.ConstructionTracking.Web.Models.FormGroupModel;

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
        public SignatureQC5? Sign { get; set; }
    }

    public class SignatureQC5
    {
        public string? MimeType { get; set; }
        public string? StorageBase64 { get; set; }

    }

    public class ResourcesSignatureQC5
    {
        public string? MimeType { get; set; }
        public string? ResourceStorageBase64 { get; set; }
        public string? PhysicalPathServer { get; set; }
        public string? ResourceStoragePath { get; set; }
        public string? Directory { get; set; }
    }
}
