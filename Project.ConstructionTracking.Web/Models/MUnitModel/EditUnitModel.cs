using System;
namespace Project.ConstructionTracking.Web.Models.MUnitModel
{ 
	public class EditUnitModel
	{
		public Guid UnitID { get; set; }
		public Guid ProjectID { get; set; }
		public int VendorID { get; set; }
		public string PONo { get; set; }
		public DateTime StartDate { get; set; }

        public Guid RequestUserID { get; set; }
        public int RequestRoleID { get; set; }
    }
}

