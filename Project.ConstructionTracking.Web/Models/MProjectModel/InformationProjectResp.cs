using System;
namespace Project.ConstructionTracking.Web.Models.MProjectModel
{
	public class InformationProjectResp
	{
		public List<BUModel> BUList { get; set; }
		public List<ProjectType> ProjectTypeList { get; set; }
		
	}

	public class BUModel
	{
		public int BUID { get; set; }
		public string BUName { get; set; }
	}

	public class ProjectType
	{
		public int ProjectTypeID { get; set; }
		public string ProjectTypeName { get; set; }
	}

}

