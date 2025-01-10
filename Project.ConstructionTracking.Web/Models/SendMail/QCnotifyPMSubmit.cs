namespace Project.ConstructionTracking.Web.Models.SendMail
{
    public class QCnotifyPMSubmit
    {
        public string? QCUserName { get; set; }
        public string? Email { get; set; }
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? PMUserName { get; set; }
        public int? StatusID { get; set; }
        public string? StatusName { get; set; }
        public string? FormName { get; set; }
        public string? QCTypeName { get; set; }
    }
}
