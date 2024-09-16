using static Project.ConstructionTracking.Web.Models.FormCheckListModel;

namespace Project.ConstructionTracking.Web.Models.QC5CheckModel
{
    public class QC5ChecklistModel
    {
        public Guid? QC5UnitChecklistID { get; set; }
        public int? QC5UnitChecklistDefectID { get; set; }
        public int? Seq { get; set; }
        public int? DefectAreaID { get; set; }
        public string? DefectAreaName { get; set; }
        public int? DefectTypeID { get; set; }
        public string? DefectTypeName { get; set; }
        public int? DefectDescriptionID { get; set; }
        public string? DefectDescriptionName { get; set; }
        public string? Remark { get; set; }
        public int? StatusID { get; set; }
        public string? StatusName { get; set; }
        public List<QC5RadioCheckListModel>? ListRadioChecklist { get; set; }
    }

    public class QC5RadioCheckListModel
    {
        public int? RadioCheckValue { get; set; }
        public string? RadioCheckText { get; set; }

    }
}
