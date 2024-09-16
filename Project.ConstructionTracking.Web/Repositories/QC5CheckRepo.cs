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

        public List<QC5ChecklistModel> GetQCUnitCheckListDefects(QC5ChecklistModel filterData)
        {
            var result = (from t1 in _context.tr_QC_UnitCheckList_Defect
                          join t2 in _context.tm_DefectArea on t1.DefectAreaID equals t2.ID into defectAreaGroup
                          from t2 in defectAreaGroup.DefaultIfEmpty()
                          join t3 in _context.tm_DefectType on t1.DefectTypeID equals t3.ID into defectTypeGroup
                          from t3 in defectTypeGroup.DefaultIfEmpty()
                          join t4 in _context.tm_DefectDescription on t1.DefectDescriptionID equals t4.ID into defectDescriptionGroup
                          from t4 in defectDescriptionGroup.DefaultIfEmpty()
                          select new QC5ChecklistModel
                          {
                              ID = t1.ID,
                              QCUnitCheckListID = t1.QCUnitCheckListID,
                              Seq = t1.Seq,
                              DefectAreaID = t1.DefectAreaID,
                              DefectAreaName = t2.Name,
                              DefectTypeID = t1.DefectTypeID,
                              DefectTypeName = t3.Name,
                              DefectDescriptionID = t1.DefectDescriptionID,
                              DefectDescriptionName = t4.Name,
                              StatusID = t1.StatusID,
                              Remark = t1.Remark,
                              FlagActive = t1.FlagActive,
      
                              ListRadioChecklist = (from rc in _context.tm_Ext
                                                    where rc.ExtTypeID == SystemConstant.Ext_Type.QC5RadioChecklist
                                                    select new QC5RadioCheckListModel
                                                    {
                                                        RadioCheckValue = rc.ID, 
                                                        RadioCheckText = rc.Name  
                                                    }).ToList()
                          }).ToList();

            return result;
        }

        public void InsertQCUnitCheckListDefect(QC5IUDModel model)
        {
            var newDefect = new tr_QC_UnitCheckList_Defect
            {
                QCUnitCheckListID = model.QCUnitCheckListID,
                Seq = model.Seq,
                DefectAreaID = model.DefectAreaID,
                DefectTypeID = model.DefectTypeID,
                DefectDescriptionID = model.DefectDescriptionID,
                StatusID = model.StatusID,
                Remark = model.Remark,
                FlagActive = true,
                CreateDate = DateTime.Now, 
                CreateBy = model.UserID,
                UpdateDate = DateTime.Now,
                UpdateBy = model.UserID,               
            };

            _context.tr_QC_UnitCheckList_Defect.Add(newDefect);
            _context.SaveChanges();
        }

        public void UpdateQCUnitCheckListDefect(QC5IUDModel model)
        {

            var existingDefect = _context.tr_QC_UnitCheckList_Defect.FirstOrDefault(d => d.ID == model.ID);

            if (existingDefect != null)
            {
                existingDefect.QCUnitCheckListID = model.QCUnitCheckListID;
                existingDefect.Seq = model.Seq;
                existingDefect.DefectAreaID = model.DefectAreaID;
                existingDefect.DefectTypeID = model.DefectTypeID;
                existingDefect.DefectDescriptionID = model.DefectDescriptionID;
                existingDefect.StatusID = model.StatusID;
                existingDefect.Remark = model.Remark;
                existingDefect.UpdateDate = DateTime.Now; 
                existingDefect.UpdateBy = model.UserID;

                _context.SaveChanges();
            }
        }

        public void RemoveQCUnitCheckListDefect(QC5IUDModel model)
        {
            var existingDefect = _context.tr_QC_UnitCheckList_Defect.FirstOrDefault(d => d.ID == model.ID);

            if (existingDefect != null)
            {
                existingDefect.FlagActive = false;
                existingDefect.UpdateDate = DateTime.Now;
                existingDefect.UpdateBy = model.UserID;

                _context.SaveChanges();
            }
        }


    }
}
