using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Filters;
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

                // Get the key for MD5 hash (if needed)
                string keyPassword = _appSettings.PasswordKey;

                // Get user profile using the login service
                LoginResp userProfile = _loginService.VerifyLogin(User_name, password, keyPassword);

                // Throw exception if user profile is null (login failed)
                if (userProfile == null)
                {
                    throw new Exception("ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง"); // Custom exception message
                }

                // Throw exception if the user does not have a mapping project
                if (userProfile.IsMappingProject == false)
                {
                    throw new Exception("ไม่มีสิทธิ์ในการจัดการโครงการ"); // Custom exception message
                }

                // If login is successful, store user details in cookies
                CookieOptions option = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the expiration date for the cookies
                };

                Response.Cookies.Append("CST.ID", userProfile.ID.ToString(), option);
                Response.Cookies.Append("CST.UserName", userProfile.Username, option);
                Response.Cookies.Append("CST.Name", userProfile.Name, option);
                Response.Cookies.Append("CST.Role", userProfile.RoleID.ToString(), option);
                Response.Cookies.Append("CST.Email", FormatExtension.NullToString(userProfile.Email), option);

                // Create a session ID and store it in cookies
                var sessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append("CST.SessionID", sessionId, option);

                // Retrieve the user's role from cookies
                var userRole = Request.Cookies["CST.Role"];
                ViewBag.UserRole = userRole;

                // Redirect the user based on their role
                var UserRoleID = userProfile.RoleID;
                if (UserRoleID == SystemConstant.UserRole.PE)
                {
                    return RedirectToAction("Index", "ProjectList");
                }
                else if (UserRoleID == SystemConstant.UserRole.PM)
                {
                    return RedirectToAction("Index", "MyTaskPM");
                }
                else if (UserRoleID == SystemConstant.UserRole.PJM)
                {
                    return RedirectToAction("Index", "MyTaskPJM");
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                // Use TempData to store the error message and pass it to the view
                TempData["ErrorMessage"] = ex.Message;
                return View("Index");
            }
        }
        
        public IActionResult ExternalAuthentication(string param)
        {
            try
            {
                //get email from param
                string decryptedParam = EncryptionHelper.DecryptParam(_appSettings.AppPortal.Key, _appSettings.AppPortal.IV, param);
                var arrParams = decryptedParam.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var email = arrParams[0].Trim();

                // Get user profile using the login service
                LoginResp userProfile = _loginService.VerifyLoginByEmail(email);

                // Simulate user authentication (in a real app, you would validate against a database)
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email)
        };
                var identity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(identity);

                // Throw exception if user profile is null (login failed)
                if (userProfile == null)
                {
                    throw new Exception("ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง"); // Custom exception message
                }

                // Throw exception if the user does not have a mapping project
                if (userProfile.IsMappingProject == false)
                {
                    throw new Exception("ไม่มีสิทธิ์ในการจัดการโครงการ"); // Custom exception message
                }

                // If login is successful, store user details in cookies
                CookieOptions option = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the expiration date for the cookies
                };

                Response.Cookies.Append("CST.ID", userProfile.ID.ToString(), option);
                Response.Cookies.Append("CST.UserName", userProfile.Username, option);
                Response.Cookies.Append("CST.Name", userProfile.Name, option);
                Response.Cookies.Append("CST.Role", userProfile.RoleID.ToString(), option);
                Response.Cookies.Append("CST.Email", FormatExtension.NullToString(userProfile.Email), option);

                // Create a session ID and store it in cookies
                var sessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append("CST.SessionID", sessionId, option);

                // Retrieve the user's role from cookies
                var userRole = Request.Cookies["CST.Role"];
                ViewBag.UserRole = userRole;

                // Redirect the user based on their role
                var UserRoleID = userProfile.RoleID;
                if (UserRoleID == SystemConstant.UserRole.PE)
                {
                    return RedirectToAction("Index", "ProjectList");
                }
                else if (UserRoleID == SystemConstant.UserRole.PM)
                {
                    return RedirectToAction("Index", "MyTaskPM");
                }
                else if (UserRoleID == SystemConstant.UserRole.PJM)
                {
                    return RedirectToAction("Index", "MyTaskPJM");
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }                
            }
            catch (Exception ex)
            {
                // Use TempData to store the error message and pass it to the view
                TempData["ErrorMessage"] = ex.Message;
                return View("Index");                
            }           
        }


    }
}
