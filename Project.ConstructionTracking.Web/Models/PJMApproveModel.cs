namespace Project.ConstructionTracking.Web.Models
{
    public class PJMApproveModel
    {
        public class filterData
        {
            public string? ActionType { get; set; }
            public int? StatusID { get; set; }
        }
        public class GetlistUnitDetail
        {
            public Guid? UnitFormID { get; set; }
            public int UnitFormActionID { get; set; }
            public string? UnitCode { get; set; }
            public string? ProjectName { get; set; }
            public int? VendorID { get; set; }
            public string? VendorName { get; set; }
            public int? FormID { get; set; }
            public string? FormName { get; set; }
            public int? RoleID { get; set; }
            public string? ActionType { get; set; }
            public string? PJMActionType { get; set; }
            public string? ActionDate { get; set; }
            public int? StatusID { get; set; }
        }
        public class GetlistChecklistPC
        {
            public Guid? UnitFormID { get; set; }
            public Guid? ProjectID { get; set; }
            public string? ProjectName { get; set; }
            public Guid? UnitID { get; set; }
            public string? UnitCode { get; set; }
            public int? UnitFormStatus { get; set; }
            public int? FormID { get; set; }
            public string? FormName { get; set; }
            public string? Grade { get; set; }
            public int? GroupID { get; set; }
            public string? GroupName { get; set; }
            public int? PC_ID { get; set; }
            public int? LockStatusID { get; set; }
            public int? PC_StatusID { get; set; }
            public bool? PC_FlagActive { get; set; }
            public string? PE_Remark { get; set; }
            public string? PM_Remark { get; set; }
            public string? PJM_Remark { get; set; }
            public string? PEUnLock_Remark { get; set; }
            public string? PMUnLock_Remark { get; set; }
            public string? PJM_Actiontype { get; set; }
            public string? PM_Actiontype { get; set; }
            public string? PJM_ActionBy { get; set; }
            public DateTime? PJM_ActionDate { get; set; }
            public int? PJM_StatusID { get; set; }
            public string? PJMUnitFormRemark { get; set; }
        }
        public class PJMApproveIU
        {
            public Guid? UnitFormID { get; set; }
            public Guid? ProjectID { get; set; }
            public Guid? UnitID { get; set; }
            public Guid? UserID { get; set; }
            public int? FormID { get; set; }
            public string? ActionType { get; set; }
            public string? Remark { get; set; }
            public List<PJMIUPC>? ListPCIC { get; set; }
            public string? ApplicationPath { get; set; }
            public List<IFormFile>? Images { get; set; }
        }
        public class PJMIUPC
        {
            public Guid? UnitFormID { get; set; }
            public int? PC_ID { get; set; }
            public int? Group_ID { get; set; }
            public int? StatusID { get; set; }
            public int? PC_FlagActive { get; set; }
            public string? PJM_Remark { get; set; }
        }

        public class GetImageUnlock
        {
            public int UnitFormResourceID { get; set; }
            public int PassConditionID { get; set; }
            public Guid? ResourceID { get; set; }
            public Guid MasterResourceID { get; set; }
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public int RoleID { get; set; }
            public Guid? UnitFormID { get; set; }
            public int? GroupID { get; set; }
            public int? PC_ID { get; set; }
        }
    }
}
