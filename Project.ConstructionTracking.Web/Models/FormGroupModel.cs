using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Project.ConstructionTracking.Web.Models
{
    public class FormGroupModel
    {
        public Guid? UnitID { get; set; }
        public int? GroupID { get; set; }
        public int? FormID { get; set; }
        public Guid? UnitFormID { get; set; }
        public string? GroupName { get; set; }
        public string? StatusUse { get; set;}
        public string? FormGrade { get; set; }
        public int? LockStatusID { get; set; }
        public int? StatusID { get; set; }
        public string? ActionType { get; set; }
        public int? Cnt_CheckList_All { get; set; }
        public int? Cnt_CheckList_Pass { get; set; }
        public int? Cnt_CheckList_NotPass { get; set; }

        public class FormGroupIUDModel
        {
            public Guid? userID { get; set; }
            public int? RoleID { get; set; }
            public string? Act { get; set; }
            public string? FormGrade { get; set; }
            public int? FormID { get; set; }
            public Guid? ProjectID { get; set; }
            public SignatureData? Sign { get; set; }
            public Guid? UnitFormID { get; set; }
            public Guid? UnitID { get; set; }
            public int? VendorID { get; set; }
            public string? ApplicationPath { get; set; }      

        }

        public class SignatureData
        {
            public string? MimeType { get; set; }
            public string? StorageBase64 { get; set; }

        }

        public class Resources
        {
            public string? MimeType { get; set; }
            public string? ResourceStorageBase64 { get; set; }
            public string? PhysicalPathServer { get; set; }
            public string? ResourceStoragePath { get; set; }
            public string? Directory { get; set; }
        }

        public class FormGroupDetail
        {
            public Guid ID { get; set; }
            public string? Grade { get; set; }
            public int? FormID { get; set; }
            public string? PE_ActionType { get; set; }
            public int? PE_StatusID { get; set; }
            public string? PM_ActionType { get; set; }
            public int? PM_StatusID { get; set; }
            public string? PJM_ActionType { get; set; }
            public int? PJM_StatusID { get; set; }
            public string? FilePath { get; set; }
            public string? FileName { get; set; }

        }
    }
}
