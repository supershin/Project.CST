using static Project.ConstructionTracking.Web.Models.FormCheckListModel;

namespace Project.ConstructionTracking.Web.Models
{
    public class UnLockPassConditionModel
    {
        public class GetDataUnlockPC
        {
            public int PC_ID { get; set; }
            public Guid? ProjectID { get; set; }
            public string? ProjectName { get; set; }
            public Guid? UnitID { get; set; }
            public string? UnitCode { get; set; }
            public int? FormID { get; set; }
            public string? FormName { get; set; }
            public string? VenderName { get; set; }
            public Guid? UnitFormID { get; set; }
            public int? GroupID { get; set; }
            public string? GroupName { get; set; }
            public int? LockStatusID { get; set; }
            public int? StatusID { get; set; }
            public string? PE_Remark { get; set; }
            public string? PM_Remark { get; set; }
            public string? PJM_Remark { get; set; }
            public string? PEUnLock_Remark { get; set; }
            public string? PMUnLock_Remark { get; set; }
            public List<GetImageUnlock>? listImageUnlock { get; set; }

        }

        public class GetImageUnlock
        {
            public int UnitFormResourceID { get; set; }
            public Guid? ResourceID { get; set; }
            public Guid MasterResourceID { get; set; }
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public int RoleID { get; set; }
            public Guid? UnitFormID { get; set; }
            public int? GroupID { get; set; }
            public int? PC_ID { get; set; }
        }

        public class UpdateUnlockPC
        {
            public int PC_ID { get; set; }
            public Guid? UnitFormID { get; set; }
            public Guid? UserID { get; set; }
            public int? RoleID { get; set; }
            public int? GroupID { get; set; }
            public string? ApplicationPath { get; set; }
            public string? PEUnLock_Remark { get; set; }
            public string? PMUnLock_Remark { get; set; }
            public string? Action { get; set; }
            public List<IFormFile>? Images { get; set; }
        }
    }
}
