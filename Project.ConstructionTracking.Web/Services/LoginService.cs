using System;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface ILoginService
	{
		LoginResp VerifyLogin(string username, string password, string key);
    }
	public class LoginService : ILoginService
	{
		private readonly ILoginRepo _loginRepo;
		public LoginService(ILoginRepo loginRepo)
		{
			_loginRepo = loginRepo;
		}

		public LoginResp VerifyLogin(string username, string password, string key)
		{
			var resp = _loginRepo.VerifyLogin(username, password, key);
			return resp;
		}
	}
}

