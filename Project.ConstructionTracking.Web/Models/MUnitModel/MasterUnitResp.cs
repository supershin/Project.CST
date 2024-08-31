using System;
namespace Project.ConstructionTracking.Web.Models.MUnitModel
{
	public class MasterUnitResp
	{
		public List<Projects> ProjectList { get; set; }

	}

	public class Projects
	{
		public Guid ProjectID { get; set; }
		public string ProjectName { get; set; }
	}
}

