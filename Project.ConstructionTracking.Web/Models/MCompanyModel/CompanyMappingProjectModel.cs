using System;
namespace Project.ConstructionTracking.Web.Models.MCompanyModel
{
	public class CompanyMappingProjectModel
	{
		public int CompanyVendorID { get; set; }
		public string CompanyVendorName { get; set; }
		public List<Guid?> ProJectIDList { get; set; }

        public Guid RequestUserID { get; set; }
        public int RequestRoleID { get; set; }
    }
}

