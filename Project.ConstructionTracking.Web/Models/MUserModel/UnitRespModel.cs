using System;
namespace Project.ConstructionTracking.Web.Models.MUserModel
{
	public class UnitRespModel
	{
		public List<BUModel> BUList { get; set; }
		public List<RoleModel> RoleList { get; set; }
		public List<PositionModel> PositionList { get; set; }

    }

	public class BUModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}

	public class RoleModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}

	public class PositionModel
	{
		public string Name { get; set; }
	}
}

