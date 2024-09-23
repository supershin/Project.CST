using System;
namespace Project.ConstructionTracking.Web.Models.MUnitModel
{
	public class EditUnitResp
	{
		public Guid UnitID { get; set; }
		public Guid ProjectID { get; set; }
		public int CompanyVendorID { get; set; }
		public string PONo { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}

