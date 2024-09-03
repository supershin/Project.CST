using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Library.BLL
{
    public class MasterManagementConfigProject
    {
        public static List<ProjectModel> SP_Get_Project(ProjectModel en)
        {
            return SiteProvider.MastermanageProject.SP_Get_Project(en);
        }
    }
}
