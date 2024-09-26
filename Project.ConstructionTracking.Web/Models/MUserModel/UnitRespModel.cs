using System;
namespace Project.ConstructionTracking.Web.Models.MUserModel
{
	public class UnitRespModel
	{
		public List<BUModel> BUList { get; set; }
		public List<RoleModel> RoleList { get; set; }
		public List<PositionModel> PositionList { get; set; }

		public List<ProjectByBU> ProjectList { get; set; }
    }

	public class BUModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public List<ProjectByBuModel> ProjectByBu { get; set; }
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

	public class ProjectByBU
	{
		public Guid ProjectID { get; set; }
		public string ProjectName { get; set; }
		public bool IsChecked { get; set; }
	}

    public class ProjectByBuModel
    {
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
        public bool IsChecked { get; set; }
    }
}

