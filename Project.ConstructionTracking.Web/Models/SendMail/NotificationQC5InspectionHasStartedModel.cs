namespace Project.ConstructionTracking.Web.Models.SendMail
{
    public class NotificationQC5InspectionHasStartedModel
    {
        public string? ProjectName { get; set; }
        public string? UnitCode { get; set; }
        public string? QCInspectionName { get; set; }
        public string? DateInspection { get; set; }
        public List<NotificationQC5InspectionHasStartedAccount>? ListNotiAccount { get; set; }
        public class NotificationQC5InspectionHasStartedAccount
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
        }
    }
}
