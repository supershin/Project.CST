namespace Project.ConstructionTracking.Web.Models.StoreProcedureModel
{
    public class WorkPeriodModel
    {
        public int index { get; set; }
        public string? act { get; set; }
        public string? unit_id { get; set; }
        public string? project_id { get; set; }
        public string? unit_status { get; set; }
        public string? user_id { get; set; }


        public string? ProjectName { get; set; }         
        public string? UnitCode { get; set; }           
        public string? FormName { get; set; }      
        public string? CompanyVendorName { get; set; } 
        public string? PONo { get; set; }       
        public string? ApproveDate { get; set; }         
        public string? UnitFormPDF { get; set; }      
        public string? QCPDF { get; set; }         
        public string? GRNo { get; set; } 
        public string? GRDate { get; set; }
        public Guid? UnitFormID { get; set; }
        public string? Statusworkperiod { get; set; }
    }
}
