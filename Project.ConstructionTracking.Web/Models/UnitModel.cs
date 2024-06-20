using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.ConstructionTracking.Web.Models
{
    public class UnitModel
    {
        public Guid UnitID { get; set; }
        public Guid? ProjectID { get; set; }
        public int? UnitTypeID { get; set; }
        public string? UnitTypeName { get; set; }
        public string? UnitCode { get; set; }
  
        public string? Build { get; set; }
        public int? Floor { get; set; }
        public int? Block { get; set; }
       
        public decimal? Area { get; set; }
     
        public DateTime? StartDate { get; set; }
     
        public DateTime? EndDate { get; set; }
    }
}
