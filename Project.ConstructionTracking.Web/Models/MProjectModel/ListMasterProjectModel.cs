using System;
namespace Project.ConstructionTracking.Web.Models.MProjectModel
{
	public class ListMasterProjectModel
	{
		public Guid ProjectId { get; set; }
		public string ProjectCode { get; set; }
		public string ProjectName { get; set; }
		public string UpdateTime { get; set; }
	}
}

