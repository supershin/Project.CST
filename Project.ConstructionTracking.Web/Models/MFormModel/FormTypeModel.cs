using System;
namespace Project.ConstructionTracking.Web.Models.MFormModel
{
	public class FormTypeModel
	{
		public int TypeData { get; set; }
		public int? ProjectTypeID { get; set; }
		public int? FormTypeID { get; set; }
		public string? FormTypeName { get; set; } = default!;
		public string? FormTypeDesc { get; set; } = default!;
	}
}

