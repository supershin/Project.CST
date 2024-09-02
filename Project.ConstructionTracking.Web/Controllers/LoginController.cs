using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MUserModel;
using Project.ConstructionTracking.Web.Services;
using System.Security.Claims;

namespace Project.ConstructionTracking.Web.Controllers
{
	public class LoginController : Controller
	{
        private readonly AppSettings _appSettings;
        private readonly IMasterUserService _masterUserService;
        private readonly ILoginService _loginService;

        public LoginController(IOptions<AppSettings> options,
            IMasterUserService masterUserService,
            ILoginService loginService)
        {
            _appSettings = options.Value;
            _masterUserService = masterUserService;
            _loginService = loginService;
        }

        public IActionResult Index()
		{
			return View();
		}

        //[HttpPost]
        //public IActionResult Login(string User_name, string password)
        //{
        //    try
        //    {
        //        // Simulate user authentication (in a real app, you would validate against a database)
        //        var claims = new List<Claim>()
        //        {
        //            new Claim(ClaimTypes.Name, User_name)
        //        };
        //        var identity = new ClaimsIdentity(claims, "login");
        //        var principal = new ClaimsPrincipal(identity);

        //        // get key for gen md5 
        //        string keyPassword = _appSettings.PasswordKey;

        //        // Get user profile using the extension method
        //        // var userProfile = LoginExtension.GetUserProfile(principal);
        //        LoginResp userProfile = _loginService.VerifyLogin(User_name, password, keyPassword);

        //        // Check if the password is correct (simple check for demonstration purposes)
        //        if (userProfile != null && !String.IsNullOrWhiteSpace(userProfile.Password))
        //        {
        //            // Set the username and role in cookies
        //            CookieOptions option = new CookieOptions
        //            {
        //                Expires = DateTime.Now.AddDays(1) // Set the expiration date for the cookies
        //            };
        //            Response.Cookies.Append("CST.ID", userProfile.ID.ToString(), option);
        //            Response.Cookies.Append("CST.UserName", userProfile.Username, option);
        //            Response.Cookies.Append("CST.Name", userProfile.Name, option);
        //            Response.Cookies.Append("CST.Role", userProfile.RoleID.ToString(), option);

        //            // Set another session cookie
        //            var sessionId = Guid.NewGuid().ToString(); // Example session ID
        //            Response.Cookies.Append("CST.SessionID", sessionId, option);

        //            var userRole = Request.Cookies["CST.Role"];
        //            ViewBag.UserRole = userRole;

        //            // Handle successful login (e.g., redirect to a dashboard)
        //            return RedirectToAction("Index", "Dashboard");
        //        }
        //        else
        //        {
        //            // Handle failed login
        //            ModelState.AddModelError("", "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง");
        //            return View("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
        //        return View("Index");
        //    }
        //}

        [HttpPost]
        public IActionResult Login(string User_name, string password)
        {
            try
            {
                // Simulate user authentication (in a real app, you would validate against a database)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, User_name)
                };
                var identity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(identity);

                // get key for gen md5 
                string keyPassword = _appSettings.PasswordKey;

                // Get user profile using the extension method
                LoginResp userProfile = _loginService.VerifyLogin(User_name, password, keyPassword);

                // Check if the password is correct (simple check for demonstration purposes)
                if (userProfile != null && !String.IsNullOrWhiteSpace(userProfile.Password))
                {
                    // Set the username and role in cookies
                    CookieOptions option = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(1) // Set the expiration date for the cookies
                    };
                    Response.Cookies.Append("CST.ID", userProfile.ID.ToString(), option);
                    Response.Cookies.Append("CST.UserName", userProfile.Username, option);
                    Response.Cookies.Append("CST.Name", userProfile.Name, option);
                    Response.Cookies.Append("CST.Role", userProfile.RoleID.ToString(), option);

                    // Set another session cookie
                    var sessionId = Guid.NewGuid().ToString(); // Example session ID
                    Response.Cookies.Append("CST.SessionID", sessionId, option);

                    var userRole = Request.Cookies["CST.Role"];
                    ViewBag.UserRole = userRole;

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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
                return View("Index");
            }
        }

    }
}
