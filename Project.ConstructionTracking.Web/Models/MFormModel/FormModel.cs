using System;
namespace Project.ConstructionTracking.Web.Models.MFormModel
{
	public class FormModel
	{
        public int TypeData { get; set; }
        public int? FormTypeID { get; set; }
        public int? FormID { get; set; }
        public string? FormName { get; set; } = default!;
        public string? FormDesc { get; set; } = default!;
        public decimal? Progress { get; set; }
        public int? Duration { get; set; }
        public List<int>? QcList { get; set; }
    }
}

