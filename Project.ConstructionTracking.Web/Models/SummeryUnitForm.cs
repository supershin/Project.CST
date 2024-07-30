using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Project.ConstructionTracking.Web.Models
{
    public class SummeryUnitForm
    {
        public Guid? projectId { get; set; }
        public Guid? UnitID { get; set; }
        public Guid? UnitFormID { get; set; }
        public string? UnitFormName { get; set;}
        public int? FormID { get; set; }
        public string? PE { get; set; }
        public string? QC1 { get; set; }
        public string? QC2 { get; set; }
        public string? QC3 { get; set; }
        public string? QC4_1 { get; set; }
        public string? QC4_2 { get; set; }
        public string? QC5 { get; set; }
        public string? PM { get; set; }
        public string? PassConditionStatus { get; set; }
        public int? LockStatusID { get; set; }
    }
}
