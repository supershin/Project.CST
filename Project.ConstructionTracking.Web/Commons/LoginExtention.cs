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
                        ID = 1,
                        UserName = "PE",
                        Password = "1234",
                        Role = "PE"
                    };
                    break;
                case "PM":
                    userProfile = new UserProfile
                    {
                        ID = 2,
                        UserName = "PM",
                        Password = "1234",
                        Role = "PM"
                    };
                    break;
                case "PJM":
                    userProfile = new UserProfile
                    {
                        ID = 3,
                        UserName = "PJM",
                        Password = "1234",
                        Role = "PJM"
                    };
                    break;
                default:
                    // Handle case where user is not found or return a default profile
                    userProfile = new UserProfile
                    {
                        ID = 0,
                        UserName = "Guest",
                        Password = string.Empty,
                        Role = "Guest"
                    };
                    break;
            }

            return userProfile;
        }
    }
}
