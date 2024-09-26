using System;
namespace Project.ConstructionTracking.Web.Models.MUnitModel
{
	public class GetPeFromProjectResp
	{
		public List<UserModel> UserModelList { get; set; }
		
	}
	public class UserModel
	{
        public Guid UserID { get; set; }
        public string FullName { get; set; }
    }
}

