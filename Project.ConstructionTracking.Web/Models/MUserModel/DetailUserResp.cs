using System;
namespace Project.ConstructionTracking.Web.Models.MUserModel
{
	public class DetailUserResp
	{
		public Guid UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public int BUID { get; set; }
        public int RoleID { get; set; }
        public string JobPosition { get; set; }
        public string ImageSign { get; set; }

        public UnitRespModel respModel { get; set; }
    }
}

