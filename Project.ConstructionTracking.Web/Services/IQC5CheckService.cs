﻿using Project.ConstructionTracking.Web.Models.QC5CheckModel;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IQC5CheckService
    {
        QC5DetailModel GetQC5CheckDetail(QC5DetailModel filterData);
        List<QC5ChecklistModel> GetQCUnitCheckListDefects(QC5ChecklistModel filterData);
        void InsertQCUnitCheckListDefect(QC5IUDModel model);
        void UpdateQCUnitCheckListDefect(QC5IUDModel model);
        void RemoveQCUnitCheckListDefect(QC5IUDModel model);
    }
}
