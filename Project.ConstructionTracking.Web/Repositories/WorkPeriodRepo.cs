using Microsoft.CodeAnalysis.Differencing;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class WorkPeriodRepo : IWorkPeriodRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public WorkPeriodRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public void UpdateGrUnitForm(Guid UnitFormID , string GRNo , Guid UserID)
        {
            var unitForm = _context.tr_UnitForm.Where(uf => uf.ID == UnitFormID).FirstOrDefault();
            if (unitForm != null)
            {
                unitForm.GRNo = GRNo;
                unitForm.GRDate = DateTime.Now;
                unitForm.UpdateBy = UserID;
                _context.tr_UnitForm.Update(unitForm);
            }            
            _context.SaveChanges();
        }
    }
}
