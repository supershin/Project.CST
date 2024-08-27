using System;
namespace Project.ConstructionTracking.Web.Models.MProjectModel
{
	public class DetailProject
	{
		public Guid ProjectID { get; set; }
		public int BUID { get; set; }
		public int ProjectTypeID { get; set; }
		public string ProjectCode { get; set; }
		public string ProjectName { get; set; }
		public List<ModelType> ModelTypeList { get; set; }

		public List<FormType> FormTypeList { get; set; }
	}

	public class ModelType
	{
		public int ModelID { get; set; }
		public string ModelCode { get; set; }
		public string ModelName { get; set; }
        public string ModelTypeCode { get; set; }
		public string ModelTypeName { get; set; }
		public int? FormTypeID { get; set; }
	}

	public class FormType
	{
		public int FormTypeID { get; set; }
		public string FormTypeName { get; set; }
	}
}

