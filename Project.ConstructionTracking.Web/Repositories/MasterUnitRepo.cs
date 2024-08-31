using System;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MUnitModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IMasterUnitRepo
	{
		dynamic GetProjectList();

		dynamic GetUnitList(DTParamModel param, MasterUnitModel criteria);
	}

	public class MasterUnitRepo : IMasterUnitRepo
	{
        private readonly ContructionTrackingDbContext _context;

        public MasterUnitRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

		public dynamic GetProjectList()
		{
			var query = (from p in _context.tm_Project
						 where p.FlagActive == true
						 select new
						 {
							 p.ProjectID,
							 p.ProjectName
						 }).ToList();
			return query;
		}

		public dynamic GetUnitList(DTParamModel param, MasterUnitModel criteria)
		{
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            //variable = (condition) ? expressionTrue :  expressionFalse;
            criteria.strSearch = (criteria.strSearch == null) ? string.Empty : criteria.strSearch.ToString() ?? string.Empty;

            var query = from u in _context.tm_Unit
                        join p in _context.tm_Project on u.ProjectID equals p.ProjectID
                        join ut in _context.tm_UnitType on u.UnitTypeID equals ut.ID into utGroup
                        from ut in utGroup.DefaultIfEmpty()
                        join mt in _context.tm_ModelType on u.ModelTypeID equals mt.ID
                        join ext in _context.tm_Ext on u.UnitStatusID equals ext.ID
                        where u.FlagActive == true && p.FlagActive == true
                        select new
                        {
                            p.ProjectID,
                            p.ProjectCode,
                            p.ProjectName,
                            u.UnitID,
                            u.UnitCode,
                            u.UnitTypeID,
                            UnitTypeName = ut.Name,
                            UnitAddress = u.AddreessNo,
                            UnitArea = u.Area,
                            UnitStatus = u.UnitStatusID,
                            UnitStatusDesc = ext.Name,
                            UnitVendor = u.VendorID,
                            UnitPO = u.PONo,
                            UnitStartDate = u.StartDate,
                            UnitEndDate = u.EndDate,
                            u.UpdateDate
                        };

            if (!string.IsNullOrEmpty(criteria.strSearch))
            {
                query = query.Where(o =>
                    o.ProjectCode.Contains(criteria.strSearch) ||
                    o.ProjectName.Contains(criteria.strSearch) ||
                    o.UnitCode.Contains(criteria.strSearch) ||
                    o.UnitTypeName.Contains(criteria.strSearch) ||
                    o.UnitAddress.Contains(criteria.strSearch) ||
                    o.UnitStatusDesc.Contains(criteria.strSearch) ||
                    o.UnitPO.Contains(criteria.strSearch)
                );
            }

            if (criteria.ProjectID != Guid.Empty)
            {
                query = query.Where(o => o.ProjectID == criteria.ProjectID);
            }

            var result = query.Page(param.start, param.length, i => i.UpdateDate, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;

            return result.AsEnumerable().Select(e => new
            {
                ProjectID = e.ProjectID,
                ProjectCode = e.ProjectCode,
                ProjectName = e.ProjectName,
                UnitID = e.UnitID,
                UnitCode = e.UnitCode,
                UnitTypeID = e.UnitTypeID,
                UnitTypeName = e.UnitTypeName,
                UnitAddress = e.UnitAddress,
                UnitArea = e.UnitArea,
                UnitStatus = e.UnitStatus,
                UnitStatusDesc = e.UnitStatusDesc,
                UnitVendor = e.UnitVendor,
                UnitPO = e.UnitPO,
                UnitStartDate = e.UnitStartDate.ToStringDateTime(),
                UnitEndDate = e.UnitEndDate.ToStringDateTime(),
                UpdateDate = e.UpdateDate.ToStringDateTime()
            }).ToList();
        }
	}
}

