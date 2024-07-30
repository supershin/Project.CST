using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Project.ConstructionTracking.Web.Models
{
    public class FormGroupModel
    {
        public int GroupID { get; set; }
        public int? FormID { get; set; }
        public string? GroupName { get; set; }
        public int? Sort { get; set; }
        public bool? FlagActive { get; set; }
    }
}
