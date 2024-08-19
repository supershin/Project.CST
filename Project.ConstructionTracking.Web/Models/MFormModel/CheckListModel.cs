using System;
namespace Project.ConstructionTracking.Web.Models.MFormModel
{
	public class CheckListModel
	{
        public int TypeData { get; set; }
        public int? FormTypeID { get; set; }
        public int? PackageID { get; set; }
        public int? CheckListID { get; set; }
        public string CheckListName { get; set; } = default!;
    }
}

