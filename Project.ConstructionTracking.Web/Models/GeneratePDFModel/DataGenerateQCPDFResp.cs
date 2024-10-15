namespace Project.ConstructionTracking.Web.Models.GeneratePDFModel
{
    public class DataGenerateQCPDFResp
    {
        //public HeaderQCPdfData? HeaderQCData { get; set; }
        //public List<BodyQCPdfListDefectData>? BodyListDefectQCData { get; set; }
        //public SummaryQCPdfData? SummaryQCData { get; set; }
        //public FooterQCPdfData? FooterQCData { get; set; }
        public class HeaderQCPdfData
        {
            public string? QCName { get; set; }
            public string? FormName { get; set; }
            public string? ProjectName { get; set; }
            public string? UnitCode { get; set; }
            public string? CompanyVenderName { get; set; }
            public string? PEInspectorName { get; set; }
            public string? QCInspectorName { get; set; }
            public string? Seq { get; set; }
            public string? QCStatus { get; set; }
            public string? QCStatusText { get; set; }
            public List<BodyQCPdfListDefectData>? BodyListDefectQCData { get; set; }
        }
        public class BodyQCPdfListDefectData
        {
            public string? RefSeqDefectText { get; set; }
            public string? DefectAreaText { get; set; }
            public string? DefectTypeText { get; set; }
            public string? DefectDescriptionText { get; set; }
            public string? IsMajorDefectText { get; set; }
            public string? DefectRemark { get; set; }
            public string? DefectStatusText { get; set; }
            public List<ListImageDefact>? ListImageDefact { get; set; }
        }
        public class ListImageDefact
        {
            public string? PathImageUrl { get; set; }
        }
        public class SummaryQCPdfData
        {
            public string? SumAllDefect { get; set; }
            public string? SumPassDefect { get; set; }
            public string? SumNotPassDefect { get; set; }
            public List<ListCalDefectBySeq>? CalDefectBySeq { get; set; }
        }
        public class ListCalDefectBySeq
        {
            public string? Seq { get; set; }
            public string? SumAllDefectBySeq { get; set; }
        }
        public class FooterQCPdfData
        {

            public string? PESignaturePathImageUrl { get; set; }
            public string? PMSignaturePathImageUrl { get; set; }
        }
    }


}
