using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class SubmitQcCheckListModel
	{
        public Guid QcID { get; set; }
        public Guid ProjectID { get; set; }
        public Guid UnitID { get; set; }
        public int CheckListID { get; set; }
        public int QcTypeID { get; set; }
        public int Seq { get; set; }
    }
}

