using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
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

        public List<SummeryUnitForm> GetSummeryUnitFormList(SummeryUnitForm model)
        {
            try
            {
                var result = from t1 in _context.tm_Unit
                             where t1.UnitID == model.UnitID
                join t2 in _context.tr_ProjectModelForm
                             on new { t1.ModelTypeID, t1.ProjectID } equals new { t2.ModelTypeID , t2.ProjectID } into t2Join
                             from t2 in t2Join.DefaultIfEmpty()
                             join t3 in _context.tm_Form
                             on t2.FormTypeID equals t3.FormTypeID into t3Join
                             from t3 in t3Join.DefaultIfEmpty()
                             join t4 in _context.tr_UnitForm
                             on new { FormID =(int?) t3.ID, UnitID=(Guid?)t1.UnitID } equals new { FormID = (int?)t4.FormID, t4.UnitID } into t4Join
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
                                 UnitFormName = t3.Name,
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
    }
}
