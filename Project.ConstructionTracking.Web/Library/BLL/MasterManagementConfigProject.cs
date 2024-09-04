using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;

namespace Project.ConstructionTracking.Web.Library.BLL
{
    public class MasterManagementConfigProject
    {
        public static List<ProjectModel> SP_Get_Project(ProjectModel en)
        {
            return SiteProvider.MastermanageProject.SP_Get_Project(en);
        }
        public static List<UnitStatusModel> sp_get_unitstatus(UnitStatusModel en)
        {
            return SiteProvider.MastermanageProject.sp_get_unitstatus(en);
        }
    }
}
