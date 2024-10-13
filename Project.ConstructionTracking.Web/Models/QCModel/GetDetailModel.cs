using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class GetDetailModel
	{
		public Guid ProjectID { get; set; }
		public Guid UnitID { get; set; }
		public Guid? ID { get; set; }
		public int QcCheckListID { get; set; }
		public int Seq { get; set; }
		public int QcTypeID { get; set; }
	}
}

