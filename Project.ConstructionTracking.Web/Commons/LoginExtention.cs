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
                        ID = Guid.Parse("19430527-992A-4904-85D1-61AEBBBC6C70"),
                        UserName = "PE",
                        Password = "1234",
                        Role = 1
                    };
                    break;
                case "PM":
                    userProfile = new UserProfile
                    {
                        ID = Guid.Parse("8A5296D4-9C54-4515-90EC-F7135F01DCAA"),
                        UserName = "PM",
                        Password = "1234",
                        Role = 2
                    };
                    break;
                case "PJM":
                    userProfile = new UserProfile
                    {
                        ID = Guid.Parse("19430527-992A-4904-85D1-61AEBBBC6C70"),
                        UserName = "PJM",
                        Password = "1234",
                        Role = 3
                    };
                    break;
                case "QC":
                    userProfile = new UserProfile
                    {
                        ID = Guid.Parse("19430527-992A-4904-85D1-61AEBBBC6C70"),
                        UserName = "QC",
                        Password = "1234",
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
                        Role = 0
                    };
                    break;
            }

            return userProfile;
        }
    }
}
