using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using Project.ConstructionTracking.Web.Models;
using System.Collections.Generic;
using System.Data;

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
                    Log.Error("SEND pram1 Act (nvarchar) : " + en.act);
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
    }
}
