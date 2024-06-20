using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class BaseController : Controller
    {
        protected string? BaseUrl = null;

        private readonly IProjectService _ProjectService;
        public BaseController(IProjectService ProjectService)
        {
            _ProjectService = ProjectService;
        }

        [Microsoft.AspNetCore.Mvc.NonAction]
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var url = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.PathBase}";
            url = url.EndsWith("/") ? url : string.Concat(url, "/");
            BaseUrl = url;
            ViewBag.baseUrl = BaseUrl;

            base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            ViewBag.ProjectSelectList = GetProjectSelectList(); 
            return View();
        }

        public List<SelectListItem> GetProjectSelectList()
        {
            var selectLists = new List<SelectListItem>();
            var lst = _ProjectService.GetProjectList();
            foreach (var item in lst)
            {
                selectLists.Add(new SelectListItem
                {
                    Value = item.ProjectID.ToString(),
                    Text = item.ProjectName 
                });
            }
            return selectLists;
        }
    }

}
