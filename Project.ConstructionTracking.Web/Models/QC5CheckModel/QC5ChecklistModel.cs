using static Project.ConstructionTracking.Web.Models.FormCheckListModel;

namespace Project.ConstructionTracking.Web.Models.QC5CheckModel
{
    public class QC5ChecklistModel
    {
        public int ID { get; set; }
        public Guid? QCUnitCheckListID { get; set; }
        public int? Seq { get; set; }
        public int? DefectAreaID { get; set; }
        public string? DefectAreaName { get; set; }
        public int? DefectTypeID { get; set; }
        public string? DefectTypeName { get; set; }
        public int? DefectDescriptionID { get; set; }
        public string? DefectDescriptionName { get; set; }
        public int? StatusID { get; set; }
        public string? Remark { get; set; }
        public bool? IsMajorDefect { get; set; }
        public bool? FlagActive { get; set; }
        public List<QC5RadioCheckListModel>? ListRadioChecklist { get; set; }
    }

    public class QC5RadioCheckListModel
    {
        public int? RadioCheckValue { get; set; }
        public string? RadioCheckText { get; set; }

    }
}
