using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class UnLockPassConditionRepo : IUnLockPassConditionRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public UnLockPassConditionRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<UnLockPassConditionModel.GetDataUnlockPC> GetListUnlockPC(UnLockPassConditionModel.GetDataUnlockPC filterData)
        {
                var result = (from t1 in _context.tr_UnitFormPassCondition
                              join t2 in _context.tr_UnitForm on t1.UnitFormID equals t2.ID into unitForms
                              from t2 in unitForms.DefaultIfEmpty()
                              join t3 in _context.tm_Project on t2.ProjectID equals t3.ProjectID into projects
                              from t3 in projects.DefaultIfEmpty()
                              join t4 in _context.tm_Unit on t2.UnitID equals t4.UnitID into units
                              from t4 in units.DefaultIfEmpty()
                              join t5 in _context.tm_Form on t2.FormID equals t5.ID into forms
                              from t5 in forms.DefaultIfEmpty()
                              join t6 in _context.tm_Vendor on t2.VendorID equals t6.ID into vendors
                              from t6 in vendors.DefaultIfEmpty()
                              where t1.UnitFormID == filterData.UnitFormID  && (filterData.GroupID == -1 || t1.GroupID == filterData.GroupID)
                              select new UnLockPassConditionModel.GetDataUnlockPC
                              {
                                  PC_ID = t1.ID,
                                  ProjectName = t3.ProjectName,
                                  UnitCode = t4.UnitCode,
                                  FormName = t5.Name,
                                  VenderName = t6.Name,
                                  UnitFormID = t1.UnitFormID,
                                  GroupID = t1.GroupID,
                                  LockStatusID = t1.LockStatusID,
                                  StatusID = t1.StatusID,
                                  PE_Remark = t1.PE_Remark,
                                  PM_Remark = t1.PM_Remark,
                                  PJM_Remark = t1.PJM_Remark,
                                  PEUnLock_Remark = t1.PEUnLock_Remark,
                                  PMUnLock_Remark = t1.PMUnLock_Remark
                              }).ToList();

                return result;        
        }
    }
}
