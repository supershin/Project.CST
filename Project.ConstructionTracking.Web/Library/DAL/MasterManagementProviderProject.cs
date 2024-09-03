using Microsoft.Extensions.Configuration;
using Project.ConstructionTracking.Web.Models;
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
            Entity.CreateDate = Commons.FormatExtension.ConvertToDateString(reader["CreateDate"]);
            Entity.CreateBy = Commons.FormatExtension.Nulltoint(reader["CreateBy"]);
            Entity.UpdateDate = Commons.FormatExtension.ConvertToDateString(reader["UpdateDate"]);
            Entity.UpdateBy = Commons.FormatExtension.Nulltoint(reader["UpdateBy"]);
            return Entity;
        }
        #endregion
    }
}
