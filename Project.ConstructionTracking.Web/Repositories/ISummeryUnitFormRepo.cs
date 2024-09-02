using Project.ConstructionTracking.Web.Models.SummeryUnitModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface ISummeryUnitFormRepo
    {
        List<SummeryUnitForm> GetSummeryUnitFormList(SummeryUnitForm Model);
    }
}
