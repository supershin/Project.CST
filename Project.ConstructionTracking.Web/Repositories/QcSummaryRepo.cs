using System;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models.MProjectModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IQcSummaryRepo
	{
		dynamic GetQcSummaryList(Guid projectID, Guid unitID);
	}

	public class QcSummaryRepo : IQcSummaryRepo
	{
		private readonly ContructionTrackingDbContext _context;
		public QcSummaryRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

		public dynamic GetQcSummaryList(Guid projectID, Guid unitID)
		{
			var query = (from u in _context.tm_Unit
						 join pmf in _context.tr_ProjectModelForm on u.ModelTypeID equals pmf.ModelTypeID
						 join ft in _context.tm_FormType on pmf.FormTypeID equals ft.ID
						 join e in _context.tm_Ext on u.UnitStatusID equals e.ID
						 where u.ProjectID == projectID && u.UnitID == unitID && u.FlagActive == true
						 select new
						 {
							 ProjectID = u.ProjectID,
							 UnitID = u.UnitID,
							 ModelType = u.ModelTypeID,
							 UnitCode = u.UnitCode,
							 UnitStatus = u.UnitStatusID,
							 UnitStatusDesc = e.Name,
							 FormTypeID = ft.ID,
							 FormTypeName = ft.Name,
							 ListQcSummary = (from f in _context.tm_Form
											  join fql in _context.tr_Form_QCCheckList on f.ID equals fql.FormID into tfqlGroup
											  from fql in tfqlGroup.DefaultIfEmpty()
											  join qcl in _context.tm_QC_CheckList on fql.CheckListID equals qcl.ID
											  join e in _context.tm_Ext on qcl.QCTypeID equals e.ID
											  where f.FormTypeID == ft.ID
											  select new
											  {
												  QcCheckListID = qcl.ID,
												  QcTypeID = qcl.QCTypeID,
												  QcTypeName = e.Name,
                                                  FormQcCheckList = fql.ID,
                                                  FormID = fql.FormID,
                                              }).ToList()
						 }).FirstOrDefault();

			return query;
        }

	}
}

