using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Storage;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class BaseController : Controller
    {
        protected string? BaseUrl = null;

        // Safe accessor — guaranteed non-empty after OnActionExecuting auth guard
        protected Guid CurrentUserID =>
            Guid.TryParse(Request.Cookies["CST.ID"], out var id) ? id : Guid.Empty;

        public BaseController()
        {
        }

        [Microsoft.AspNetCore.Mvc.NonAction]
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var rawId = context.HttpContext.Request.Cookies["CST.ID"];

            // Guard: cookie หมดอายุหรือไม่มี → redirect to login แทนที่จะ throw 500
            if (string.IsNullOrEmpty(rawId) || !Guid.TryParse(rawId, out _))
            {
                bool isAjax = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isAjax)
                {
                    context.Result = new JsonResult(new { status = 0, message = "Session หมดอายุ กรุณา Login ใหม่" })
                    {
                        StatusCode = 401
                    };
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Login", null);
                }
                return;
            }

            var url = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.PathBase}";
            url = url.EndsWith("/") ? url : string.Concat(url, "/");
            BaseUrl = url;
            ViewBag.baseUrl = BaseUrl;

            setUserProfile();
            base.OnActionExecuting(context);
        }

        private void setUserProfile()
        {
            ViewBag.ID       = Request.Cookies["CST.ID"];
            ViewBag.Name     = Request.Cookies["CST.Name"];
            ViewBag.UserName = Request.Cookies["CST.UserName"];
            ViewBag.UserRole = Request.Cookies["CST.Role"];
        }

        #region Protected function
        protected string RenderRazorViewtoString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine? viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewEngineResult = viewEngine.FindView(controller.ControllerContext, viewName, false);
                if (!viewEngineResult.Success)
                    throw new InvalidOperationException($"View '{viewName}' not found. Searched: {string.Join(", ", viewEngineResult.SearchedLocations)}");

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
