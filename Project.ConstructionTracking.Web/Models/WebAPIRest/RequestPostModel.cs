namespace Project.ConstructionTracking.Web.Models.WebAPIRest
{
    public class RequestPostModel
    {
        public class GRVenderrportal
        {
            public class Sends
            {
                public string? GRNO { get; set; }
                public string? PONO { get; set; }
                public string? Remark { get; set; }
            }
            public class Responds
            {
                public int? SyncStatusID { get; set; }
                public string? SyncMessage { get; set; }
            }
        }
    }
}
