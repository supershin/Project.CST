using System;
namespace Project.ConstructionTracking.Web.Models.MFormModel
{
	public class FormDetail
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Progress { get; set; }
		public int DurationDay { get; set; }
		public List<QcList> QcLists { get; set; }
	}
}

