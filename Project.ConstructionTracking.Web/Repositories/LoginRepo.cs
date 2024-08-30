using System;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface ILoginRepo
	{
		LoginResp VerifyLogin(string username, string password, string key);
	}

	public class LoginRepo : ILoginRepo
	{
		private readonly ContructionTrackingDbContext _context;
		public LoginRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

		public LoginResp VerifyLogin(string username, string password, string key)
		{
			string verifyPassword = HashExtension.EncryptMD5(password , key);

			tm_User? user = _context.tm_User
							.Where(o => o.Username == username
							&& o.Password == verifyPassword
							&& o.FlagActive == true).FirstOrDefault();

			if (user == null) throw new Exception("ไม่พบข้อมูลรหัสผู้ใช้งาน");

			LoginResp resp = new LoginResp()
			{
				ID = user.ID,
				Username = user.Username,
				Password = user.Password,
				RoleID = (int)user.RoleID
			};

			return resp;
        }
	}
}

