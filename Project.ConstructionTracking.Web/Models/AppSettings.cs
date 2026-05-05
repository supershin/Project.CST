using System;
namespace Project.ConstructionTracking.Web.Models
{
	public class AppSettings
	{
		public string PasswordKey { get; set; }
		public AppPortal AppPortal { get; set; }
	}
	public class AppPortal
	{
		public string Key { get; set; }
        public string IV { get; set; }
    }
}

