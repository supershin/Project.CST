using System;
namespace Project.ConstructionTracking.Web.Models.MProjectModel
{
	public class EditProjectModel
	{
		public int BUID { get; set; }
		public int ProjectTypeID { get; set; }
		public Guid ProjectID { get; set; }
		public string ProjectCode { get; set; }
		public string ProjectName { get; set; }
		public List<ModelForm> ModelMapping { get; set; }
    }

	public class ModelForm
	{
		public int ModelID { get; set; }
		public int FormTypeID { get; set; }
	}
}

