using System;
namespace Project.ConstructionTracking.Web.Models.MCompanyModel
{
	public class ActionVendorModel
	{
		public int VendorID { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }

        public Guid RequestUserID { get; set; }
        public int RequestRoleID { get; set; }
    }
}

