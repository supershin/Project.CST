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
                    var vendorQuery = from t1 in _context.tr_CompanyVendorProject
                                      join t2 in _context.tm_CompanyVendor.Where(cv => cv.FlagActive == true)
                                      on t1.CompanyVendorID equals t2.ID into t2Joins
                                      from t2 in t2Joins.DefaultIfEmpty()

                                      join t3 in _context.tr_CompanyVendor.Where(cv => cv.FlagActive == true)
                                      on t1.CompanyVendorID equals t3.CompanyVendorID into t3Joins
                                      from t3 in t3Joins.DefaultIfEmpty()

                                      join t4 in _context.tm_Vendor
                                      on t3.VendorID equals t4.ID into t4Joins
                                      from t4 in t4Joins.DefaultIfEmpty()

                                      where t1.ProjectID == Guid.Parse("3ED05DB5-C3C7-4CC0-A98B-169EA8489CF4")
                                            && t1.FlagActive == true
                                      orderby t3.VendorID
                                      select new GetDDL
                                      {
                                          Value = t3.VendorID,
                                          Text = t2.Name + " / " + t4.Name
                                      };

                    return vendorQuery.ToList();


                default:

                return new List<GetDDL>();
            }
        }
    }
}
