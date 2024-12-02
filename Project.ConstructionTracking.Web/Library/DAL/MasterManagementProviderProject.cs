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

        public abstract List<UnitListModel> sp_get_UnitList(UnitListModel EN);

        public abstract List<SummeryUnitFormModel> sp_get_summaryunitform(SummeryUnitFormModel EN);

        public abstract List<UnitFormStatusModel> sp_get_UnitFormStatusByUnit(UnitFormStatusModel EN);

        public abstract List<WorkPeriodModel> sp_get_workperiod(WorkPeriodModel EN);

        public abstract List<ReportProjectProgressModel> sp_get_report_Project_Prcress(ReportProjectProgressModel EN);

        public abstract List<ReportProjectPassConditionModel> sp_get_report_Project_PassCondition(ReportProjectPassConditionModel EN);

        public abstract List<ReportWorkloadAndInspectionResultsModel> sp_get_report_workload_inspection_results(ReportWorkloadAndInspectionResultsModel EN);


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
            Entity.unit_id = Commons.FormatExtension.NullToString(reader["UnitID"]);
            Entity.project_id = Commons.FormatExtension.NullToString(reader["ProjectID"]);
            Entity.unit_code = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.model_type_str = Commons.FormatExtension.NullToString(reader["model_type_str"]);
            Entity.unit_type_str = Commons.FormatExtension.NullToString(reader["unit_type_str"]);
            Entity.unit_status_str = Commons.FormatExtension.NullToString(reader["unit_status_str"]);
            Entity.date_start_plan_str = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["date_start_plan_str"]);
            Entity.date_end_plan_str = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["date_end_plan_str"]);
            Entity.formname_plan_str = Commons.FormatExtension.NullToString(reader["FormPlan"]);
            Entity.formname_true_str = Commons.FormatExtension.NullToString(reader["FormActual"]);
            Entity.process_plan_str = Commons.FormatExtension.NullToString(reader["ProgressPlan"]);
            Entity.process_true_str = Commons.FormatExtension.NullToString(reader["ProgressActual"]);
            Entity.allday_str = Commons.FormatExtension.NullToString(reader["AllDay"]);
            Entity.realday_use_str = Commons.FormatExtension.NullToString(reader["AllDayActual"]);
            Entity.delay_ahead_str = Commons.FormatExtension.NullToString(reader["DelayAhead"]);
            Entity.unit_build_status_str = Commons.FormatExtension.NullToString(reader["unit_build_status_str"]);
            Entity.Latestwithdrawal = Commons.FormatExtension.NullToString(reader["Latestwithdrawal"]);
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
            Entity.PassConditionIcon = Commons.FormatExtension.NullToString(reader["PassCondition_Icon"]);
            Entity.PassConditionColor = Commons.FormatExtension.NullToString(reader["PassCondition_Color"]);
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
            Entity.PassConditionIcon = Commons.FormatExtension.NullToString(reader["PassCondition_Icon"]);
            Entity.PassConditionColor = Commons.FormatExtension.NullToString(reader["PassCondition_Color"]);
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
            Entity.PassConditionIcon = Commons.FormatExtension.NullToString(reader["PassCondition_Icon"]);
            Entity.PassConditionColor = Commons.FormatExtension.NullToString(reader["PassCondition_Color"]);
            return Entity;
        }


        public static List<UnitListModel> sp_get_UnitList_ListReader(IDataReader reader)
        {
            List<UnitListModel> list = new List<UnitListModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_UnitList_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static UnitListModel sp_get_UnitList_Reader(IDataReader reader, int index)
        {
            UnitListModel Entity = new UnitListModel();

            Entity.index = index;
            Entity.UnitID = Commons.FormatExtension.NullToString(reader["UnitID"]);
            Entity.ProjectID = Commons.FormatExtension.NullToString(reader["ProjectID"]);
            Entity.UnitCode = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.UnitStatusID = Commons.FormatExtension.NullToString(reader["UnitStatusID"]);
            Entity.UnitStatusName = Commons.FormatExtension.NullToString(reader["UnitStatusName"]);
            Entity.FormID = Commons.FormatExtension.NullToString(reader["FormID"]);
            Entity.FormName = Commons.FormatExtension.NullToString(reader["FormName"]);
            return Entity;
        }


        public static List<SummeryUnitFormModel> sp_get_summaryunitform_ListReader(IDataReader reader)
        {
            List<SummeryUnitFormModel> list = new List<SummeryUnitFormModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_summaryunitform_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static SummeryUnitFormModel sp_get_summaryunitform_Reader(IDataReader reader, int index)
        {
            SummeryUnitFormModel Entity = new SummeryUnitFormModel();

            Entity.index = index;
            Entity.UnitID = Commons.FormatExtension.ConvertStringToGuid(reader["UnitID"]);
            Entity.ProjectID = Commons.FormatExtension.ConvertStringToGuid(reader["ProjectID"]);
            Entity.UnitFormID = Commons.FormatExtension.ConvertStringToGuid(reader["UnitFormID"]);
            Entity.ProjectName = Commons.FormatExtension.NullToString(reader["ProjectName"]);
            Entity.UnitCode = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.FormID = Commons.FormatExtension.Nulltoint(reader["FormID"]);
            Entity.FormName = Commons.FormatExtension.NullToString(reader["FormName"]);
            Entity.StatusID = Commons.FormatExtension.Nulltoint(reader["StatusID"]);
            Entity.PEColor = Commons.FormatExtension.NullToString(reader["PEColor"]);
            Entity.QC1 = Commons.FormatExtension.NullToString(reader["QC1"]);
            Entity.QC2 = Commons.FormatExtension.NullToString(reader["QC2"]);
            Entity.QC3 = Commons.FormatExtension.NullToString(reader["QC3"]);
            Entity.QC4 = Commons.FormatExtension.NullToString(reader["QC4"]);
            Entity.QC4_1 = Commons.FormatExtension.NullToString(reader["QC4_1"]);
            Entity.QC4_2 = Commons.FormatExtension.NullToString(reader["QC4_2"]);
            Entity.QC5 = Commons.FormatExtension.NullToString(reader["QC5"]);
            Entity.PMColor = Commons.FormatExtension.NullToString(reader["PMColor"]);
            Entity.Cnt_PC = Commons.FormatExtension.Nulltoint(reader["Cnt_PC"]);
            Entity.PCColor = Commons.FormatExtension.NullToString(reader["PCColor"]);
            Entity.PCIcon = Commons.FormatExtension.NullToString(reader["PCIcon"]);
            return Entity;
        }


        public static List<UnitFormStatusModel> sp_get_UnitFormStatusByUnit_ListReader(IDataReader reader)
        {
            List<UnitFormStatusModel> list = new List<UnitFormStatusModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_UnitFormStatusByUnit_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static UnitFormStatusModel sp_get_UnitFormStatusByUnit_Reader(IDataReader reader, int index)
        {
            UnitFormStatusModel Entity = new UnitFormStatusModel();

            Entity.index = index;
            Entity.UnitCode = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.UnitFormID = Commons.FormatExtension.ConvertStringToGuid(reader["UnitFormID"]);
            Entity.Form = Commons.FormatExtension.NullToString(reader["Form"]);
            Entity.Vender = Commons.FormatExtension.NullToString(reader["Vender"]);
            Entity.Progress = Commons.FormatExtension.NullToString(reader["Progress"]);
            Entity.DurationDay = Commons.FormatExtension.NullToString(reader["DurationDay"]);
            Entity.QC = Commons.FormatExtension.NullToString(reader["QC"]);
            Entity.StartPlan = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["StartPlan"]);
            Entity.EndPlan = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["EndPlan"]);
            Entity.PESave = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["PESave"]);
            Entity.PMSubmit = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["PMSubmit"]);
            Entity.PCStatus = Commons.FormatExtension.NullToString(reader["PCStatus"]);
            Entity.DisbursementStatus = Commons.FormatExtension.NullToString(reader["DisbursementStatus"]);
            Entity.PCUnlock = Commons.FormatExtension.NullToString(reader["PCUnlock"]);
            Entity.UnitFormPDF = Commons.FormatExtension.NullToString(reader["UnitFormPDF"]);
            Entity.QCPDF = Commons.FormatExtension.NullToString(reader["UnitQCPDF"]);
            return Entity;
        }


        public static List<WorkPeriodModel> sp_get_workperiod_ListReader(IDataReader reader)
        {
            List<WorkPeriodModel> list = new List<WorkPeriodModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_workperiod_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static WorkPeriodModel sp_get_workperiod_Reader(IDataReader reader, int index)
        {
            WorkPeriodModel Entity = new WorkPeriodModel();

            Entity.index = index;
            Entity.ProjectName = Commons.FormatExtension.NullToString(reader["ProjectName"]);
            Entity.UnitCode = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.FormName = Commons.FormatExtension.NullToString(reader["FormName"]);
            Entity.CompanyVendorName = Commons.FormatExtension.NullToString(reader["CompanyVendorName"]);
            Entity.PONo = Commons.FormatExtension.NullToString(reader["PONo"]);
            Entity.ApproveDate = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["ApproveDate"]);
            Entity.UnitFormPDF = Commons.FormatExtension.NullToString(reader["UnitFormPDF"]);
            Entity.QCPDF = Commons.FormatExtension.NullToString(reader["QCPDF"]);
            Entity.GRNo = Commons.FormatExtension.NullToString(reader["GRNo"]);
            Entity.SyncStatusID = Commons.FormatExtension.NullToString(reader["SyncStatusID"]);
            Entity.GRDate = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["GRDate"]);
            Entity.UnitFormID = Commons.FormatExtension.ConvertStringToGuid(reader["UnitFormID"]);
            Entity.SumPercentPayment = Commons.FormatExtension.NullToString(reader["SumPercentPayment"]);
            Entity.Statusworkperiod = Commons.FormatExtension.NullToString(reader["Statusworkperiod"]);
            return Entity;
        }


        public static List<ReportProjectProgressModel> sp_get_report_Project_PrcressListReader(IDataReader reader)
        {
            List<ReportProjectProgressModel> list = new List<ReportProjectProgressModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_report_Project_Prcress_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static ReportProjectProgressModel sp_get_report_Project_Prcress_Reader(IDataReader reader, int index)
        {
            ReportProjectProgressModel Entity = new ReportProjectProgressModel();

            Entity.index = index;
            Entity.ProjectName = Commons.FormatExtension.NullToString(reader["ProjectName"]);
            Entity.UnitCode = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.CompanyName = Commons.FormatExtension.NullToString(reader["CompanyName"]);
            Entity.PEName = Commons.FormatExtension.NullToString(reader["PEName"]);
            Entity.UnitStatus = Commons.FormatExtension.NullToString(reader["UnitStatus"]);
            Entity.TransferDate = Commons.FormatExtension.NullToString(reader["TransferDate"]);
            Entity.StartDatePlan = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["StartDatePlan"]);
            Entity.EndDatePlan = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["EndDatePlan"]);
            Entity.FormActual = Commons.FormatExtension.NullToString(reader["FormActual"]);
            Entity.ProgressPlan = Commons.FormatExtension.NullToString(reader["ProgressPlan"]);
            Entity.ProgressActual = Commons.FormatExtension.NullToString(reader["ProgressActual"]);
            Entity.DelayAhead = Commons.FormatExtension.NullToString(reader["DelayAhead"]);
            Entity.LastFormTransfer = Commons.FormatExtension.NullToString(reader["LastFormTransfer"]);
            return Entity;
        }


        public static List<ReportProjectPassConditionModel> sp_get_report_Project_PassConditionListReader(IDataReader reader)
        {
            List<ReportProjectPassConditionModel> list = new List<ReportProjectPassConditionModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_report_Project_PassCondition_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static ReportProjectPassConditionModel sp_get_report_Project_PassCondition_Reader(IDataReader reader, int index)
        {
            ReportProjectPassConditionModel Entity = new ReportProjectPassConditionModel();

            Entity.index = index;
            Entity.UnitCode = Commons.FormatExtension.NullToString(reader["UnitCode"]);
            Entity.FormName = Commons.FormatExtension.NullToString(reader["FormName"]);
            Entity.FormGroupName = Commons.FormatExtension.NullToString(reader["FormGroupName"]);
            Entity.PEStatusIDName = Commons.FormatExtension.NullToString(reader["PEStatusIDName"]);
            Entity.PEActionName = Commons.FormatExtension.NullToString(reader["PEActionName"]);
            Entity.PEActionDate = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["PEActionDate"]);
            Entity.PEPCRemark = Commons.FormatExtension.NullToString(reader["PEPCRemark"]);
            Entity.PMStatusIDName = Commons.FormatExtension.NullToString(reader["PMStatusIDName"]);
            Entity.PMActionName = Commons.FormatExtension.NullToString(reader["PMActionName"]);
            Entity.PMActionDate = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["PMActionDate"]);
            Entity.PMPCRemark = Commons.FormatExtension.NullToString(reader["PMPCRemark"]);
            Entity.PJMStatusIDName = Commons.FormatExtension.NullToString(reader["PJMStatusIDName"]);
            Entity.PJMActionName = Commons.FormatExtension.NullToString(reader["PJMActionName"]);
            Entity.PJMActionDate = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["PJMActionDate"]);
            Entity.PJMPCRemark = Commons.FormatExtension.NullToString(reader["PJMPCRemark"]);
            Entity.PERequestUnlock = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["PERequestUnlock"]);
            Entity.PMUnlock = Commons.FormatExtension.FormatDateToDayMonthNameYear(reader["PMUnlock"]);
            Entity.PCStatusName = Commons.FormatExtension.NullToString(reader["PCStatusName"]);
            return Entity;
        }


        public static List<ReportWorkloadAndInspectionResultsModel> sp_get_report_workload_inspection_resultsListReader(IDataReader reader)
        {
            List<ReportWorkloadAndInspectionResultsModel> list = new List<ReportWorkloadAndInspectionResultsModel>();
            int index = 1;
            while ((reader.Read()))
            {
                list.Add(sp_get_report_workload_inspection_results_Reader(reader, index));
                index++;
            }
            reader.Close();
            return list;
        }

        private static ReportWorkloadAndInspectionResultsModel sp_get_report_workload_inspection_results_Reader(IDataReader reader, int index)
        {
            ReportWorkloadAndInspectionResultsModel Entity = new ReportWorkloadAndInspectionResultsModel();

            Entity.index = index;
            Entity.MonthName = Commons.FormatExtension.NullTo2decimalplaces(reader["MonthName"]);
            Entity.MonthNumber = Commons.FormatExtension.NullTo2decimalplaces(reader["MonthNumber"]);
            Entity.QC1Pass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC1Pass"]);
            Entity.QC1NotPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC1NotPass"]);
            Entity.QC1NotReady = Commons.FormatExtension.NullTo2decimalplaces(reader["QC1NotReady"]);
            Entity.TotalQC1 = Commons.FormatExtension.NullTo2decimalplaces(reader["TotalQC1"]);
            Entity.QC1PercentPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC1PercentPass"]);
            Entity.QC2Pass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC2Pass"]);
            Entity.QC2NotPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC2NotPass"]);
            Entity.QC2NotReady = Commons.FormatExtension.NullTo2decimalplaces(reader["QC2NotReady"]);
            Entity.TotalQC2 = Commons.FormatExtension.NullTo2decimalplaces(reader["TotalQC2"]);
            Entity.QC2PercentPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC2PercentPass"]);
            Entity.QC3Pass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC3Pass"]);
            Entity.QC3NotPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC3NotPass"]);
            Entity.QC3NotReady = Commons.FormatExtension.NullTo2decimalplaces(reader["QC3NotReady"]);
            Entity.TotalQC3 = Commons.FormatExtension.NullTo2decimalplaces(reader["TotalQC3"]);
            Entity.QC3PercentPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC3PercentPass"]);
            Entity.QC4Pass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC4Pass"]);
            Entity.QC4NotPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC4NotPass"]);
            Entity.QC4NotReady = Commons.FormatExtension.NullTo2decimalplaces(reader["QC4NotReady"]);
            Entity.TotalQC4 = Commons.FormatExtension.NullTo2decimalplaces(reader["TotalQC4"]);
            Entity.QC4PercentPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC4PercentPass"]);
            Entity.QC5Pass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC5Pass"]);
            Entity.QC5NotPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC5NotPass"]);
            Entity.QC5NotReady = Commons.FormatExtension.NullTo2decimalplaces(reader["QC5NotReady"]);
            Entity.TotalQC5 = Commons.FormatExtension.NullTo2decimalplaces(reader["TotalQC5"]);
            Entity.QC5PercentPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QC5PercentPass"]);
            Entity.QCALLPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QCALLPass"]);
            Entity.QCALLNotPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QCALLNotPass"]);
            Entity.QCALLNotReady = Commons.FormatExtension.NullTo2decimalplaces(reader["QCALLNotReady"]);
            Entity.TotalQCALL = Commons.FormatExtension.NullTo2decimalplaces(reader["TotalQCALL"]);
            Entity.QCALLPercentPass = Commons.FormatExtension.NullTo2decimalplaces(reader["QCALLPercentPass"]);
            Entity.RowOrder = Commons.FormatExtension.NullTo2decimalplaces(reader["RowOrder"]);
            return Entity;
        }

        #endregion
    }
}
