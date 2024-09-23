using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class QcStatusListSummaryResp
	{
		public List<QcStatusList> QcStatusLists { get; set; }
	}

	public class QcStatusList
	{
		public Guid QcUnitCheckListID { get; set; }
		public int Seq { get; set; }
		public int QcResultStatus { get; set; }
		public string QcResultStatusDesc { get; set; }
	}
}

