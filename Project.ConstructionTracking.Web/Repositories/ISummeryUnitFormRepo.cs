using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface ISummeryUnitFormRepo
    {
        List<SummeryUnitForm> GetSummeryUnitFormList(SummeryUnitForm Model);
    }
}
