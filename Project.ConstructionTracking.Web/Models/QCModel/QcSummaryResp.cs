using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class QcSummaryResp
	{
		public Guid ProjectID { get; set; }
		public string ProjectName { get; set; }
		public Guid UnitID { get; set; }
		public int ModelTypeID { get; set; }
		public string UnitCode { get; set; }
		public int UnitStatusID { get; set; }
		public string UnitStatusDesc { get; set; }
		public int FormTypeID { get; set; }
		public string FormTypeName { get; set; }
		public List<QcSummaryList> QcSummaryLists { get; set; }
	}

	public class QcSummaryList
	{
		public int QcCheckListID { get; set; }
		public int QcTypeID { get; set; }
		public string QcTypeName { get; set; }
		public int FormQcCheckList { get; set; }
		public int FormID { get; set; }
		public List<QcInspection> QcInspections { get; set; }
	}

	public class QcInspection
	{
		public Guid QcCheckListID { get; set; }
		public int QcStatusID { get; set; }
		public string QcStatusDesc { get; set; }
		public int Seq { get; set; }
	}
}

