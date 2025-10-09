namespace Project.ConstructionTracking.Web.Models.SendMail
{
    public class AdminRespond
    {
        public string? StatusName { get; set; }
        public int? StatusID { get; set; } // Only StatusID remains as int
        public string? FormName { get; set; }
        public string? PMFullname { get; set; }
        public string? ActionDate { get; set; }
        public string? PJMFullName { get; set; }
        public string? PJMActionDate { get; set; }
        public string? PJMRemark { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? PMRemark { get; set; }
        public List<AdminRespondPassConditionModel>? ListAdminRespondPassCondition { get; set; }
        public List<ListSendEmailAdmin>? ListSendEmailAdmin { get; set; }
    }
    public class AdminRespondPassConditionModel
    {
        public string? FormGroupName { get; set; }
        public int? PCStatusID { get; set; }
        public string? PCStatusName { get; set; }
        public string? RemarkPEPassCodition { get; set; }
        public string? RemarkPMPassCodition { get; set; }
        public string? RemarkPJMPassCodition { get; set; }
    }
    public class ListSendEmailAdmin
    {
        public string? AdminFullname { get; set; }
        public string? AdminEmail { get; set; }
    }
}
