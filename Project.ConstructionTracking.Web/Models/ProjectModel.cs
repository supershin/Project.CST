using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.ConstructionTracking.Web.Models
{
    public class ProjectModel
    {
        public string ProjectID { get; set; }
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }
    }
}
