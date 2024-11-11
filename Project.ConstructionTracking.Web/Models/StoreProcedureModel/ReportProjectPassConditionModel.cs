namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class ReportProjectPassConditionModel
    {
        public string? act { get; set; }
        public string? project_id { get; set; }
        public string? unit_id { get; set; }
        public string? unit_status { get; set; }
        public string? build_status { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }

        public int index { get; set; }
        public string? UnitCode { get; set; }
        public string? FormName { get; set; }
        public string? FormGroupName { get; set; }
        public string? PEStatusIDName { get; set; }
        public string? PEActionName { get; set; }
        public string? PEActionDate { get; set; }
        public string? PEPCRemark { get; set; }
        public string? PMStatusIDName { get; set; }
        public string? PMActionName { get; set; }
        public string? PMActionDate { get; set; }
        public string? PMPCRemark { get; set; }
        public string? PJMStatusIDName { get; set; }
        public string? PJMActionName { get; set; }
        public string? PJMActionDate { get; set; }
        public string? PJMPCRemark { get; set; }
        public string? PERequestUnlock { get; set; }
        public string? PMUnlock { get; set; }
        public string? PCStatusName { get; set; }
    }
}
