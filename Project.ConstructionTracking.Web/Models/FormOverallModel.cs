using Microsoft.AspNetCore.Mvc.Rendering;

namespace Project.ConstructionTracking.Web.Models
{
    public class ProjectFormList
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? Action { get; set; }
    }
}
