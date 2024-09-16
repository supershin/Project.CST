using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class QC5CheckRepo : IQC5CheckRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public QC5CheckRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public QC5DetailModel GetQC5CheckDetail(QC5DetailModel filterData)
        {
            var query = (from t1 in _context.tm_Project
                         join t2 in _context.tm_Unit on t1.ProjectID equals t2.ProjectID into unitGroup
                         from t2 in unitGroup.DefaultIfEmpty()
                         join t3 in _context.tm_Ext on t2.UnitStatusID equals t3.ID into extGroup
                         from t3 in extGroup.DefaultIfEmpty()
                         join t4 in _context.tr_QC_UnitCheckList on new { ProjectID = (Guid?)t1.ProjectID, UnitID = (Guid?)t2.UnitID , filterData.Seq } equals new { t4.ProjectID, t4.UnitID ,t4.Seq } into unitCheckListGroup
                         from t4 in unitCheckListGroup.DefaultIfEmpty()
                         join t5 in _context.tr_QC_UnitCheckList_Action on t4.ID equals t5.QCUnitCheckListID into actionGroup
                         from t5 in actionGroup.DefaultIfEmpty()
                         join t6 in _context.tm_User on t5.UpdateBy equals t6.ID into Users
                         from t6 in Users.DefaultIfEmpty()
                         where t1.ProjectID == filterData.ProjectID
                               && t2.UnitID == filterData.UnitID
                         select new QC5DetailModel
                         {
                             ProjectID = t1.ProjectID,
                             ProjectsName = t1.ProjectName,
                             UnitID = t2.UnitID,
                             UnitCode = t2.UnitCode,
                             UnitStatusName = t3.Name,
                             QC5UnitChecklistID = t4.ID,
                             QC5UnitStatusID = t4.QCStatusID,
                             QC5UpdateDate = FormatExtension.FormatDateToDayMonthNameYearTime(t4.UpdateDate),
                             QC5UpdateByName = t6.FirstName + ' ' + t6.LastName,
                             Seq = t4.Seq,
                             ActionType = t5.ActionType
                         }).FirstOrDefault(); 

            return query;
        }

    }
}
