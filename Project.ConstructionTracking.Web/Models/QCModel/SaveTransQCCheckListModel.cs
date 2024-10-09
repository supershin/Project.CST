using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class SaveTransQCCheckListModel
	{
		// checklist info 
		public Guid ProjectID { get; set; }
		public Guid UnitID { get; set; }
		public int CheckListID { get; set; }
		public int QCTypeID { get; set; }
		public int Seq { get; set; }

		// sign resource
		public Guid PEID { get; set; }
		public Guid PESignResource { get; set; }

		// action resource
        public bool IsNotReadyInspect { get; set; }
        public int QCStatusID { get; set; } // 10 = pass , 11 = not pass , 15 = in progress
		public string Remark { get; set; }
		public List<FormFile> Images { get; set; } = new List<FormFile>();

		// checklist detail
		public List<CheckListDetailInfo> CheckListItems { get; set; } = new List<CheckListDetailInfo>();
    }

	public class CheckListDetailInfo
	{
		public int CheckListDetailID { get; set; }
		public bool ConditionPass { get; set; }
		public bool ConditionNotPass { get; set; }
		public string DetailRemark { get; set; }
		public List<FormFile> DetailImage { get; set; } = new List<FormFile>();
	}
}

