using Microsoft.Extensions.Configuration;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using System.Collections.Generic;
using System.Data;

namespace Project.ConstructionTracking.Web.Library.DAL
{
    public abstract class MasterManagementProviderProject : DataAccess
    {
        private static MasterManagementProviderProject _instance;

        public static MasterManagementProviderProject Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Project.ConstructionTracking.Web.Library.DAL.SQL.SqlMasterManagementProject(_configuration);
                }
                return _instance;
            }
        }

        protected static IConfiguration _configuration;

        public MasterManagementProviderProject(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public abstract List<ProjectModel> SP_Get_Project(ProjectModel EN);

        public abstract List<UnitStatusModel> sp_get_unitstatus(UnitStatusModel EN);

        public abstract List<PEMyTaskModel> sp_get_mytask_pe(PEMyTaskModel EN);

        public abstract List<PMMyTaskModel> sp_get_mytask_pm(PMMyTaskModel EN);

        public abstract List<PJMMyTaskModel> sp_get_mytask_pjm(PJMMyTaskModel EN);


        #region __ Reader __
        public static List<ProjectModel> SP_Get_Project_ListReader(IDataReader reader)
        {
            List<ProjectModel> list = new List<ProjectModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(SP_Get_ListProject_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static ProjectModel SP_Get_ListProject_Reader(IDataReader reader, int index)
        {
            ProjectModel Entity = new ProjectModel();

            Entity.index = index;
            Entity.ProjectID = Commons.FormatExtension.NullToString(reader["ProjectID"]);
            Entity.BUID = Commons.FormatExtension.Nulltoint(reader["BUID"]);
            Entity.ProjectTypeID = Commons.FormatExtension.Nulltoint(reader["ProjectTypeID"]);
            Entity.ProjectCode = Commons.FormatExtension.NullToString(reader["ProjectCode"]);
            Entity.ProjectName = Commons.FormatExtension.NullToString(reader["ProjectName"]);
            Entity.FlagActive = Commons.FormatExtension.Nulltoint(reader["FlagActive"]);
            Entity.CreateDate = Commons.FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(reader["CreateDate"]);
            Entity.CreateBy = Commons.FormatExtension.Nulltoint(reader["CreateBy"]);
            Entity.UpdateDate = Commons.FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(reader["UpdateDate"]);
            Entity.UpdateBy = Commons.FormatExtension.Nulltoint(reader["UpdateBy"]);
            return Entity;
        }



        public static List<UnitStatusModel> sp_get_unitstatus_ListReader(IDataReader reader)
        {
            List<UnitStatusModel> list = new List<UnitStatusModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_unitstatus_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static UnitStatusModel sp_get_unitstatus_Reader(IDataReader reader, int index)
        {
            UnitStatusModel Entity = new UnitStatusModel();

            Entity.index = index;
            Entity.unit_code = Commons.FormatExtension.NullToString(reader["unit_code"]);
            Entity.model_type_str = Commons.FormatExtension.NullToString(reader["model_type_str"]);
            Entity.unit_type_str = Commons.FormatExtension.NullToString(reader["unit_type_str"]);
            Entity.unit_status_str = Commons.FormatExtension.NullToString(reader["unit_status_str"]);
            Entity.unit_build_status_str = Commons.FormatExtension.NullToString(reader["unit_build_status_str"]);
            Entity.date_start_plan_str = Commons.FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(reader["date_start_plan_str"]);
            Entity.date_end_plan_str = Commons.FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(reader["date_end_plan_str"]);
            Entity.formname_plan_str = Commons.FormatExtension.NullToString(reader["formname_plan_str"]);
            Entity.formname_true_str = Commons.FormatExtension.NullToString(reader["formname_true_str"]);
            Entity.process_plan_str = Commons.FormatExtension.NullToString(reader["process_plan_str"]);
            Entity.process_true_str = Commons.FormatExtension.NullToString(reader["process_true_str"]);
            Entity.delay_ahead_str = Commons.FormatExtension.NullToString(reader["delay_ahead_str"]);
            Entity.allday_str = Commons.FormatExtension.NullToString(reader["allday_str"]);
            Entity.realday_use_str = Commons.FormatExtension.NullToString(reader["realday_use_str"]);
            return Entity;
        }



        public static List<PEMyTaskModel> sp_get_mytask_pe_ListReader(IDataReader reader)
        {
            List<PEMyTaskModel> list = new List<PEMyTaskModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_mytask_pe_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static PEMyTaskModel sp_get_mytask_pe_Reader(IDataReader reader, int index)
        {
            PEMyTaskModel Entity = new PEMyTaskModel();

            Entity.index = index;
            Entity.UnitID = Commons.FormatExtension.NullToString(reader["UnitID"]);
            Entity.UnitCode = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.ProjectName = Commons.FormatExtension.NullToString(reader["ProjectName"]);
            Entity.Grade = Commons.FormatExtension.NullToString(reader["Grade"]);
            Entity.FormID = Commons.FormatExtension.Nulltoint(reader["FormID"]);
            Entity.FormName = Commons.FormatExtension.NullToString(reader["FormName"]);
            Entity.ComVendorName = Commons.FormatExtension.NullToString(reader["ComVendorName"]);
            Entity.StatusID = Commons.FormatExtension.Nulltoint(reader["StatusID"]);
            Entity.StatusDescription = Commons.FormatExtension.NullToString(reader["StatusDescription"]);
            Entity.StatusIcon = Commons.FormatExtension.NullToString(reader["StatusIcon"]);
            Entity.StatusColor = Commons.FormatExtension.NullToString(reader["StatusColor"]);
            Entity.PMActionBy = Commons.FormatExtension.NullToString(reader["PMActionBy"]);
            Entity.PMActionDate = Commons.FormatExtension.FormatDateToDayMonthNameYearTime(reader["PMActionDate"]);
            Entity.PJMActionBy = Commons.FormatExtension.NullToString(reader["PJMActionBy"]);
            Entity.PJMActionDate = Commons.FormatExtension.NullToString(reader["PJMActionDate"]);
            Entity.PassConditionIcon = Commons.FormatExtension.NullToString(reader["PassConditionIcon"]);
            Entity.PassConditionColor = Commons.FormatExtension.NullToString(reader["PassConditionColor"]);
            return Entity;
        }



        public static List<PMMyTaskModel> sp_get_mytask_pm_ListReader(IDataReader reader)
        {
            List<PMMyTaskModel> list = new List<PMMyTaskModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_mytask_pm_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static PMMyTaskModel sp_get_mytask_pm_Reader(IDataReader reader, int index)
        {
            PMMyTaskModel Entity = new PMMyTaskModel();

            Entity.index = index;
            Entity.UnitID = Commons.FormatExtension.NullToString(reader["UnitID"]);
            Entity.UnitCode = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.ProjectName = Commons.FormatExtension.NullToString(reader["ProjectName"]);
            Entity.FormID = Commons.FormatExtension.Nulltoint(reader["FormID"]);
            Entity.FormName = Commons.FormatExtension.NullToString(reader["FormName"]);
            Entity.ComVendorName = Commons.FormatExtension.NullToString(reader["ComVendorName"]);
            Entity.StatusID = Commons.FormatExtension.Nulltoint(reader["StatusID"]);
            Entity.StatusDescription = Commons.FormatExtension.NullToString(reader["StatusDescription"]);
            Entity.StatusIcon = Commons.FormatExtension.NullToString(reader["StatusIcon"]);
            Entity.StatusColor = Commons.FormatExtension.NullToString(reader["StatusColor"]);
            Entity.PEActionBy = Commons.FormatExtension.NullToString(reader["PEActionBy"]);
            Entity.PEActionDate = Commons.FormatExtension.FormatDateToDayMonthNameYearTime(reader["PEActionDate"]);
            Entity.PassConditionIcon = Commons.FormatExtension.NullToString(reader["PassConditionIcon"]);
            Entity.PassConditionColor = Commons.FormatExtension.NullToString(reader["PassConditionColor"]);
            return Entity;
        }


        public static List<PJMMyTaskModel> sp_get_mytask_pjm_ListReader(IDataReader reader)
        {
            List<PJMMyTaskModel> list = new List<PJMMyTaskModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_mytask_pjm_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static PJMMyTaskModel sp_get_mytask_pjm_Reader(IDataReader reader, int index)
        {
            PJMMyTaskModel Entity = new PJMMyTaskModel();

            Entity.index = index;
            Entity.UnitID = Commons.FormatExtension.NullToString(reader["UnitID"]);
            Entity.UnitFormID = Commons.FormatExtension.NullToString(reader["UnitFormID"]);
            Entity.UnitCode = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.ProjectName = Commons.FormatExtension.NullToString(reader["ProjectName"]);
            Entity.FormID = Commons.FormatExtension.Nulltoint(reader["FormID"]);
            Entity.FormName = Commons.FormatExtension.NullToString(reader["FormName"]);
            Entity.ComVendorName = Commons.FormatExtension.NullToString(reader["ComVendorName"]);
            Entity.StatusID = Commons.FormatExtension.Nulltoint(reader["StatusID"]);
            Entity.StatusDescription = Commons.FormatExtension.NullToString(reader["StatusDescription"]);
            Entity.StatusIcon = Commons.FormatExtension.NullToString(reader["StatusIcon"]);
            Entity.StatusColor = Commons.FormatExtension.NullToString(reader["StatusColor"]);
            Entity.PEActionBy = Commons.FormatExtension.NullToString(reader["PEActionBy"]);
            Entity.PEActionDate = Commons.FormatExtension.FormatDateToDayMonthNameYearTime(reader["PEActionDate"]);
            Entity.PMActionBy = Commons.FormatExtension.NullToString(reader["PMActionBy"]);
            Entity.PMActionDate = Commons.FormatExtension.FormatDateToDayMonthNameYearTime(reader["PMActionDate"]);
            Entity.PassConditionIcon = Commons.FormatExtension.NullToString(reader["PassConditionIcon"]);
            Entity.PassConditionColor = Commons.FormatExtension.NullToString(reader["PassConditionColor"]);
            return Entity;
        }
        #endregion
    }
}
