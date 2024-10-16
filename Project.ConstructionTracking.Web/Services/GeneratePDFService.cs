using System;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IGeneratePDFService
	{
		DataGenerateCheckListResp GetDataToGeneratePDF(DataToGenerateModel model);
        DataGenerateQCPDFResp GetDataQCToGeneratePDF(DataToGenerateModel model);
        DataDocumentModel GenerateDocumentNO(Guid projectID);
        bool SaveFileDocument(DataSaveTableResource model);
    }
	public class GeneratePDFService : IGeneratePDFService
	{
		private readonly IGeneratePDFRepo _generatePDFRepo;

        public GeneratePDFService(IGeneratePDFRepo generatePDFRepo)
		{
			_generatePDFRepo = generatePDFRepo;
		}

		public DataGenerateCheckListResp GetDataToGeneratePDF(DataToGenerateModel model)
		{
			var queryData = _generatePDFRepo.GetDataToGeneratePDF(model);

			DataGenerateCheckListResp resp = new DataGenerateCheckListResp();

			// set data resp into header 
			resp.HeaderData = new HeaderPdfData()
			{
				ProjectName = queryData.ProjectName,
				UnitCode = queryData.UnitCode,
				UnitFormID = queryData.UnitFormID,
				CompanyName = queryData.CompanyName,
                FormName = queryData.FormName,
				FormDesc = queryData.FormDesc
            };

			resp.BodyCheckListData = new BodyPdfCheckListData();
			resp.BodyImageData = new BodyPdfImageData()
			{
				GroupImages = new List<GroupImages>()
			};

			resp.FooterData = new FooterPdfData()
			{
				PEData = new PEModel(),
				PMData = new PMModel(),
				VendorData = new VendorModel()
				{
					VendorName = queryData.VendorName
				}
			};

			foreach (var work in queryData.WorkerData)
			{
				if (work.RoleID == SystemConstant.UserRole.PE)
				{
					resp.HeaderData.PEName = work.FullName;
					resp.FooterData.PEData.PEName = work.FullName;

                }
                if (work.RoleID == SystemConstant.UserRole.PM)
                {
					resp.HeaderData.PMSubmitDate = work.UpdateDate;
					resp.FooterData.PMData.PMName = work.FullName;
                }
            }

			// set data resp into checklist
			resp.BodyCheckListData = new BodyPdfCheckListData()
			{
				GroupDataModels = new List<GroupDataModel>()
			};

			foreach ( var group in queryData.DataCheckList)
			{
				GroupDataModel groupData = new GroupDataModel()
				{
					GroupName = group.FormGroupName,
					PackageDataModels = new List<PackageDataModel>()
                };

				foreach(var package in group.PackageList)
				{
					PackageDataModel packageData = new PackageDataModel()
					{
                        PackageName = package.PackageName,
                        PackageRemark = package.PackageDesc,
						CheckListDataModels = new List<CheckListDataModel>()
					};

					foreach(var checklist in package.CheckList)
					{
						CheckListDataModel checkListData = new CheckListDataModel()
						{
							CheckListName = checklist.CheckListName,
							StatusCheckList = checklist.CheckListStatus
						};

						packageData.CheckListDataModels.Add(checkListData);
                    }

					groupData.PackageDataModels.Add(packageData);
                }

                GroupImages groupImages = new GroupImages()
                {
                    GroupName = group.FormGroupName,
                    ImageUploads = new List<ImageUpload>()
                };

                foreach (var image in group.ImageCheckList)
				{
					ImageUpload imageUpload = new ImageUpload()
					{
						PathImageUrl = image.ImagePath
                    };

					groupImages.ImageUploads.Add(imageUpload);
                }

				resp.BodyImageData.GroupImages.Add(groupImages);

				resp.BodyCheckListData.GroupDataModels.Add(groupData);
            }

			// set data resp into footer
			resp.FooterData.PEData.PEImageSignUrl = queryData.SignPE;
			resp.FooterData.PMData.PMImageSignUrl = queryData.SignPM;
			resp.FooterData.VendorData.VendorImageSignUrl = queryData.SignVendor;

			return resp;
        }

        public DataGenerateQCPDFResp GetDataQCToGeneratePDF(DataToGenerateModel filterData)
        {
            var DataQCToGeneratePDF = _generatePDFRepo.GetDataQCToGeneratePDF(filterData);
            return DataQCToGeneratePDF;
        }

        public DataDocumentModel GenerateDocumentNO(Guid projectID)
        {
            DataDocumentModel resp = _generatePDFRepo.GenerateDocumentNO(projectID);
            return resp;
        }

        public bool SaveFileDocument(DataSaveTableResource model)
        {

            bool result = _generatePDFRepo.SaveFileDocument(model);
            return result;
        }
    }
}

