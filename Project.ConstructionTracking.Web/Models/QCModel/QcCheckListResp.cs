using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class QcCheckListResp
	{
        //info
        public Guid ID { get; set; }
		public Guid ProjectID { get; set; }
		public Guid UnitID { get; set; }
		public int QcCheckListID { get; set; }
		public int QcTypeID { get; set; }
		public int Seq { get; set; }

		// condition checklist status 
		public bool IsNotReadyInspect { get; set; }
		public bool IsPassCondition { get; set; }
		public int QcStatusID { get; set; }

		// user QC
		public Guid? UserQc { get; set; }
		public Guid? UserQcResource { get; set; }
	}
}

