using Microsoft.AspNetCore.Mvc.Rendering;

namespace Project.ConstructionTracking.Web.Models
{
    public class FormOverallView
    {
        public List<SelectListItem>? SelectProjectList { get; set; }
        public List<SelectListItem>? SelectUnitTypeList { get; set; }
    }
}
