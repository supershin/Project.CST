using Project.ConstructionTracking.Web.Models.QC5CheckModel;

namespace Project.ConstructionTracking.Web.Models.SendMail
{
    public class PERequesModel
    {
        public string? PMFullName { get; set; }
        public string? FormName { get; set; }
        public string? PEFullName { get; set; }
        public string? ActionDate { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? PMEmail { get; set; }
        public List<PERequesPassConditionModel>? ListPERequesPassCondition { get; set; }
    }
    public class PERequesPassConditionModel
    {
        public string? FormGroupName { get; set; }
        public string? RemarkPassCodition { get; set; }
    }
}
