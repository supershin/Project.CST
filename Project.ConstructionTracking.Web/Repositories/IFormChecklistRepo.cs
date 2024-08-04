using Project.ConstructionTracking.Web.Models;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IFormChecklistRepo
    {
        List<FormCheckListModel.Form_getListPackages> GetFormCheckList(FormCheckListModel.Form_getFilterData filterData);
        List<FormCheckListModel.Form_getListStatus> GetFormCheckListStatus(FormCheckListModel.Form_getFilterData filterData);
        void InsertOrUpdate(FormChecklistIUDModel model);
    }
}
