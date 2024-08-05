using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Project.ConstructionTracking.Web.Models
{
    public class FormGroupModel
    {
        public Guid? UnitID { get; set; }
        public int GroupID { get; set; }
        public int? FormID { get; set; }
        public Guid? UnitFormID { get; set; }
        public string? GroupName { get; set; }
        public string? StatusUse { get; set;}
        public int? LockStatusID { get; set; }
        public int? Cnt_CheckList_All { get; set; }
        public int? Cnt_CheckList_Unit { get; set; }
        public int? Cnt_CheckList_NotPass { get; set; }
    }
}
