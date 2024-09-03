namespace Project.ConstructionTracking.Web.Library.DAL
{
    public class SiteProvider
    {
        public static MasterManagementProviderProject MastermanageProject
        {
            get { return MasterManagementProviderProject.Instance; }
        }
    }
}
