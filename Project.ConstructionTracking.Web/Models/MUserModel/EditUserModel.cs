using System;
namespace Project.ConstructionTracking.Web.Models.MUserModel
{
	public class EditUserModel
	{
        public Guid UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public int BUID { get; set; }
        public int RoleID { get; set; }
        public string JobPosition { get; set; }

        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        public List<Guid>? MappingProject { get; set; }

        public string? SignUser { get; set; }

        public string? KeyPassword { get; set; }
        public string? ApplicationPath { get; set; }
        public Guid RequestUserID { get; set; }
        public int RequestRoleID { get; set; }
    }
}

