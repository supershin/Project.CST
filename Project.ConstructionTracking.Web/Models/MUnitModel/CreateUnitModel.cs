using System;
namespace Project.ConstructionTracking.Web.Models.MUnitModel
{
	public class CreateUnitModel
	{
		public Guid ProjectID { get; set; }
		public int UnitTypeID { get; set; }
		public int ModelTypeID { get; set; }
		public string UnitCode { get; set; }
		public string UnitAddress { get; set; }
		public decimal UnitArea { get; set; }

		public Guid RequestUserID { get; set; }
		public int RequestRoleID { get; set; }
	}
}

