using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Services
{
    public interface ISummeryUnitFormService
    {
        List<SummeryUnitForm> GetSummeryUnitFormList(SummeryUnitForm Model);
    }
}
