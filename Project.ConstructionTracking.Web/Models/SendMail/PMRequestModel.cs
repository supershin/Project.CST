namespace Project.ConstructionTracking.Web.Models.SendMail
{
    public class PMRequestModel
    {
        public string? PJMFullName { get; set; }
        public string? FormName { get; set; }
        public string? PEFullName { get; set; }
        public string? ActionDatePE { get; set; }
        public string? PMFullName { get; set; }
        public string? ActionDatePM { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? PJMEmail { get; set; }
        public List<PMRequesPassConditionModel>? ListPMRequesPassCondition { get; set; }
    }
    public class PMRequesPassConditionModel
    {
        public string? FormGroupName { get; set; }
        public string? RemarkPassCoditionPE { get; set; }
        public string? RemarkPassCoditionPM { get; set; }
    }
}
