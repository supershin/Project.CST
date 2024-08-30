namespace Project.ConstructionTracking.Web.Models
{
    public class ChatInBoxModel
    {
        public class insertInBox
        {
            public Guid? UserID { get; set; }
            public Guid UnitFormID { get; set; }
            public int RoleID { get; set; }
            public int FormID { get; set; }
            public string? TextInbox { get; set; }
        }
        public class GetlistChatInBox
        {
            public Guid? UserID { get; set; }
            public string? UserName { get; set; }
            public Guid UnitFormID { get; set; }
            public string? UnitCode { get; set; }
            public int RoleID { get; set; }
            public string? RoleName { get; set; }
            public int FormID { get; set; }
            public string? FormName { get; set; }
            public string? TextInbox { get; set; }
            public string? Actiondate { get; set; }
        }
    }
}
