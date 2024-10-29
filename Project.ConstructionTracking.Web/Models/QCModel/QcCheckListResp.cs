using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class QcCheckListResp
	{
        //info
        public Guid? ID { get; set; }
		public Guid ProjectID { get; set; }
		public Guid UnitID { get; set; }
		public int QcCheckListID { get; set; }
		public int QcTypeID { get; set; }
		public int Seq { get; set; }

		public int? QcStatusID { get; set; }

		public string? Remark { get; set; }
		// user QC
		public string? UserQcResourceUrl { get; set; }

		// qc action
		public string? QcActionType { get; set; }

		public List<MainImage> MainImages { get; set; } = new List<MainImage>();

		public string? StartDate { get; set; }
		public string? EndDate { get; set; }
	}

	public class MainImage
	{
		public Guid ImageID { get; set; }
		public string ImageUrl { get; set; }
	}
}

