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
            var query = from t1 in _context.tm_FormGroup.Where(g => g.FlagActive == true)
                        where t1.FormID == model.FormID
                        join t2 in _context.tm_FormPackage.Where(p => p.FlagActive == true) on t1.ID equals t2.GroupID into t2Group
                        from t2 in t2Group.DefaultIfEmpty()
                        join t3 in _context.tm_FormCheckList.Where(c => c.FlagActive == true) on t2.ID equals t3.PackageID into t3Group
                        from t3 in t3Group.DefaultIfEmpty()
                        join t4 in _context.tr_UnitFormCheckList.Where(f => f.FlagActive == true) on new { t1.FormID, PackageID = t2.ID, CheckListID = t3.ID, model.UnitFormID } equals new { t4.FormID, t4.PackageID, t4.CheckListID, t4.UnitFormID } into t4Group
                        from t4 in t4Group.Where(t => t.StatusID != null).DefaultIfEmpty()
                        join t5 in _context.tr_UnitFormCheckList.Where(f => f.FlagActive == true) on new { t1.FormID, PackageID = t2.ID, CheckListID = t3.ID, model.UnitFormID } equals new { t5.FormID, t5.PackageID, t5.CheckListID, t5.UnitFormID } into t5Group
                        from t5 in t5Group.Where(t => t.StatusID == 2).DefaultIfEmpty()
                        join t6 in _context.tr_UnitFormPassCondition.Where(p => p.FlagActive == true) on new { t4.UnitFormID, t4.GroupID } equals new { t6.UnitFormID, t6.GroupID } into t6Group
                        from t6 in t6Group.DefaultIfEmpty()
                        group new { t3, t4, t5, t6 } by new { t1.ID, t1.FormID, t1.Name, t6.LockStatusID } into g
                        select new FormGroupModel
                        {
                            GroupID = g.Key.ID,
                            FormID = g.Key.FormID,
                            GroupName = g.Key.Name,
                            LockStatusID = g.Key.LockStatusID,
                            Cnt_CheckList_All = g.Count(x => x.t3 != null),
                            Cnt_CheckList_Unit = g.Count(x => x.t4 != null),
                            Cnt_CheckList_NotPass = g.Count(x => x.t5 != null),
                            StatusUse = g.Count(x => x.t5 != null) > 0 && g.Count(x => x.t3 != null) == g.Count(x => x.t4 != null) ? "danger" :
                                        g.Count(x => x.t4 != null) == 0 ? "secondary" :
                                        g.Count(x => x.t3 != null) == g.Count(x => x.t4 != null) ? "success" :
                                        g.Count(x => x.t3 != null) > g.Count(x => x.t4 != null) ? "warning" : ""
                        };

            var orderedQuery = query.OrderBy(fg => fg.GroupID).ToList();

            //var query = from t0 in _context.tm_Form
            //            where t0.ID == model.FormID
            //            join t1 in _context.tm_FormGroup on t0.ID equals t1.FormID into t1Group
            //            from t1 in t1Group.DefaultIfEmpty()
            //            join t2 in _context.tm_FormPackage on t1.ID equals t2.GroupID into t2Group
            //            from t2 in t2Group.DefaultIfEmpty()
            //            join t3 in _context.tm_FormCheckList on t2.ID equals t3.PackageID into t3Group
            //            from t3 in t3Group.DefaultIfEmpty()
            //            join tUnitForm in _context.tr_UnitForm on t0.ID equals tUnitForm.FormID into tUnitFGroup
            //            from tUnitForm in tUnitFGroup.Where(t => t.UnitID == model.UnitID).DefaultIfEmpty()
            //            join t4 in _context.tr_UnitFormCheckList on new { t1.FormID, PackageID = t2.ID, CheckListID = t3.ID } equals new { t4.FormID, t4.PackageID, t4.CheckListID } into t4Group
            //            from t4 in t4Group.Where(t => t.StatusID != null && t.UnitFormID == model.UnitFormID).DefaultIfEmpty()
            //            join t5 in _context.tr_UnitFormCheckList on new { t1.FormID, PackageID = t2.ID, CheckListID = t3.ID } equals new { t5.FormID, t5.PackageID, t5.CheckListID } into t5Group
            //            from t5 in t5Group.Where(t => t.StatusID == 2 && t.UnitFormID == model.UnitFormID).DefaultIfEmpty()
            //            join t6 in _context.tr_UnitFormPassCondition on new { t4.UnitFormID, t4.GroupID } equals new { t6.UnitFormID, t6.GroupID } into t6Group
            //            from t6 in t6Group.DefaultIfEmpty()
            //            group new { t0, t1, t2, t3, t4, t5, tUnitForm, t6 } by new { t0.ID, t0.Name, GroupID = t1.ID, GroupName = t1.Name, tUnitForm.UnitID, t6.LockStatusID } into g
            //            select new FormGroupModel
            //            {
            //                FormID = g.Key.ID,
            //                UnitID = g.Key.UnitID,
            //                GroupID = g.Key.GroupID,
            //                GroupName = g.Key.GroupName,
            //                LockStatusID = g.Key.LockStatusID,
            //                Cnt_CheckList_All = g.Count(x => x.t3 != null),
            //                Cnt_CheckList_Unit = g.Count(x => x.t4 != null),
            //                Cnt_CheckList_NotPass = g.Count(x => x.t5 != null),
            //                StatusUse = g.Count(x => x.t5 != null) > 0 && g.Count(x => x.t3 != null) == g.Count(x => x.t4 != null) ? "danger" :
            //                                            g.Count(x => x.t4 != null) == 0 ? "secondary" :
            //                                            g.Count(x => x.t3 != null) == g.Count(x => x.t4 != null) ? "success" :
            //                                            g.Count(x => x.t3 != null) > g.Count(x => x.t4 != null) ? "warning" : ""
            //            };

            //var result = query.ToList();
            return orderedQuery;

        }

    }
}
