using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class GetDDLRepo : IGetDDLRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public GetDDLRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<GetDDL> GetDDLList(GetDDL Model)
        {

            if (Model.Act == "Ext")
            {
                var query = from ext in _context.tm_Ext
                            where ext.ExtTypeID == Model.ID
                            orderby ext.LineOrder
                            select new GetDDL
                            {
                                Value = ext.ID,
                                Text = ext.Name
                            };

                return query.ToList();
            }

            return new List<GetDDL>();
        }
    }
}
