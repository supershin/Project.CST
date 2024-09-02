using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SummeryUnitModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class SummeryUnitFormRepo : ISummeryUnitFormRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public SummeryUnitFormRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public SummeryUnitFormDetailModel GetSummeryUnitDetail(SummeryUnitFormDetailModel model)
        {
            try
            {
                var result = (from t1 in _context.tm_Unit
                              where t1.UnitID == model.UnitID
                              join t2 in _context.tm_Project.Where(t2 => t2.FlagActive == true) on t1.ProjectID equals t2.ProjectID into projectJoin
                              from t2 in projectJoin.DefaultIfEmpty() 
                              join t3 in _context.tm_Ext.Where(t3 => t3.FlagActive == true) on t1.UnitStatusID equals t3.ID into extJoin
                              from t3 in extJoin.DefaultIfEmpty() 
                              //join t4 in _context.tm_UnitType.Where(t4 => t4.FlagActive == true) on t1.UnitTypeID equals t4.ID into unitTypeJoin
                              //from t4 in unitTypeJoin.DefaultIfEmpty() 
                              //join t5 in _context.tm_ModelType.Where(t5 => t5.FlagActive == true) on t1.ModelTypeID equals t5.ID into modelTypeJoin
                              //from t5 in modelTypeJoin.DefaultIfEmpty() 
                              //join t6 in _context.tr_ProjectModelForm.Where(t6 => t6.FlagActive == true) on t1.ModelTypeID equals t6.ModelTypeID into projectModelFormJoin
                              //from t6 in projectModelFormJoin.DefaultIfEmpty() 
                              select new SummeryUnitFormDetailModel
                              {
                                  ProjectID = t1.ProjectID,
                                  ProjectsName = t2 != null ? t2.ProjectName : null,
                                  UnitID = t1.UnitID,
                                  UnitCode = t1.UnitCode,
                                  UnitStatusID = t1.UnitStatusID,
                                  UnitStatusName = t3 != null ? t3.Name : null,
                              }).FirstOrDefault(); 

             
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw; 
            }
        }

        public List<SummeryUnitForm> GetSummeryUnitFormList(SummeryUnitForm model)
        {
            try
            {
                var result = from t1 in _context.tm_Unit
                             where t1.UnitID == model.UnitID && t1.FlagActive == true
                             join t2 in _context.tr_ProjectModelForm.Where(f => f.FlagActive == true)
                             on new { t1.ModelTypeID, t1.ProjectID } equals new { t2.ModelTypeID, t2.ProjectID } into t2Join
                             from t2 in t2Join.DefaultIfEmpty()
                             join t3 in _context.tm_Form.Where(f => f.FlagActive == true)
                             on t2.FormTypeID equals t3.FormTypeID into t3Join
                             from t3 in t3Join.DefaultIfEmpty()
                             join t4 in _context.tr_UnitForm.Where(f => f.FlagActive == true)
                             on new { FormID = (int?)t3.ID, UnitID = (Guid?)t1.UnitID } equals new { FormID = (int?)t4.FormID, t4.UnitID } into t4Join
                             from t4 in t4Join.DefaultIfEmpty()
                             join t5 in (
                                 from uf in _context.vw_UnitForm_Action
                                 join qc in _context.vw_UnitQC_Action
                                 on new { uf.ProjectID, uf.UnitID, uf.FormID } equals new { qc.ProjectID, qc.UnitID, qc.FormID } into qcJoin
                                 from qc in qcJoin.DefaultIfEmpty()
                                 select new
                                 {
                                     uf.ProjectID,
                                     uf.UnitID,
                                     uf.FormID,
                                     uf.PE,
                                     uf.PM,
                                     uf.PassConditionStatus,
                                     uf.LockStatusID,
                                     qc.QC1,
                                     qc.QC2,
                                     qc.QC3,
                                     qc.QC4_1,
                                     qc.QC4_2,
                                     qc.QC5
                                 }
                             ) on new { UnitID = (Guid?)t1.UnitID, FormID = (int?)t4.FormID } equals new { t5.UnitID, t5.FormID } into t5Join
                             from t5 in t5Join.DefaultIfEmpty()
                             select new SummeryUnitForm
                             {
                                 projectId = t1.ProjectID,
                                 UnitID = t1.UnitID,
                                 UnitFormID = t4.ID,
                                 FormName = t3.Name,
                                 FormID = t3.ID,
                                 PE = t5.PE,
                                 QC1 = t5.QC1,
                                 QC2 = t5.QC2,
                                 QC3 = t5.QC3,
                                 QC4_1 = t5.QC4_1,
                                 QC4_2 = t5.QC4_2,
                                 QC5 = t5.QC5,
                                 PM = t5.PM,
                                 PassConditionStatus = t5.PassConditionStatus,
                                 LockStatusID = t5.LockStatusID
                             };

                var list = result.ToList();

                return list;
            }
            catch (Exception ex)
            {
                // Log the exception (you can replace this with your logging mechanism)
                Console.WriteLine(ex.Message);
                // Optionally, you can rethrow the exception or handle it as needed
                throw;
            }
        }

        //public List<SummeryUnitForm> GetSummeryUnitFormList(SummeryUnitForm model)
        //{
        //    try
        //    {
        //        var query = (from t1 in _context.tm_Unit
        //                     where t1.UnitID == model.UnitID && t1.FlagActive == true
        //                     join t2 in _context.tr_ProjectModelForm.Where(f => f.FlagActive == true)
        //                     on new { t1.ModelTypeID, t1.ProjectID } equals new { t2.ModelTypeID, t2.ProjectID } into t2Join
        //                     from t2 in t2Join.DefaultIfEmpty()
        //                     join t3 in _context.tm_Form.Where(f => f.FlagActive == true)
        //                     on t2.FormTypeID equals t3.FormTypeID into t3Join
        //                     from t3 in t3Join.DefaultIfEmpty()
        //                     join t4 in _context.tr_UnitForm.Where(f => f.FlagActive == true)
        //                     on new { FormID = (int?)t3.ID, UnitID = (Guid?)t1.UnitID } equals new { FormID = (int?)t4.FormID, t4.UnitID } into t4Join
        //                     from t4 in t4Join.DefaultIfEmpty()
        //                     join t5 in _context.tr_UnitFormAction.Where(t5 => t5.RoleID == 1)
        //                         on t4.ID equals t5.UnitFormID into unitFormActionPEJoin
        //                     from t5 in unitFormActionPEJoin.DefaultIfEmpty()
        //                     join t6 in _context.tr_UnitFormAction.Where(t6 => t6.RoleID == 2)
        //                         on t4.ID equals t6.UnitFormID into unitFormActionPMJoin
        //                     from t6 in unitFormActionPMJoin.DefaultIfEmpty()
        //                     join t7 in _context.tr_UnitFormAction.Where(t7 => t7.RoleID == 3)
        //                         on t4.ID equals t7.UnitFormID into unitFormActionPJMJoin
        //                     from t7 in unitFormActionPJMJoin.DefaultIfEmpty()
        //                     join TPC in
        //                         (from TbPC in _context.tr_UnitFormPassCondition
        //                          where TbPC.FlagActive == true
        //                          group TbPC by TbPC.UnitFormID into grouped
        //                          select new
        //                          {
        //                              UnitFormID = grouped.Key,
        //                              Cnt_PC = grouped.Count()
        //                          }) on t4.ID equals TPC.UnitFormID into passConditionJoin
        //                     from TPC in passConditionJoin.DefaultIfEmpty()
        //                     join TUnLockPC in
        //                         (from TbPC in _context.tr_UnitFormPassCondition
        //                          where TbPC.FlagActive == true && TbPC.LockStatusID == 8
        //                          group TbPC by TbPC.UnitFormID into grouped
        //                          select new
        //                          {
        //                              UnitFormID = grouped.Key,
        //                              Cnt_PCUnlock = grouped.Count()
        //                          }) on t4.ID equals TUnLockPC.UnitFormID into passConditionUnlockJoin
        //                     from TUnLockPC in passConditionUnlockJoin.DefaultIfEmpty()
        //                     join VMQC in _context.vw_UnitQC_Action
        //                         on new { t1.ProjectID, UnitID = (Guid?)t1.UnitID, FormID = (int?)t3.ID } equals new { VMQC.ProjectID, VMQC.UnitID, VMQC.FormID } into vmqcJoin
        //                     from VMQC in vmqcJoin.DefaultIfEmpty()
        //                     select new
        //                     {
        //                         t1.UnitID,
        //                         t1.UnitCode,
        //                         t1.ProjectID,
        //                         FormID = t3.ID,
        //                         FormName = t3.Name,
        //                         UnitFormID = t4.ID,
        //                         UnitFormStatus = t4.StatusID,
        //                         UnitFormActionPEID = t5.ID,
        //                         UnitFormActionPEActionType = t5.ActionType,
        //                         UnitFormActionPMID = t6.ID,
        //                         UnitFormActionPMStatusID = t6.StatusID,
        //                         UnitFormActionPMActionType = t6.ActionType,
        //                         UnitFormActionPJMID = t7.ID,
        //                         UnitFormActionPJMStatusID = t7.StatusID,
        //                         UnitFormActionPJMActionType = t7.ActionType,
        //                         Cnt_PC = TPC.Cnt_PC,
        //                         Cnt_PCUnlock = TUnLockPC.Cnt_PCUnlock,
        //                         QC1 = VMQC.QC1,
        //                         QC2 = VMQC.QC2,
        //                         QC3 = VMQC.QC3,
        //                         QC4_1 = VMQC.QC4_1,
        //                         QC4_2 = VMQC.QC4_2,
        //                         QC5 = VMQC.QC5
        //                     }).ToList();

        //        var formattedResult = query
        //            .Select(item => new SummeryUnitForm
        //            {
        //                projectId = item.ProjectID,
        //                UnitID = item.UnitID,
        //                UnitFormID = item.UnitFormID,
        //                FormID = item.FormID,
        //                FormName = item.FormName,
        //                PE = item.UnitFormActionPEActionType == "save" ? "warning" : item.UnitFormActionPEActionType == "submit" ? "success" : "secondary",
        //                QC1 = item.QC1,
        //                QC2 = item.QC2,
        //                QC3 = item.QC3,
        //                QC4_1 = item.QC4_1,
        //                QC4_2 = item.QC4_2,
        //                QC5 = item.QC5,
        //                PM = (item.UnitFormActionPMStatusID == 4 || item.UnitFormActionPMStatusID == 6) ? "success" : (item.UnitFormActionPMStatusID == 5 || item.UnitFormActionPMStatusID == 7) ? "danger" : "secondary",
        //                PassConditionStatus = (item.Cnt_PC > 0 && item.UnitFormActionPEActionType == "submit") ? "warning"
        //                                    : (item.Cnt_PC > 0 && item.UnitFormActionPMActionType == "submit" && item.UnitFormActionPMStatusID == 6) ? "primary"
        //                                    : (item.Cnt_PC > 0 && item.UnitFormActionPMActionType == "submit" && item.UnitFormActionPMStatusID == 7) ? "danger"
        //                                    : (item.Cnt_PC > 0 && item.UnitFormActionPJMActionType == "submit" && item.UnitFormActionPJMStatusID == 8) ? "success"
        //                                    : (item.Cnt_PC > 0 && item.UnitFormActionPJMActionType == "submit" && item.UnitFormActionPJMStatusID == 9) ? "danger"
        //                                    : "",
        //                LockStatusID = item.Cnt_PCUnlock
        //            })
        //            .ToList();

        //        return formattedResult;

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw;
        //    }
        //}
    }
}
