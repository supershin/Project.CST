using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class QC5CheckService : IQC5CheckService
    {
        private readonly IQC5CheckRepo _IQC5CheckRepo;

        public QC5CheckService(IQC5CheckRepo QC5CheckRepo)
        {
            _IQC5CheckRepo = QC5CheckRepo;
        }

        public QC5MaxSeqStatusChecklistModel CheckQC5MaxSeqStatusChecklist(QC5MaxSeqStatusChecklistModel filterData)
        {
            var C5MaxSeqStatusChecklist = _IQC5CheckRepo.CheckQC5MaxSeqStatusChecklist(filterData);
            return C5MaxSeqStatusChecklist;
        }

        public QC5DetailModel GetQC5CheckDetail(QC5DetailModel filterData)
        {
            var QC5CheckDetail = _IQC5CheckRepo.GetQC5CheckDetail(filterData);
            return QC5CheckDetail;
        }

        public List<QC5ChecklistModel> GetQCUnitCheckListDefects(QC5ChecklistModel filterData)
        {
            var QCUnitCheckListDefects = _IQC5CheckRepo.GetQCUnitCheckListDefects(filterData);
            return QCUnitCheckListDefects;
        }

        public QC5DefectModel GetQC5DefactEdit(QC5DefectModel filterData)
        {
            var QC5Defactdata = _IQC5CheckRepo.GetQC5DefactEdit(filterData);
            return QC5Defactdata;
        }

        public void InsertQCUnitCheckListDefect(QC5IUDModel model, Guid userid)
        {
            try
            {
                _IQC5CheckRepo.InsertQCUnitCheckListDefect(model, userid);
            }
            catch (Exception ex)
            {
                throw new Exception($"ผิดพลาด : {ex.Message}", ex);
            }
        }

        public void UpdateQCUnitCheckListDefect(QC5IUDModel model)
        {
            try
            {
                _IQC5CheckRepo.UpdateQCUnitCheckListDefect(model);
            }
            catch (Exception ex)
            {
                throw new Exception($"ผิดพลาด : {ex.Message}", ex);
            }
        }

        public void UpdateDetailQCUnitCheckListDefect(QC5IUDModel model)
        {
            try
            {
                _IQC5CheckRepo.UpdateDetailQCUnitCheckListDefect(model);
            }
            catch (Exception ex)
            {
                throw new Exception($"ผิดพลาด : {ex.Message}", ex);
            }
        }

        public void RemoveQCUnitCheckListDefect(QC5IUDModel model)
        {
            try
            {
                _IQC5CheckRepo.RemoveQCUnitCheckListDefect(model);
            }
            catch (Exception ex)
            {
                throw new Exception($"ผิดพลาด : {ex.Message}", ex);
            }
        }

        public void RemoveImage(Guid resourceId, Guid UserID)
        {
            try
            {
                _IQC5CheckRepo.RemoveImage(resourceId, UserID);
            }
            catch (Exception ex)
            {
                throw new Exception($"ผิดพลาด : {ex.Message}", ex);
            }
        }

        public void SaveSubmitQC5UnitCheckList(QC5SaveSubmitModel model)
        {
            try
            {
                _IQC5CheckRepo.SaveSubmitQC5UnitCheckList(model);
            }
            catch (Exception ex)
            {
                throw new Exception($"ผิดพลาด : {ex.Message}", ex);
            }
        }

        public (string filePath, string currentDate) SaveSignature(SignatureQC5 signData, string? appPath, Guid? QCUnitCheckListID, Guid? userID)
        {
            try
            {
                var (filePath, currentDate) = _IQC5CheckRepo.SaveSignature(signData, appPath, QCUnitCheckListID, userID);
                return (filePath, currentDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}", ex);
            }
        }

        public void SelectedQCUnitCheckListDefectStatus(QC5IUDModel model)
        {
            try
            {
                _IQC5CheckRepo.SelectedQCUnitCheckListDefectStatus(model);
            }
            catch (Exception ex)
            {
                throw new Exception($"ผิดพลาด : {ex.Message}", ex);
            }
        }

        public SummaryQCPdfData GetSummaryQC5(Guid QCUnitCheckListID)
        {
            var DataSummaryQC5 = _IQC5CheckRepo.GetSummaryQC5(QCUnitCheckListID);
            return DataSummaryQC5;
        }

    }
}
