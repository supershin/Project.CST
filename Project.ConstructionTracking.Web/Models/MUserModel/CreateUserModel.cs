using System;
namespace Project.ConstructionTracking.Web.Models.MUserModel
{
	public class CreateUserModel
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string MobileNo { get; set; }
		public int BUID { get; set; }
		public int RoleID { get; set; }
		public string JobPosition { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }

		public string? PasswordKey { get; set; }
	}
}

