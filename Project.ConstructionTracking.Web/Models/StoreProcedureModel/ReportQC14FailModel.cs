namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportQC14FailModel
    {
        public string? act { get; set; }
        public string? project_id { get; set; }
        public string? unit_id { get; set; }
        public string? unit_status { get; set; }
        public string? build_status { get; set; }
        public string? vender_id { get; set; }
        public string? qctype_id { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }

        public int index { get; set; }
        public int? QCTypeID { get; set; }
        public string? QCTypeName { get; set; }
        public int? ChecklistID { get; set; }
        public int? ParentID { get; set; }
        public string? ChecklistName { get; set; }
        public string? AllQC { get; set; }
        public string? AllQCFail { get; set; }
        public string? QCPercentFail { get; set; }
        public string? CNTUnit { get; set; }

    }
}
