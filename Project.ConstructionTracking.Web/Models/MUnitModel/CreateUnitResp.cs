using System;
namespace Project.ConstructionTracking.Web.Models.MUnitModel
{
	public class CreateUnitResp
	{
		public Guid UnitID { get; set; }
        public Guid ProjectID { get; set; }
        public int UnitTypeID { get; set; }
        public int ModelTypeID { get; set; }
        public string UnitCode { get; set; }
        public string UnitAddress { get; set; }
        public decimal UnitArea { get; set; }
        public bool FlagActive { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

