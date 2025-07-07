using DocumentFormat.OpenXml.Spreadsheet;

namespace Project.ConstructionTracking.Web.Models.WebAPIRest
{
    public class RequestPostModel
    {
        public class GRVenderrportal
        {
            public class Sends
            {
                public string? grno { get; set; }
                public string? pono { get; set; }
                public string? remark { get; set; }
                public List<IFormFile> fileData { get; set; } = new List<IFormFile>();
            }
            public class Responds
            {
                public int? status { get; set; }
                public string? message { get; set; }
            }
        }
        public class Get_User_CRM
        {
            public class Sends
            {
                public string? email { get; set; }
            }

            public class Responds
            {
                public int? status { get; set; }
                public string? message { get; set; }
                public string? UserID { get; set; }
            }

            public class CentralizeUserResponse
            {
                public bool Success { get; set; }
                public string? Message { get; set; }
                public List<CentralizeUser>? Data { get; set; }
            }

            public class CentralizeUser
            {
                public string? UserID { get; set; }
                public string? Username { get; set; }
                public string? FirstName { get; set; }
                public string? LastName { get; set; }
                public string? Email { get; set; }
                public string? EmployeeID { get; set; }
                public string? DepartmentID { get; set; }
                public string? DepartmentName { get; set; }
            }
        }
        public class QC_Status_Update_QC5 
        {
            public class Sends
            {
                public string? project_code { get; set; }
                public Guid? project_id { get; set; }
                public string? unit_number { get; set; }
                public Guid? unit_id { get; set; }
                public int? sync_type { get; set; }
                public DateTime? submit_date { get; set; }
                public string? contractor_appointment_date { get; set; }
                public string? contractor_appointment_timeStart { get; set; }
                public string? contractor_appointment_timeEnd { get; set; }
                public string? qc_response_user_id { get; set; }
                public string? qc_response_date { get; set; }
                public string? qc_remark { get; set; }
                public string? qc_type { get; set; }
                public Guid? CQTUserID { get; set; }
            }
            public class Responds
            {
                public int? Status { get; set; }
                public string? message { get; set; }
            }
            public class Getdetail
            {
                public Guid? UnitID { get; set; }
                public int? QCTypeID { get; set; }
                public string? QCAppointDate { get; set; }
                public string? QCAppointTimeFrom { get; set; }
                public string? QCAppointTimeTo { get; set; }
                public string? QCResponseDate { get; set; }
                public int? SyncType { get; set; }
                public string? SubmitBy { get; set; }
                public string? QCRemark { get; set; }
            }
        }
    }
}
