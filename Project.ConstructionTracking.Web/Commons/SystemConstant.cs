namespace Project.ConstructionTracking.Web.Commons
{
    public static class SystemConstant
    {
        public static class Ext
        {
            public const int FORM_TYPE_PE = 3;
            public const int FORM_TYPE_QC = 4;
            public const int SAVE_DARF_PE = 8;
        }

        public static class Ext_Type
        {
            public const int PROJECT_TYPE = 1;
            public const int UNIT_STATUS_ID = 4;
            public const int QC5RadioChecklist = 9;
        }

        public static class Unit_Status
        {
            public const int FREE = 3; //ว่าง
            public const int RESERVE = 4; //จอง
            public const int CONTRACT = 5; //สัญญา
            public const int TRANSFER = 6; //โอน
        }

        public static class UserRole
        {
            public const int PE = 1;
            public const int PM = 2;
            public const int PJM = 3;
            public const int QC = 4;
            public const int ADMIN = 5;
        }

        public static class UserRole_Desc
        {
            public const string PE = "PE/SE";
            public const string PM = "PM";
            public const string PJM = "PJM-Head";
            public const string QC = "Quality Control";
            public const string ADMIN = "Administrator";
        }

        public static class QcStatus
        {
            public const int PASS = 10;
            public const int FAILED = 11;
        }

        public static class ActionType
        {
            public const string SAVE = "save";
            public const string SUBMIT = "submit";
        }

        public static class QcSummaryStatus
        {
            public const int FINISH = 1;
            public const int SUSPEND = 2;
            public const int WORKING = 5;

            public class Desc
            {
                public const string FINISH = "F";
                public const string SUSPEND = "S";
                public const string WORKING = "W";
            }

        }

        public static class QcTypeID
        {
            public const int QC1 = 12;
            public const int QC2 = 13;
            public const int QC3 = 14;
            public const int QC4 = 26;
            public const int QC41 = 15;
            public const int QC42 = 16;
            public const int QC5 = 17;
        }

        public static class Unit_Form_QC
        {
            public const int QC1 = 12;
            public const int QC2 = 13;
            public const int QC3 = 14;
            public const int QC4_1 = 15;
            public const int QC4_2 = 16;
            public const int QC4 = 26;
            public const int QC5 = 17;
        }

        public static class Unit_Form_Status
        {
            public const int PE_Save = 1; 
            public const int PE_Submit = 2; 
            public const int PM_Save = 3; 
            public const int PM_Approve = 4;
            public const int PM_Reject = 5;
            public const int PM_Sendto_PJM = 6;
            public const int PJM_Approve = 7;
            public const int PJM_Reject = 8;
            public const int PJM_Save = 9;
            public const int PE_REQ_Unlock = 10;
            public const int PM_Approve_Unlock = 11;
            public const int PM_Reject_Unlock = 12;
        }

        public static class CheckList_Status
        {
            public const int PASS = 9;
            public const int NOTPASS = 10;
            public const int NOWORK = 11;
        }

    }

}
