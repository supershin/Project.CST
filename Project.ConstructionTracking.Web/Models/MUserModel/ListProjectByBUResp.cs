using System;
namespace Project.ConstructionTracking.Web.Models.MUserModel
{
	public class ListProjectByBUResp
	{
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
        public bool IsChecked { get; set; }
    }
}

