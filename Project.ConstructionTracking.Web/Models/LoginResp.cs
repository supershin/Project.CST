using System;
namespace Project.ConstructionTracking.Web.Models
{
	public class LoginResp
	{
		public Guid ID { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Name { get; set; }
		public int RoleID { get; set; }
	}
}

