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
            switch (Model.Act)
            {
                case "Ext":
                    var extQuery = from ext in _context.tm_Ext
                                   where ext.ExtTypeID == Model.ID
                                   orderby ext.LineOrder
                                   select new GetDDL
                                   {
                                       Value = ext.ID,
                                       Text = ext.Name
                                   };

                    return extQuery.ToList();

                case "Vender":
                    var vendorQuery = from Vendor in _context.tm_Vendor
                                      where Vendor.FlagActive == true
                                      orderby Vendor.ID
                                      select new GetDDL
                                      {
                                          Value = Vendor.ID,
                                          Text = Vendor.Name
                                      };

                    return vendorQuery.ToList();

                default:

                return new List<GetDDL>();
            }
        }
    }
}
