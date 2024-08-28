namespace Project.ConstructionTracking.Web.Models
{
    public class FormCheckListModel
    {
        public class Form_getFilterData
        {
            public Guid? ProjectID { get; set; }
            public Guid? UnitID { get; set; }
            public Guid? UnitFormID { get; set; }
            public int? FormID { get; set; }
            public int GroupID { get; set; }
        }
        public class Form_getUnitFormData
        {
            public Guid? ProjectID { get; set; }
            public string? ProjectName { get; set; }
            public Guid? UnitID { get; set; }
            public string? UnitCode { get; set; }
            public Guid? UnitFormID { get; set; }
            public int? UnitFormStatusID { get; set; }
            public int? FormID { get; set; }
            public string? FormName { get; set; }
            public int GroupID { get; set; }
            public string? GroupName { get; set; }
        }
        public class Form_getListPackages
        {
            public int? UnitPackagesID { get; set; }
            public int? GroupID { get; set; }
            public int? PackagesID { get; set; }
            public string? PackagesName { get; set; }
            public string? Remark { get; set; }
            public List<Form_getListCheckLists> ListCheckLists { get; set; }
        }
        public class Form_getListCheckLists
        {
            public int? UnitCheckListID { get; set; }
            public int? PackageID { get; set; }
            public int? CheckListID { get; set; }
            public string? CheckListName { get; set; }
            public List<Form_getRadioCheckLists> ListRadioCheck { get; set; }
            public int? StatusCheck { get; set; }
          
        }
        public class Form_getRadioCheckLists
        {
            public int? RadioCheck_ID { get; set; }
            public string? RadioCheck_Name { get; set; }

        }
        public class Form_getListStatus
        {
            public Guid? ID { get; set; }
            public Guid? ProjectID { get; set; }
            public Guid? UnitID { get; set; }
            public int? UnitFormActionID { get; set; }
            public int? PM_StatusID { get; set; }
            public string? PM_ActionType { get; set; }
            public int? PJM_StatusID { get; set; }
            public string? PJM_ActionType { get; set; }
            public int? FormID { get; set; }
            public int? LockStatusID { get; set; }
            public int? PCStatusID { get; set; }
            public string? RemarkPassCondition { get; set; }
            public int? RoleID { get; set; }
            public List<Form_getListImagePasswithCondition>? Form_getListImagePasswithCondition { get; set; }
            public string? ActionType { get; set; }
            public string? UpdateDate { get; set; }

        }

        public class Form_getListImagePasswithCondition
        {
            public Guid? ResourceID { get; set; }
            public string? FileName { get; set; }
            public string? FilePath { get; set; }

        }

    }
}
