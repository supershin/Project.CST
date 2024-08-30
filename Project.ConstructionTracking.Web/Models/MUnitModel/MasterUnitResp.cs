using System;
namespace Project.ConstructionTracking.Web.Models.MUnitModel
{
	public class MasterUnitResp
	{
		public List<Projects> ProjectList { get; set; }

		public List<UnitTypes> UnitTypeList { get; set; }
	}

	public class Projects
	{
		public Guid ProjectID { get; set; }
		public string ProjectName { get; set; }
		public List<ModelTypes> ModelList { get; set; }
	}

	public class ModelTypes
	{
		public int ModelTypeID { get; set; }
		public string ModelTypeName { get; set; } 
	}

	public class UnitTypes
	{
		public int UnitTypeID { get; set; }
		public string UnitTypeName { get; set; }
	}
}

