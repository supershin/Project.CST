using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Data;

namespace Project.ConstructionTracking.Web.Library.DAL
{
    public class DataAccess
    {
        private string _connectionString = "";
        private bool _enableCaching = true;
        private int _cacheDuration = 0;

        protected string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        protected bool EnableCaching
        {
            get { return _enableCaching; }
            set { _enableCaching = value; }
        }

        protected int CacheDuration
        {
            get { return _cacheDuration; }
            set { _cacheDuration = value; }
        }

        public DataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ContructionTrackingStrings");
        }

        protected string ExecuteNonQuerystr(DbCommand cmd)
        {
            return Convert.ToString(cmd.ExecuteNonQuery());
        }

        protected int ExecuteNonQuery(DbCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }

        protected IDataReader ExecuteReader(DbCommand cmd)
        {
            return ExecuteReader(cmd, CommandBehavior.Default);
        }

        protected IDataReader ExecuteReader(DbCommand cmd, CommandBehavior behavior)
        {
            return cmd.ExecuteReader(behavior);
        }

        protected object ExecuteScalar(DbCommand cmd)
        {
            return cmd.ExecuteScalar();
        }
    }
}
