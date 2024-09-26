using System;
namespace Project.ConstructionTracking.Web.Models.GeneratePDFModel
{
	public class DataGenerateCheckListResp
	{
		public HeaderPdfData HeaderData { get; set; }
		public BodyPdfCheckListData BodyCheckListData { get; set; }
		public BodyPdfImageData BodyImageData { get; set; }
		public FooterPdfData FooterData { get; set; }
	}

	public class HeaderPdfData
	{
		//โครงการ
		public string ProjectName { get; set; }
		//ยูนิต
		public string UnitCode { get; set; }
		//ผู้รับเหมา
		public string VendorName { get; set; }
		//วิศวกรผู้ควบคุมงาน
		public string PEName { get; set; }
		//วันที่ pm อนุมัติ
		public string PMSubmitDate { get; set; }

		//ข้อมูลฟอร์ม
		public string FormName { get; set; }
		public string FormDesc { get; set; }

		public Guid UnitFormID { get; set; }

	}

	public class BodyPdfCheckListData
    {
		public List<GroupDataModel> GroupDataModels { get; set; }
	}

	public class GroupDataModel
    {
		public string GroupName { get; set; }
		public List<PackageDataModel> PackageDataModels { get; set; }
	}

	public class PackageDataModel
	{
        public string PackageName { get; set; }
		public string PackageRemark { get; set; }
        public List<CheckListDataModel> CheckListDataModels { get; set; }
	}

	public class CheckListDataModel
	{
		public string CheckListName { get; set; }
		public int StatusCheckList { get; set; }
    }

    public class BodyPdfImageData
	{
		public List<GroupImages> GroupImages { get; set; }
    }

	public class GroupImages
	{
        public string GroupName { get; set; }
        public List<ImageUpload> ImageUploads { get; set; }
    }

	public class ImageUpload
	{
		public string PathImageUrl { get; set; }
	}

	public class FooterPdfData
	{
		public VendorModel VendorData { get; set; }
		public PEModel PEData { get; set; }
		public PMModel PMData { get; set; }
	}

	public class VendorModel
	{
		public string VendorName { get; set; }
		public string VendorImageSignUrl { get; set; }
	}

	public class PEModel
	{
        public string PEName { get; set; }
        public string PEImageSignUrl { get; set; }
    }

	public class PMModel
	{
        public string PMName { get; set; }
        public string PMImageSignUrl { get; set; }
    }
}

