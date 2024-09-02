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
            public const string PE = "Project Engineer";
            public const string PM = "Product Manager";
            public const string PJM = "Project Manager";
            public const string QC = "Quality Control";
            public const string ADMIN = "Administrator";
        }
    }

}
