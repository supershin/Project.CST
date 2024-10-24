namespace Project.ConstructionTracking.Web.Models.GeneratePDFModel
{
    public class DataGenerateQCPDFResp
    {
        public HeaderQCPdfData? HeaderQCData { get; set; }
        public List<BodyQCPdfListDefectData>? BodyListDefectQCData { get; set; }
        public SummaryQCPdfData? SummaryQCData { get; set; }
        public IsNotReadyQCPdfData? IsNotReadyQCData{ get; set; }
        //add model qc1-4
        public BodyQCPdfData? BodyQCPdf { get; set; }
        public FooterQCPdfData? FooterQCData { get; set; }
    }
    public class HeaderQCPdfData
    {
        public string? QCName { get; set; }
        public string? FormName { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? CompanyVenderName { get; set; }
        public string? UpdateDate { get; set; }
        public string? PEInspectorName { get; set; }
        public string? QCInspectorName { get; set; }
        public string? Seq { get; set; }
        public int? QCStatus { get; set; }
        public string? QCStatusText { get; set; }

        public MainInfo? Info { get; set; } = new MainInfo();
    }
    public class BodyQCPdfListDefectData
    {
        public string? RefSeqDefectText { get; set; }
        public string? DefectAreaText { get; set; }
        public string? DefectTypeText { get; set; }
        public string? DefectDescriptionText { get; set; }
        public string? IsMajorDefectText { get; set; }
        public string? DefectRemark { get; set; }
        public int? DefectStatus { get; set; }
        public List<ListImageDefact>? ListImageDefact { get; set; }
    }
    public class ListImageDefact
    {
        public string? PathImageUrl { get; set; }
    }
    public class SummaryQCPdfData
    {
        public int? SumAllDefect { get; set; }
        public int? SumPassDefect { get; set; }
        public int? SumNotPassDefect { get; set; }
        public List<ListCalDefectBySeq>? CalDefectBySeq { get; set; }
    }
    public class ListCalDefectBySeq
    {
        public int? RefSeq { get; set; }
        public int? RefSeqCnt { get; set; }
    }
    public class FooterQCPdfData
    {
        public string? QCSignaturePathImageUrl { get; set; }
        public string? PESignaturePathImageUrl { get; set; }
    }
    public class IsNotReadyQCPdfData
    {
        public string? Remark { get; set; }

        public List<ListImageQC5>? ListImageQC5 { get; set; }
    }
    public class ListImageQC5
    {
        public string? FileImageUrl { get; set; }
    }


    public class BodyQCPdfData
    {
        public List<QcCheckListDetailData> QcCheckListDetailDatas { get; set; } = new List<QcCheckListDetailData>();
    }

    public class QcCheckListDetailData
    {
        public int? DetailID { get; set; }
        public string DetailName { get; set; }
        public int? StatusID { get; set; }
        public string? DetailRemark { get; set; }
        public int? PassBySeq { get; set; }

        public List<DetailImage> DetailImages { get; set; } = new List<DetailImage>();
        public List<ParentDetailData> ParentDetailDatas { get; set; } = new List<ParentDetailData>();
    }

    public class DetailImage
    {
        public string FilePath { get; set; }
    }

    public class ParentDetailData
    {
        public int? ParentDetailID { get; set; }
        public string ParentDetailName { get; set; }
        public int? ParentStatusID { get; set; }
        public string? ParentDetailRemark { get; set; }
        public int? ParentPassBySeq { get; set; }

        public List<ParentImage> ParentImages { get; set; }
    }

    public class ParentImage
    {
        public string FilePath { get; set; }
    }

    public class MainInfo
    {
        public string MainRemark { get; set; }
        public List<MainImage> MainImages { get; set; } = new List<MainImage>();
    }

    public class MainImage
    {
        public string FilePath { get; set; }
    }
}

