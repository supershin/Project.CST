using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class QcCheckListDetailResp
	{
		// get project and unit
		public ProjectUnitModel ProjectUnit { get; set; }

		// get qc checklist info 
        public QcCheckListResp QcCheckList { get; set; }
        public MasterQcCheckListDetailResp MasterQcCheckListDetail { get; set; }

        // qc checklist detail
        public List<QcCheckListDetail> QcCheckListDetail { get; set; } = new List<QcCheckListDetail>();

        // another value 
        public GetValueSetModel AnotherValue { get; set; }
    }

	public class ProjectUnitModel
	{
		public string ProjectName { get; set; }
		public string UnitCode { get; set; }
		public string UnitStatus { get; set; }
	}

    public class QcCheckListDetail
    {
        public int QcCheckListDetailID { get; set; }
        public int StatusID { get; set; }
        public string Remark { get; set; }
        public List<ImageQcCheckListDetail> Images { get; set; }
    }

    public class ImageQcCheckListDetail
    {
        public Guid ResourceID { get; set; }
        public string FilePath { get; set; }
    }

    public class GetValueSetModel
    {
        public string QCName { get; set; }
        public string QCNumber { get; set; }

        public Guid PEID { get; set; }
        public string PEName { get; set; }
    }
}

