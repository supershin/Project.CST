using System.Data;
using System.Security.Claims;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Commons
{
    public static class LoginExtension
    {
        public static UserProfile GetUserProfile(ClaimsPrincipal user)
        {
            var userName = user.Identity.Name;

            UserProfile userProfile = null;

            switch (userName)
            {
                case "PE":
                    userProfile = new UserProfile
                    {
                        ID = Guid.Parse("D0E92B67-4FF7-4284-892F-25A4BB3722FA"),
                        UserName = "PE",
                        Password = "1234",
                        Name = "สมชาย ใจดี", 
                        Role = 1
                    };
                    break;
                case "PM":
                    userProfile = new UserProfile
                    {
                        ID = Guid.Parse("92a2372e-8ad0-4c5c-8811-2d25e8da0a97"),
                        UserName = "PM",
                        Password = "1234",
                        Name = "สุทธิชัย วงค์ใหญ่",
                        Role = 2
                    };
                    break;
                case "PJM":
                    userProfile = new UserProfile
                    {
                        ID = Guid.Parse("19430527-992a-4904-85d1-61aebbbc6c70"),
                        UserName = "PJM",
                        Password = "1234",
                        Name = "พรชัย แสวงการ",
                        Role = 3
                    };
                    break;
                case "QC":
                    userProfile = new UserProfile
                    {
                        ID = Guid.Parse("19430527-992A-4904-85D1-61AEBBBC6C70"),
                        UserName = "QC",
                        Password = "1234",
                        Name = "",
                        Role = 4
                    };
                    break;
                default:
                    // Handle case where user is not found or return a default profile
                    userProfile = new UserProfile
                    {
                        ID = Guid.Parse("19430527-992A-4904-85D1-61AEBBBC6C70"),
                        UserName = "Guest",
                        Password = string.Empty,
                        Name = "",
                        Role = 0
                    };
                    break;
            }

            return userProfile;
        }
    }
}
