using System;
namespace Project.ConstructionTracking.Web.Models.GeneratePDFModel
{
	public class DataToGenerateModel
	{
		public Guid ProjectID { get; set; }
		public Guid UnitID { get; set; }
		public int FormID { get; set; }
        public Guid QCUnitCheckListID { get; set; }
        public int QCTypeID { get; set; }
    }
}

