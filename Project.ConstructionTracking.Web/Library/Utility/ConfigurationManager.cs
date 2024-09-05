namespace Project.ConstructionTracking.Web.Library.Utility
{
    public class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }

        static ConfigurationManager()
        {

            string env = "";

            if (env.ToUpper() == "DEVELOPMENT")
            {


                AppSetting = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings." + env + ".json")
                  .Build();
            }
            else
            {   //production
                AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
            }
        }
    }
}
