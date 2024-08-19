using System;
namespace Project.ConstructionTracking.Web.Models.MFormModel
{
	public class FormTypeResp
	{
		public int FormTypeID { get; set; }
		public int ProjectTypeID { get; set; }
		public string FormTypeName { get; set; }
		public string FormTypeDesc { get; set; }
		public bool? FlagActive { get; set; }
	}
}

