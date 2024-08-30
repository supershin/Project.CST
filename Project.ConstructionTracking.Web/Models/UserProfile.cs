namespace Project.ConstructionTracking.Web.Models
{
    public class UserProfile
    {
        public Guid? ID { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public int? Role { get; set; }
    }
}
