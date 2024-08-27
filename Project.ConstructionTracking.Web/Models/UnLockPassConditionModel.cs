namespace Project.ConstructionTracking.Web.Models
{
    public class UnLockPassConditionModel
    {
        public class GetDataUnlockPC
        {
            public int PC_ID { get; set; }
            public string? ProjectName { get; set; }
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
        }
        public class UpdateUnlockPC
        {
            public int PC_ID { get; set; }
            public Guid? UnitFormID { get; set; }
            public int? GroupID { get; set; }
            public string? PEUnLock_Remark { get; set; }
        }
    }
}
