using System;
using System.Collections.Generic;

namespace Project.ConstructionTracking.Web.Data
{
    public partial class TrCompanyVendor
    {
        public int Id { get; set; }
        public int? CompanyVendorId { get; set; }
        public int? VendorId { get; set; }
        public bool? FlagActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual TmCompanyVendor? CompanyVendor { get; set; }
        public virtual TmVendor? Vendor { get; set; }
    }
}
