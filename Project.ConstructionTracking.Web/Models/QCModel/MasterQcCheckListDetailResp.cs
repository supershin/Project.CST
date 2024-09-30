using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class MasterQcCheckListDetailResp
	{
		public int QcCheckListID { get; set; }
		public int QcTypeID { get; set; }
		public List<CheckListDetail> CheckListDetails {get;set;}
	}

	public class CheckListDetail
	{
		public int CheckListDetailID { get; set; }
		public string Name { get; set; }
		public int LineOrder { get; set; }
		public List<ParentCheckListDetail> ParentDetails { get; set; }
	}

	public class ParentCheckListDetail
	{
		public int CheckListDetailID { get; set; }
		public int ParentID { get; set; }
		public string Name { get; set; }
		public int LineOrder { get; set; }
	}
}

