using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class FormOverallRepo : IFormOverallRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public FormOverallRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<ProjectFormList> ProjectFormLists(Guid formId, int typeId)
        {
            var ProjectFormList = new List<ProjectFormList>();

            //var query = (from u in _context.tr_ProjectForm.Where(e => e.ProjectID == formId && e.UnitTypeID == typeId)
            //            select new
            //            {
            //                u.ID,
            //                u.Name,
            //                u.FlagActive
            //            }).ToList();

            //foreach ( var item in query)
            //{
            //    var data = new ProjectFormList()
            //    {
            //        Id = item.ID,
            //        Name = item.Name,
            //        Action = item.FlagActive
            //    };
            //    ProjectFormList.Add(data);
            //}

            return ProjectFormList;
        }
    }
}
