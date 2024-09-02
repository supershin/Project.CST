using System;
namespace Project.ConstructionTracking.Web.Models.MFormModel
{
	public class GroupModel
	{
        public int TypeData { get; set; }
        public int? FormTypeID { get; set; }
        public int? FormID { get; set; }
        public int? GroupID { get; set; }
        public string GroupName { get; set; } = default!;

        public Guid RequestUserID { get; set; }
        public int RequestRoleID { get; set; }
    }
}

