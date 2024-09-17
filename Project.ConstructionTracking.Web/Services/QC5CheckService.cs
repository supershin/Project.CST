using Project.ConstructionTracking.Web.Models;
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

        public void InsertQCUnitCheckListDefect(QC5IUDModel model)
        {
            try
            {
                _IQC5CheckRepo.InsertQCUnitCheckListDefect(model);
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


    }
}
