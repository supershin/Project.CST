namespace Project.ConstructionTracking.Web.Models.WebAPIRest
{
    public class RequestPostModel
    {
        public class GRVenderrportal
        {
            public class Sends
            {
                public string? grno { get; set; }
                public string? pono { get; set; }
                public string? remark { get; set; }
                public List<IFormFile> fileData { get; set; } = new List<IFormFile>();
            }
            public class Responds
            {
                public int? status { get; set; }
                public string? message { get; set; }
            }
        }
    }
}
