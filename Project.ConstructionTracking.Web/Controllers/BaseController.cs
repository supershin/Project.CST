using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class BaseController : Controller
    {
        protected string? BaseUrl = null;
    
        public BaseController()
        {
        }

        [Microsoft.AspNetCore.Mvc.NonAction]
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var url = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.PathBase}";
            url = url.EndsWith("/") ? url : string.Concat(url, "/");
            BaseUrl = url;
            ViewBag.baseUrl = BaseUrl;

            setUserProfile();
            base.OnActionExecuting(context);
        }
        private void setUserProfile() {
            var userName = Request.Cookies["CST.UserName"];
            var userRole = Request.Cookies["CST.Role"];
            var name = Request.Cookies["CST.Name"];
            var id = Request.Cookies["CST.ID"];
            ViewBag.ID = id;
            ViewBag.Name = name;
            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;
        }
    }

}
