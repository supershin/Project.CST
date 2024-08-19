using System;
namespace Project.ConstructionTracking.Web.Models.MFormModel
{
	public class ProjectTypeListModel
	{
		public List<ProjectType>? ProjectTypeList { get; set; }

		public DetailFormType DetailFormType { get; set; }
	}

	public class ProjectType
	{
		public int ID { get; set; }
		public string Name { get; set; } = default!;
	}

	public class DetailFormType
	{
		public int FormTypeId { get; set; }
		public string FormTypeName { get; set; }
		public string Description { get; set; }
		public int ProjectTypeId { get; set; }
		public string ProjectTypeName { get; set; }
	}
}

