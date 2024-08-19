using System;
namespace Project.ConstructionTracking.Web.Models.MFormModel
{
	public class PackageModel
	{
        public int TypeData { get; set; }
        public int? FormTypeID { get; set; }
        public int? GroupID { get; set; }
        public int? PackageID { get; set; }
        public string PackageName { get; set; } = default!;
    }
}

