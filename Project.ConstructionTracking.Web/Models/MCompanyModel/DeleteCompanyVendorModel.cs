using System;
namespace Project.ConstructionTracking.Web.Models.MCompanyModel
{
	public class DeleteCompanyVendorModel
	{
		public int? CompanyID { get; set; }
		public int? VendorID { get; set; }

        public Guid RequestUserID { get; set; }
        public int RequestRoleID { get; set; }
    }
}

