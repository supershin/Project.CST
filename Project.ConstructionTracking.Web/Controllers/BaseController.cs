using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class BaseController : Controller
    {
        protected string? BaseUrl = null;
        private static readonly string[] AuthCookieNames =
        {
            "CST.ID",
            "CST.UserName",
            "CST.Name",
            "CST.Role",
            "CST.Email",
            "CST.SessionID"
        };

        public BaseController()
        {
        }

        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var url = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.PathBase}";
            url = url.EndsWith("/") ? url : string.Concat(url, "/");
            BaseUrl = url;
            ViewBag.baseUrl = BaseUrl;

            if (!HasValidUserCookies(context.HttpContext.Request))
            {
                ClearAuthCookies(context.HttpContext.Response);

                context.Result = IsAjaxRequest(context.HttpContext.Request)
                    ? new JsonResult(new { success = false, message = "Session expired. Please login again." })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    }
                    : RedirectToAction("Index", "Login");

                return;
            }

            setUserProfile();
            base.OnActionExecuting(context);
        }

        private void setUserProfile()
        {
            var userName = Request.Cookies["CST.UserName"];
            var userRole = Request.Cookies["CST.Role"];
            var name = Request.Cookies["CST.Name"];
            var id = Request.Cookies["CST.ID"];
            ViewBag.ID = id;
            ViewBag.Name = name;
            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;
        }

        private static bool HasValidUserCookies(HttpRequest request)
        {
            return Guid.TryParse(request.Cookies["CST.ID"], out var userId)
                && userId != Guid.Empty
                && int.TryParse(request.Cookies["CST.Role"], out var roleId)
                && roleId > 0;
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
                || request.Headers.Accept.Any(value => value?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true);
        }

        private static void ClearAuthCookies(HttpResponse response)
        {
            foreach (var cookieName in AuthCookieNames)
            {
                response.Cookies.Delete(cookieName);
            }
        }

        #region Protected function
        protected string RenderRazorViewtoString(Controller controller, string viewName, object? model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine? viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                if (viewEngine == null)
                {
                    throw new InvalidOperationException("Razor view engine is not registered.");
                }

                ViewEngineResult viewEngineResult = viewEngine.FindView(controller.ControllerContext, viewName, false);
                if (!viewEngineResult.Success)
                {
                    throw new InvalidOperationException($"Razor view '{viewName}' was not found.");
                }

                ViewContext viewContext = new ViewContext
                (
                    controller.ControllerContext,
                    viewEngineResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewEngineResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion
    }
}
