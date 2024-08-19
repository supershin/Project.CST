using System;
namespace Project.ConstructionTracking.Web.Models.MFormModel
{
	public class FormTypeDetailModel
	{
		public List<QcList> DataQcList { get; set; }
	}

	public class QcList
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}
}

