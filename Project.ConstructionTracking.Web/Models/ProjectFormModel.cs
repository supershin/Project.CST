using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Project.ConstructionTracking.Web.Models
{
    public class ProjectFormModel
    {
        public class ProjectForm_getForm
        {
            public int ID { get; set; }
            public Guid? ProjectID { get; set; }
            public Guid? UnitID { get; set; }
            public int? FormID { get; set; }
            public string FormName { get; set; }

            public List<ProjectForm_getListGroups> ListGroups { get; set; }

        }
        public class ProjectForm_getListGroups
        {
            public int GroupID { get; set; }
            public int? FormID { get; set; }
            public string? GroupName { get; set; }
            public int? Sort { get; set; }
            public bool? FlagActive { get; set; }
            public List<ProjectForm_getListPackages> ListPackages { get; set; }
        }
        public class ProjectForm_getListPackages
        {
            public int PackagesID { get; set; }
            public int? GroupID { get; set; }
            public string? PackagesName { get; set; }
            public int? Sort { get; set; }
            public bool? FlagActive { get; set; }
            public List<ProjectForm_getListCheckLists> ListCheckLists { get; set; }
        }
        public class ProjectForm_getListCheckLists
        {
            public int CheckListID { get; set; }
            public int? PackageID { get; set; }
            public string? CheckListName { get; set; }
            public int? Sort { get; set; }
            public bool? FlagActive { get; set; }

        }

    }
}
