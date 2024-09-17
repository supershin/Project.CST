﻿using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IQC5CheckRepo
    {
        QC5DetailModel GetQC5CheckDetail(QC5DetailModel filterData);
        List<QC5ChecklistModel> GetQCUnitCheckListDefects(QC5ChecklistModel filterData);
        void InsertQCUnitCheckListDefect(QC5IUDModel model ,Guid userid);
        void UpdateQCUnitCheckListDefect(QC5IUDModel model);
        void RemoveQCUnitCheckListDefect(QC5IUDModel model);
    }
}
