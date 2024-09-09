using System;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models.QCModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IQcCheckListRepo
	{
		dynamic ValidateQcCheckList(QcActionModel model);
	}

	public class QcCheckListRepo : IQcCheckListRepo
	{
        private readonly ContructionTrackingDbContext _context;
        public QcCheckListRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

		public dynamic ValidateQcCheckList(QcActionModel model)
		{
			var checkQcUnit = from qc in _context.tr_QC_UnitCheckList
							  where qc.ProjectID == model.ProjectID
							  && qc.UnitID == model.UnitID
							  && qc.CheckListID == model.QcCheckListID
							  && qc.QCTypeID == model.QcTypeID
							  && qc.FlagActive == true
							  select new
							  {
								  qc
							  };

			if (model.Seq != null)
			{
				var query = checkQcUnit.Where(o => o.qc.Seq == model.Seq);

				
			}
			else
			{
				var query = checkQcUnit.OrderByDescending(o => o.qc.Seq);
				if (query != null)
				{

				}
				else
				{

				}
			}
        }
    }
}

