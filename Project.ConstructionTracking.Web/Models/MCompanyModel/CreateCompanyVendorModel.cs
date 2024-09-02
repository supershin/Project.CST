using System;
namespace Project.ConstructionTracking.Web.Models.MCompanyModel
{
	public class CreateCompanyVendorModel
	{
		public string CompanyVendorName { get; set; }

		public Guid RequestUserID { get; set; }
		public int RequestRoleID { get; set; }
	}
}

