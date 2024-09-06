namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class PEMyTaskModel
    {
        public int index { get; set; }
        public string? act { get; set; }
        public string? unit_id { get; set; }
        public string? project_id { get; set; }
        public string? unit_status { get; set; }
        public string? user_id { get; set; }
        public Guid ID { get; set; } 
        public string? UnitCode { get; set; } 
        public string? ProjectName { get; set; } 
        public string? Grade { get; set; } 
        public int FormID { get; set; } 
        public string? FormName { get; set; }
        public int StatusID { get; set; } 
        public string? StatusDescription { get; set; } 
        public string? StatusIcon { get; set; }
        public string? StatusColor { get; set; } 
        public string? PMActionBy { get; set; } 
        public string? PMActionDate { get; set; } 
        public string? PJMActionBy { get; set; } 
        public string? PJMActionDate { get; set; } 
        public string? PassConditionIcon { get; set; } 
        public string? PassConditionColor { get; set; } 
    }
}
