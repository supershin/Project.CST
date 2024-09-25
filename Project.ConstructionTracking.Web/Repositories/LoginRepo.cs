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

            LoginResp resp = new LoginResp();

            if (user == null) throw new Exception("ไม่พบข้อมูลรหัสผู้ใช้งาน");
			else
			{
                bool isPermission = _context.tr_ProjectPermission
										.Any(o => o.UserID == user.ID && o.FlagActive == true);

				if (isPermission)
				{
					resp = new LoginResp()
					{
						ID = user.ID,
						Username = user.Username,
						Password = user.Password,
						Name = user.FirstName + " " + user.LastName,
						RoleID = (int)user.RoleID,
						IsMappingProject = true
					};
				}
				else
				{
					resp = new LoginResp()
					{
						ID = user.ID,
						Username = user.Username,
						Password = user.Password,
						Name = user.FirstName + " " + user.LastName,
						RoleID = (int)user.RoleID,
						IsMappingProject = false
                    };
                }
            }

			return resp;
        }
	}
}

