using System;
namespace Project.ConstructionTracking.Web.Models.MProjectModel
{
	public class EditProjectResp
    {
        public int BUID { get; set; }
        public int ProjectTypeID { get; set; }
        public Guid ProjectID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public List<ModelForm> ModelMapping { get; set; }
    }
}

