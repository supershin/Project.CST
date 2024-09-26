using System;
namespace Project.ConstructionTracking.Web.Models.MUnitModel
{
	public class DetailUnitResp
	{
		public Guid UnitID { get; set; }
        public Guid ProjectID { get; set; }
        public int? CompanyVendorID { get; set; }
        public string? PONo { get; set; }
        public DateTime? StartDate { get; set; }
        public List<Companys> CompanyVendorList { get; set; }
    }

    public class Companys
    {
        public int VendorID { get; set; }
        public string VendorName { get; set; }
    }
}

