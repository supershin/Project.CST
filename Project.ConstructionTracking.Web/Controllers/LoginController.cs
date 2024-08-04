using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Commons;
using System.Security.Claims;

namespace Project.ConstructionTracking.Web.Controllers
{
	public class LoginController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

        [HttpPost]
        public IActionResult Login(string User_name, string password)
        {
            // Simulate user authentication (in a real app, you would validate against a database)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, User_name)
            };
            var identity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(identity);

            // Get user profile using the extension method
            var userProfile = LoginExtension.GetUserProfile(principal);

            // Check if the password is correct (simple check for demonstration purposes)
            if (userProfile != null && userProfile.Password == password)
            {
                // Set the username and role in cookies
                CookieOptions option = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the expiration date for the cookies
                };

                Response.Cookies.Append("CST.UserName", userProfile.UserName, option);
                Response.Cookies.Append("CST.Role", userProfile.Role?.ToString(), option);

                // Set another session cookie
                var sessionId = Guid.NewGuid().ToString(); // Example session ID
                Response.Cookies.Append("CST.SessionID", sessionId, option);

                // Handle successful login (e.g., redirect to a dashboard)
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                // Handle failed login
                ModelState.AddModelError("", "Invalid username or password");
                return View("Index");
            }
        }
  //      [HttpPost]
		//public IActionResult Login()
		//{

  //          return RedirectToAction("Index", "Dashboard");
  //      }
    }
}
