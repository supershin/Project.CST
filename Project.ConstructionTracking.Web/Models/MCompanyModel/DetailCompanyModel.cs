using System;
using Project.ConstructionTracking.Web.Models.MProjectModel;

namespace Project.ConstructionTracking.Web.Models.MCompanyModel
{
	public class DetailCompanyModel
	{
		public int CompanyID { get; set; }
		public string CompanyName { get; set; }
		public List<ProjectMapping> ProjectMappings { get; set; }

        public List<Projects> ProjectLists { get; set; }
    }

	public class ProjectMapping
	{
		public Guid ProjectID { get; set; }
		public string ProjectName { get; set; }
	}

    public class Projects
    {
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
    }
}

