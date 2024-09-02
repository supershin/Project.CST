using System;
namespace Project.ConstructionTracking.Web.Models.MProjectModel
{
	public class CreateProjectModel
	{
		public int BUID { get; set; }
		public int ProjectTypeID { get; set; }
		public string ProjectCode { get; set; }
	    public string ProjectName { get; set; }

		public Guid RequestUserID { get; set; }
		public int RequestRoleID { get; set; }
	}
}

