namespace Project.ConstructionTracking.Web.Models.SendMail
{
    public class PJMRespondModel
    {
        public string? ID { get; set; }
        public int? StatusID { get; set; }
        public string? StatusName { get; set; }
        public string? Fullname { get; set; }
        public string? FormName { get; set; }
        public string? PJMFullName { get; set; }
        public string? PJMActionDate { get; set; }
        public string? PJMRemark { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? Email { get; set; }
        public List<PJMRespondPassConditionModel>? ListPJMRespondPassCondition { get; set; }
    }
    public class PJMRespondPassConditionModel
    {
        public string? FormGroupName { get; set; }
        public int? PCStatusID { get; set; }
        public string? PCStatusName { get; set; }
        public string? RemarkPEPassCodition { get; set; }
        public string? RemarkPMPassCodition { get; set; }
        public string? RemarkPJMPassCodition { get; set; }
    }
}
