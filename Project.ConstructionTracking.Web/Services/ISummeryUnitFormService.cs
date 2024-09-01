using Project.ConstructionTracking.Web.Models.SummeryUnitModel;

namespace Project.ConstructionTracking.Web.Services
{
    public interface ISummeryUnitFormService
    {
        List<SummeryUnitForm> GetSummeryUnitFormList(SummeryUnitForm Model);
    }
}
