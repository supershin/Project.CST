using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using Project.ConstructionTracking.Web.Models;
using System.Collections.Generic;
using System.Data;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;

namespace Project.ConstructionTracking.Web.Library.DAL.SQL
{
    public class SqlMasterManagementProject : MasterManagementProviderProject
    {
        public SqlMasterManagementProject(IConfiguration configuration) : base(configuration)
        {

        }

        public override List<ProjectModel> SP_Get_Project(ProjectModel en)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand SqlCmd = new SqlCommand("SP_Get_Project", SqlCon);
                try
                {
                    SqlCon.Open();
                    SqlTransaction Trans = SqlCon.BeginTransaction();
                    SqlCmd.Transaction = Trans;
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlCmd.Parameters.Add(new SqlParameter("@act", SqlDbType.NVarChar)).Value = en.act;
                    SqlCmd.Parameters.Add(new SqlParameter("@ProjectID", SqlDbType.NVarChar)).Value = en.ProjectID ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@BUID", SqlDbType.Int)).Value = en.BUID != -1 ? en.BUID : (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@ProjectTypeID", SqlDbType.Int)).Value = en.ProjectTypeID != -1 ? en.ProjectTypeID : (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@ProjectCode", SqlDbType.NVarChar)).Value = en.ProjectCode ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@ProjectName", SqlDbType.NVarChar)).Value = en.ProjectName ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@FlagActive", SqlDbType.Int)).Value = en.FlagActive != -1 ? en.FlagActive : (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@CreateBy", SqlDbType.Int)).Value = en.CreateBy != -1 ? en.CreateBy : (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@UpdateBy", SqlDbType.Int)).Value = en.UpdateBy != -1 ? en.UpdateBy : (object)DBNull.Value;

                    switch (en.act)
                    {
                        case "GET":
                            return SP_Get_Project_ListReader(ExecuteReader(SqlCmd));

                        default:
                            return new List<ProjectModel>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Stored name : Sp_Get_Project");
                    Log.Error("SEND pram1 Act (nvarchar) : {Act}", en.act);
                    Log.Error("SEND pram2 ProjectID (nvarchar) : " + en.ProjectID);
                    Log.Error("SEND pram3 BUID (int) : " + en.BUID);
                    Log.Error("SEND pram4 ProjectTypeID (int) : " + en.ProjectTypeID);
                    Log.Error("SEND pram5 ProjectCode (nvarchar) : " + en.ProjectCode);
                    Log.Error("SEND pram6 ProjectName (nvarchar) : " + en.ProjectName);
                    Log.Error("SEND pram7 FlagActive (int) : " + en.FlagActive);
                    Log.Error("SEND pram8 CreateBy (int) : " + en.CreateBy);
                    Log.Error("SEND pram9 UpdateBy (int) : " + en.UpdateBy);
                    Log.Error(ex.ToString());
                    Log.Error("=========== END ===========");

                    return new List<ProjectModel>();
                }
                finally
                {
                    SqlCmd.Dispose();
                    SqlCon.Close();
                    SqlCon.Dispose();
                }
            }
        }

        public override List<UnitStatusModel> sp_get_unitstatus(UnitStatusModel en)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand SqlCmd = new SqlCommand("sp_get_unitstatus", SqlCon);
                try
                {
                    SqlCon.Open();
                    SqlTransaction Trans = SqlCon.BeginTransaction();
                    SqlCmd.Transaction = Trans;
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlCmd.Parameters.Add(new SqlParameter("@act", SqlDbType.NVarChar)).Value = en.act ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@project_id", SqlDbType.NVarChar)).Value = en.project_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_id", SqlDbType.NVarChar)).Value = en.unit_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_status", SqlDbType.NVarChar)).Value = en.unit_status ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@build_status", SqlDbType.NVarChar)).Value = en.build_status ?? (object)DBNull.Value;
                    switch (en.act)
                    {
                        case "GetlistUnitStatustest":
                            return sp_get_unitstatus_ListReader(ExecuteReader(SqlCmd));

                        default:
                            return new List<UnitStatusModel>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Stored name : sp_get_unitstatus");
                    Log.Error("SEND pram1 Act (nvarchar) : {Act}", en.act);
                    Log.Error("SEND pram2 unit_id (nvarchar) : {Unit_id}", en.unit_id);
                    Log.Error("SEND pram3 project_id (nvarchar) : {Project_id}", en.project_id);
                    Log.Error("SEND pram4 unit_status (nvarchar) : {Unit_status}", en.unit_status);
                    Log.Error("SEND pram5 build_status (nvarchar) : {build_status}", en.build_status);
                    Log.Error(ex.ToString());
                    Log.Error("=========== END ===========");

                    return new List<UnitStatusModel>();
                }
                finally
                {
                    SqlCmd.Dispose();
                    SqlCon.Close();
                    SqlCon.Dispose();
                }
            }
        }

        public override List<PEMyTaskModel> sp_get_mytask_pe(PEMyTaskModel en)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand SqlCmd = new SqlCommand("sp_get_mytask", SqlCon);
                try
                {
                    SqlCon.Open();
                    SqlTransaction Trans = SqlCon.BeginTransaction();
                    SqlCmd.Transaction = Trans;
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlCmd.Parameters.Add(new SqlParameter("@act", SqlDbType.NVarChar)).Value = en.act;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_id", SqlDbType.NVarChar)).Value = en.unit_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@project_id", SqlDbType.NVarChar)).Value = en.project_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_status", SqlDbType.NVarChar)).Value = en.unit_status ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@user_id", SqlDbType.NVarChar)).Value = en.user_id ?? (object)DBNull.Value;
                    switch (en.act)
                    {
                        case "listPEtask":
                            return sp_get_mytask_pe_ListReader(ExecuteReader(SqlCmd));

                        default:
                            return new List<PEMyTaskModel>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Stored name : sp_get_mytask");
                    Log.Error("SEND pram1 Act (nvarchar) : {Act}", en.act);
                    Log.Error("SEND pram2 unit_id (nvarchar) : {Unit_id}", en.unit_id);
                    Log.Error("SEND pram3 project_id (nvarchar) : {Project_id}", en.project_id);
                    Log.Error("SEND pram4 unit_status (int) : {Unit_status}", en.unit_status);
                    Log.Error("SEND pram5 user_id (int) : {user_id}", en.user_id);
                    Log.Error(ex.ToString());
                    Log.Error("=========== END ===========");

                    return new List<PEMyTaskModel>();
                }
                finally
                {
                    SqlCmd.Dispose();
                    SqlCon.Close();
                    SqlCon.Dispose();
                }
            }
        }

        public override List<PMMyTaskModel> sp_get_mytask_pm(PMMyTaskModel en)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand SqlCmd = new SqlCommand("sp_get_mytask", SqlCon);
                try
                {
                    SqlCon.Open();
                    SqlTransaction Trans = SqlCon.BeginTransaction();
                    SqlCmd.Transaction = Trans;
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlCmd.Parameters.Add(new SqlParameter("@act", SqlDbType.NVarChar)).Value = en.act;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_id", SqlDbType.NVarChar)).Value = en.unit_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@project_id", SqlDbType.NVarChar)).Value = en.project_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_status", SqlDbType.NVarChar)).Value = en.unit_status ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@user_id", SqlDbType.NVarChar)).Value = en.user_id ?? (object)DBNull.Value;
                    switch (en.act)
                    {
                        case "listPMtask":
                            return sp_get_mytask_pm_ListReader(ExecuteReader(SqlCmd));

                        default:
                            return new List<PMMyTaskModel>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Stored name : sp_get_mytask");
                    Log.Error("SEND pram1 Act (nvarchar) : {Act}", en.act);
                    Log.Error("SEND pram2 unit_id (nvarchar) : {Unit_id}", en.unit_id);
                    Log.Error("SEND pram3 project_id (nvarchar) : {Project_id}", en.project_id);
                    Log.Error("SEND pram4 unit_status (int) : {Unit_status}", en.unit_status);
                    Log.Error("SEND pram5 user_id (int) : {user_id}", en.user_id);
                    Log.Error(ex.ToString());
                    Log.Error("=========== END ===========");

                    return new List<PMMyTaskModel>();
                }
                finally
                {
                    SqlCmd.Dispose();
                    SqlCon.Close();
                    SqlCon.Dispose();
                }
            }
        }

        public override List<PJMMyTaskModel> sp_get_mytask_pjm(PJMMyTaskModel en)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand SqlCmd = new SqlCommand("sp_get_mytask", SqlCon);
                try
                {
                    SqlCon.Open();
                    SqlTransaction Trans = SqlCon.BeginTransaction();
                    SqlCmd.Transaction = Trans;
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlCmd.Parameters.Add(new SqlParameter("@act", SqlDbType.NVarChar)).Value = en.act;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_id", SqlDbType.NVarChar)).Value = en.unit_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@project_id", SqlDbType.NVarChar)).Value = en.project_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_status", SqlDbType.NVarChar)).Value = en.unit_status ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@user_id", SqlDbType.NVarChar)).Value = en.user_id ?? (object)DBNull.Value;
                    switch (en.act)
                    {
                        case "listPJMtask":
                            return sp_get_mytask_pjm_ListReader(ExecuteReader(SqlCmd));

                        default:
                            return new List<PJMMyTaskModel>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Stored name : sp_get_mytask");
                    Log.Error("SEND pram1 Act (nvarchar) : {Act}", en.act);
                    Log.Error("SEND pram2 unit_id (nvarchar) : {Unit_id}", en.unit_id);
                    Log.Error("SEND pram3 project_id (nvarchar) : {Project_id}", en.project_id);
                    Log.Error("SEND pram4 unit_status (int) : {Unit_status}", en.unit_status);
                    Log.Error("SEND pram5 user_id (int) : {user_id}", en.user_id);
                    Log.Error(ex.ToString());
                    Log.Error("=========== END ===========");

                    return new List<PJMMyTaskModel>();
                }
                finally
                {
                    SqlCmd.Dispose();
                    SqlCon.Close();
                    SqlCon.Dispose();
                }
            }
        }

        public override List<UnitListModel> sp_get_UnitList(UnitListModel en)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand SqlCmd = new SqlCommand("sp_get_unitstatusbk", SqlCon);
                try
                {
                    SqlCon.Open();
                    SqlTransaction Trans = SqlCon.BeginTransaction();
                    SqlCmd.Transaction = Trans;
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlCmd.Parameters.Add(new SqlParameter("@act", SqlDbType.NVarChar)).Value = en.act;
                    SqlCmd.Parameters.Add(new SqlParameter("@project_id", SqlDbType.NVarChar)).Value = en.project_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_id", SqlDbType.NVarChar)).Value = en.unit_id ?? (object)DBNull.Value;                    
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_status", SqlDbType.NVarChar)).Value = en.unit_status ?? (object)DBNull.Value;
                    switch (en.act)
                    {
                        case "GetUnitlist":
                            return sp_get_UnitList_ListReader(ExecuteReader(SqlCmd));

                        default:
                            return new List<UnitListModel>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Stored name : sp_get_unitstatusbk");
                    Log.Error("SEND pram1 Act (nvarchar) : {Act}", en.act);
                    Log.Error("SEND pram2 unit_id (nvarchar) : {Unit_id}", en.unit_id);
                    Log.Error("SEND pram3 project_id (nvarchar) : {Project_id}", en.project_id);
                    Log.Error("SEND pram4 unit_status (nvarchar) : {Unit_status}", en.unit_status);
                    Log.Error(ex.ToString());
                    Log.Error("=========== END ===========");

                    return new List<UnitListModel>();
                }
                finally
                {
                    SqlCmd.Dispose();
                    SqlCon.Close();
                    SqlCon.Dispose();
                }
            }
        }

        public override List<SummeryUnitFormModel> sp_get_summaryunitform(SummeryUnitFormModel en)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand SqlCmd = new SqlCommand("sp_get_summaryunitform", SqlCon);
                try
                {
                    SqlCon.Open();
                    SqlTransaction Trans = SqlCon.BeginTransaction();
                    SqlCmd.Transaction = Trans;
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlCmd.Parameters.Add(new SqlParameter("@act", SqlDbType.NVarChar)).Value = en.act;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_id", SqlDbType.NVarChar)).Value = en.unit_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@project_id", SqlDbType.NVarChar)).Value = en.project_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_status", SqlDbType.NVarChar)).Value = en.unit_status ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@user_id", SqlDbType.NVarChar)).Value = en.user_id ?? (object)DBNull.Value;
                    switch (en.act)
                    {
                        case "unitsummary":
                            return sp_get_summaryunitform_ListReader(ExecuteReader(SqlCmd));

                        default:
                            return new List<SummeryUnitFormModel>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Stored name : sp_get_summaryunitform");
                    Log.Error("SEND pram1 Act (nvarchar) : {Act}", en.act);
                    Log.Error("SEND pram2 unit_id (nvarchar) : {Unit_id}", en.unit_id);
                    Log.Error("SEND pram3 project_id (nvarchar) : {Project_id}", en.project_id);
                    Log.Error("SEND pram4 unit_status (nvarchar) : {Unit_status}", en.unit_status);
                    Log.Error("SEND pram5 user_id (nvarchar) : {user_id}", en.user_id);
                    Log.Error(ex.ToString());
                    Log.Error("=========== END ===========");

                    return new List<SummeryUnitFormModel>();
                }
                finally
                {
                    SqlCmd.Dispose();
                    SqlCon.Close();
                    SqlCon.Dispose();
                }
            }
        }

        public override List<UnitFormStatusModel> sp_get_UnitFormStatusByUnit(UnitFormStatusModel en)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand SqlCmd = new SqlCommand("sp_get_unitstatus", SqlCon);
                try
                {
                    SqlCon.Open();
                    SqlTransaction Trans = SqlCon.BeginTransaction();
                    SqlCmd.Transaction = Trans;
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlCmd.Parameters.Add(new SqlParameter("@act", SqlDbType.NVarChar)).Value = en.act ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@project_id", SqlDbType.NVarChar)).Value = en.project_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_id", SqlDbType.NVarChar)).Value = en.unit_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_status", SqlDbType.NVarChar)).Value = en.unit_status ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@build_status", SqlDbType.NVarChar)).Value = en.build_status ?? (object)DBNull.Value;
                    switch (en.act)
                    {
                        case "UnitFormStatusByUnit":
                            return sp_get_UnitFormStatusByUnit_ListReader(ExecuteReader(SqlCmd));

                        default:
                            return new List<UnitFormStatusModel>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Stored name : sp_get_unitstatus");
                    Log.Error("SEND pram1 Act (nvarchar) : {Act}", en.act);
                    Log.Error("SEND pram2 unit_id (nvarchar) : {Unit_id}", en.unit_id);
                    Log.Error("SEND pram3 project_id (nvarchar) : {Project_id}", en.project_id);
                    Log.Error("SEND pram4 unit_status (nvarchar) : {Unit_status}", en.unit_status);
                    Log.Error("SEND pram5 build_status (nvarchar) : {build_status}", en.build_status);
                    Log.Error(ex.ToString());
                    Log.Error("=========== END ===========");

                    return new List<UnitFormStatusModel>();
                }
                finally
                {
                    SqlCmd.Dispose();
                    SqlCon.Close();
                    SqlCon.Dispose();
                }
            }
        }

        public override List<WorkPeriodModel> sp_get_workperiod(WorkPeriodModel en)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand SqlCmd = new SqlCommand("sp_get_workperiod", SqlCon);
                try
                {
                    SqlCon.Open();
                    SqlTransaction Trans = SqlCon.BeginTransaction();
                    SqlCmd.Transaction = Trans;
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlCmd.Parameters.Add(new SqlParameter("@act", SqlDbType.NVarChar)).Value = en.act ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@project_id", SqlDbType.NVarChar)).Value = en.project_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_id", SqlDbType.NVarChar)).Value = en.unit_id ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@unit_status", SqlDbType.NVarChar)).Value = en.unit_status ?? (object)DBNull.Value;
                    SqlCmd.Parameters.Add(new SqlParameter("@user_id", SqlDbType.NVarChar)).Value = en.user_id ?? (object)DBNull.Value;
                    switch (en.act)
                    {
                        case "Getworkperiodlist":
                            return sp_get_workperiod_ListReader(ExecuteReader(SqlCmd));

                        default:
                            return new List<WorkPeriodModel>();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Stored name : sp_get_unitstatus");
                    Log.Error("SEND pram1 Act (nvarchar) : {Act}", en.act);
                    Log.Error("SEND pram2 unit_id (nvarchar) : {Unit_id}", en.unit_id);
                    Log.Error("SEND pram3 project_id (nvarchar) : {Project_id}", en.project_id);
                    Log.Error("SEND pram4 unit_status (nvarchar) : {Unit_status}", en.unit_status);
                    Log.Error("SEND pram5 user_id (nvarchar) : {user_id}", en.user_id);
                    Log.Error(ex.ToString());
                    Log.Error("=========== END ===========");

                    return new List<WorkPeriodModel>();
                }
                finally
                {
                    SqlCmd.Dispose();
                    SqlCon.Close();
                    SqlCon.Dispose();
                }
            }
        }
    }
}
