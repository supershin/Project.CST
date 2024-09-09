using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class QcActionModel
	{
        public Guid ProjectID { get; set; }
        public Guid UnitID { get; set; }
        public int QcCheckListID { get; set; }
        public int QcTypeID { get; set; }
        public int FormID { get; set; }
        public int FormQcCheckListID { get; set; }

        public Guid? QcUnitCheckListID { get; set; }
        public int? QcUnitStatusID { get; set; }
        public int? Seq { get; set; }
    }
}

