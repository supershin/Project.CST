using Microsoft.EntityFrameworkCore;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class FormGroupRepo : IFormGroupRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public FormGroupRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<FormGroupModel> GetFormGroupList(FormGroupModel model)
        {
            var query = from t1 in _context.tm_FormGroup
                        where t1.FormID == model.FormID
                        select new FormGroupModel
                        {
                            GroupID = t1.ID,
                            FormID = t1.FormID,
                            GroupName = t1.Name,
                            Sort = t1.Sort,
                            FlagActive = t1.FlagActive
                        };

            return query.ToList();
        }
    }
}
