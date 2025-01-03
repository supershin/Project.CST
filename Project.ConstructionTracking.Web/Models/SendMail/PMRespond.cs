namespace Project.ConstructionTracking.Web.Models.SendMail
{
    public class PMRespond
    {
        public string? StatusName { get; set; }
        public int? StatusID { get; set; } // Only StatusID remains as int
        public string? PEFullname { get; set; }
        public string? FormName { get; set; }
        public string? PMFullname { get; set; }
        public string? ActionDate { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? PMRemark { get; set; }
        public string? PEEmail { get; set; }
        public List<PMRespondPassConditionModel>? ListPMRespondPassCondition { get; set; }
    }
    public class PMRespondPassConditionModel
    {
        public string? FormGroupName { get; set; }
        public int? PCStatusID { get; set; }
        public string? PCStatusName { get; set; }
        public string? RemarkPEPassCodition { get; set; }
        public string? RemarkPMPassCodition { get; set; }
    }
}
