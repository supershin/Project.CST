using System;
namespace Project.ConstructionTracking.Web.Models.QCModel
{
	public class SaveTransQCCheckListModel
	{
		// checklist info
		public Guid? QcID { get; set; }
		public Guid ProjectID { get; set; }
		public Guid UnitID { get; set; }
		public int CheckListID { get; set; }
		public int QCTypeID { get; set; }
		public int Seq { get; set; }

		// sign resource
		public Guid PeID { get; set; }
		public string? PeSignResource { get; set; }
		public Guid? PeSignResourceID { get; set; }

		// action resource
        public bool IsNotReadyInspect { get; set; }
		public string Remark { get; set; }
		public List<IFormFile> Images { get; set; } = new List<IFormFile>();

		// checklist detail
		public List<CheckListDetailInfo> CheckListItems { get; set; } = new List<CheckListDetailInfo>();

		public string ApplicationPath { get; set; }
    }

	public class CheckListDetailInfo
	{
		public int CheckListDetailID { get; set; }
		public bool ConditionPass { get; set; }
		public bool ConditionNotPass { get; set; }
		public string DetailRemark { get; set; }
		public List<IFormFile> DetailImage { get; set; } = new List<IFormFile>();
	}

    public class Resources
    {
        public string? MimeType { get; set; }
        public string? ResourceStorageBase64 { get; set; }
        public string? PhysicalPathServer { get; set; }
        public string? ResourceStoragePath { get; set; }
        public string? Directory { get; set; }
    }

    public class SignatureData
    {
        public string? MimeType { get; set; }
        public string? StorageBase64 { get; set; }

    }
}

