using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;
using static Project.ConstructionTracking.Web.Models.GeneratePDFModel.DataGenerateQCPDFResp;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using static System.Net.Mime.MediaTypeNames;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IGeneratePDFRepo
	{
	    dynamic GetDataToGeneratePDF(DataToGenerateModel model);

        DataGenerateQCPDFResp GetDataQCToGeneratePDF(DataToGenerateModel model);

        DataDocumentModel GenerateDocumentNO(Guid projectID);

        bool SaveFileDocument(DataSaveTableResource model);
    }

	public class GeneratePDFRepo : IGeneratePDFRepo
	{
        private readonly ContructionTrackingDbContext _context;
        private readonly IHostEnvironment _hosting;

        public GeneratePDFRepo(ContructionTrackingDbContext context , IHostEnvironment hostEnvironment)
		{
			_context = context;
            _hosting = hostEnvironment;
        }

        public dynamic GetDataToGeneratePDF(DataToGenerateModel model)
		{
            var query = (from truf in _context.tr_UnitForm
                         join tmp in _context.tm_Project on truf.ProjectID equals tmp.ProjectID
                         join tmu in _context.tm_Unit on truf.UnitID equals tmu.UnitID
                         join tmv in _context.tm_Vendor on truf.VendorID equals tmv.ID
                         join trcv in _context.tr_CompanyVendor on tmv.ID equals trcv.VendorID
                         join tmcv in _context.tm_CompanyVendor on trcv.CompanyVendorID equals tmcv.ID
                         join tmf in _context.tm_Form on truf.FormID equals tmf.ID
                         where truf.ProjectID == model.ProjectID
                            && truf.UnitID == model.UnitID
                            && truf.FormID == model.FormID
                            && truf.FlagActive == true
                         select new
                         {
                             // Header
                             ProjectName = tmp.ProjectName,
                             UnitCode = tmu.UnitCode,
                             UnitFormID = truf.ID,
                             VendorName = tmv.Name,
                             CompanyName = tmcv.Name,
                             FormName = tmf.Name,
                             FormDesc = tmf.Description,

                             // Worker Data
                             WorkerData = (from trufa in _context.tr_UnitFormAction
                                           join tmus in _context.tm_User on trufa.UpdateBy equals tmus.ID
                                           where trufa.UnitFormID == truf.ID
                                              && trufa.RoleID != SystemConstant.UserRole.PJM
                                              && trufa.ActionType == "submit"
                                           select new
                                           {
                                               RoleID = tmus.RoleID,
                                               FullName = tmus.FirstName + " " + tmus.LastName,
                                               UpdateDate = trufa.UpdateDate.ToStringDateTime()
                                           }).ToList(),

                             // Data Checklist
                             DataCheckList = (from tmfg in _context.tm_FormGroup
                                              where tmfg.FormID == tmf.ID && tmfg.FlagActive == true
                                              select new
                                              {
                                                  FormGroupName = tmfg.Name,
                                                  PackageList = (from trufp in _context.tr_UnitFormPackage
                                                                 join tmfp in _context.tm_FormPackage on trufp.PackageID equals tmfp.ID
                                                                 where trufp.UnitFormID == truf.ID
                                                                    && trufp.FormID == tmf.ID
                                                                    && trufp.GroupID == tmfg.ID
                                                                 select new
                                                                 {
                                                                     PackageName = tmfp.Name,
                                                                     PackageDesc = trufp.Remark,
                                                                     CheckList = (from trufcl in _context.tr_UnitFormCheckList
                                                                                  join tmfcl in _context.tm_FormCheckList on trufcl.CheckListID equals tmfcl.ID
                                                                                  where trufcl.UnitFormID == truf.ID
                                                                                     && trufcl.FormID == tmf.ID
                                                                                     && trufcl.GroupID == tmfg.ID
                                                                                     && trufcl.PackageID == tmfp.ID
                                                                                     && (trufcl.StatusID == 9 || trufcl.StatusID == 11)
                                                                                  select new
                                                                                  {
                                                                                      CheckListName = tmfcl.Name,
                                                                                      CheckListStatus = trufcl.StatusID
                                                                                  }).ToList()
                                                                 }).ToList(),

                                                  ImageCheckList = (from trufr in _context.tr_UnitFormResource
                                                                    join tmr in _context.tm_Resource on trufr.ResourceID equals tmr.ID
                                                                    where trufr.GroupID == tmfg.ID
                                                                       && trufr.RoleID == SystemConstant.UserRole.PE
                                                                       && trufr.UnitFormID == truf.ID
                                                                    select new
                                                                    {
                                                                        ImagePath = tmr.FilePath
                                                                    }).ToList()
                                              }).ToList(),

                             // Signatures
                             SignPE = (from trufa in _context.tr_UnitFormAction
                                       join trur in _context.tr_UserResource on trufa.UpdateBy equals trur.UserID
                                       join image in _context.tm_Resource on trur.ResourceID equals image.ID
                                       where trufa.UnitFormID == truf.ID && trufa.RoleID == SystemConstant.UserRole.PE
                                       && trufa.ActionType == "submit" && trufa.StatusID == 1 && trur.FlagActive == true
                                       select image.FilePath).FirstOrDefault(),

                             SignPM = (from trufa in _context.tr_UnitFormAction
                                       join trur in _context.tr_UserResource on trufa.UpdateBy equals trur.UserID
                                       join image in _context.tm_Resource on trur.ResourceID equals image.ID
                                       where trufa.UnitFormID == truf.ID && trufa.RoleID == SystemConstant.UserRole.PM
                                       && trufa.ActionType == "submit" && (trufa.StatusID == 4 || trufa.StatusID == 6)
                                       && trur.FlagActive == true
                                       select image.FilePath).FirstOrDefault(),

                             SignVendor = (from tr in _context.tm_Resource
                                           where tr.ID == truf.VendorResourceID && tr.FlagActive == true
                                           select tr.FilePath).FirstOrDefault()
                         }).FirstOrDefault();

            return query;
		}


        //public DataGenerateQCPDFResp GetDataQCToGeneratePDF(DataToGenerateModel model)
        //{

        //    var queryHeader = (from trQC in _context.tr_QC_UnitCheckList
        //                       join tmExQcType in _context.tm_Ext on trQC.QCTypeID equals tmExQcType.ID
        //                       join tmp in _context.tm_Project on trQC.ProjectID equals tmp.ProjectID
        //                       join tmu in _context.tm_Unit on trQC.UnitID equals tmu.UnitID
        //                       join tmpeunit in _context.tr_PE_Unit on tmu.UnitID equals tmpeunit.UnitID
        //                       join tmcv in _context.tm_CompanyVendor on tmu.CompanyVendorID equals tmcv.ID
        //                       join trfqc in _context.tr_Form_QCCheckList on trQC.CheckListID equals trfqc.CheckListID
        //                       join tmf in _context.tm_Form on trfqc.FormID equals tmf.ID
        //                       join tmuserqc in _context.tm_User on trQC.UpdateBy equals tmuserqc.ID
        //                       join tmuserpe in _context.tm_User on tmpeunit.UserID equals tmuserpe.ID
        //                       where trQC.ProjectID == model.ProjectID
        //                          && trQC.UnitID == model.UnitID
        //                          && trQC.ID == model.QCUnitCheckListID
        //                       select new
        //                       {
        //                           QCName = tmExQcType.Name,
        //                           ProjectName = tmp.ProjectName,
        //                           UnitCode = tmu.UnitCode,
        //                           CompanyVenderName = tmcv.Name,
        //                           FormName = tmf.Name,
        //                           QCInspectorName = tmuserqc.FirstName + " " + tmuserqc.LastName,
        //                           PEInspectorName = tmuserpe.FirstName + " " + tmuserpe.LastName,
        //                           QCStatus = trQC.QCStatusID
        //                       }).FirstOrDefault();


        //    var queryBody = (from t1 in _context.tr_QC_UnitCheckList_Defect
        //                     join t2 in _context.tm_DefectArea on t1.DefectAreaID equals t2.ID into defectAreaGroup
        //                     from t2 in defectAreaGroup.DefaultIfEmpty()
        //                     join t3 in _context.tm_DefectType on t1.DefectTypeID equals t3.ID into defectTypeGroup
        //                     from t3 in defectTypeGroup.DefaultIfEmpty()
        //                     join t4 in _context.tm_DefectDescription on t1.DefectDescriptionID equals t4.ID into defectDescriptionGroup
        //                     from t4 in defectDescriptionGroup.DefaultIfEmpty()
        //                     where t1.QCUnitCheckListID == model.QCUnitCheckListID
        //                     select new BodyQCPdfListDefectData
        //                     {
        //                         RefSeqDefectText = t1.Seq.ToString(),
        //                         DefectAreaText = t2.Name,
        //                         DefectTypeText = t3.Name,
        //                         DefectDescriptionText = t4.Name,
        //                         IsMajorDefectText = t1.IsMajorDefect == true ? "Y" : "",
        //                         DefectRemark = t1.Remark,
        //                         DefectStatus = t1.StatusID,
        //                         ListImageDefact = (from t5 in _context.tr_QC_UnitCheckList_Resource
        //                                            join t6 in _context.tm_Resource on t5.ResourceID equals t6.ID
        //                                            where t5.QCUnitCheckListID == model.QCUnitCheckListID && t5.DefectID == t1.ID
        //                                            select new ListImageDefact
        //                                            {
        //                                                PathImageUrl = t6.FilePath
        //                                            }).ToList()
        //                     }).ToList();

        //    var refSeqCounts = _context.tr_QC_UnitCheckList_Defect
        //                    .Where(t1 => t1.QCUnitCheckListID == model.QCUnitCheckListID && t1.FlagActive == true)
        //                    .GroupBy(t1 => t1.RefSeq)
        //                    .Select(g => new
        //                    {
        //                        RefSeq = g.Key,            // The grouped RefSeq value
        //                        RefSeqCnt = g.Count()      // Count of occurrences per RefSeq
        //                    })
        //                    .ToList();

        //    var statusCounts = _context.tr_QC_UnitCheckList_Defect
        //                    .Where(t1 => t1.QCUnitCheckListID == model.QCUnitCheckListID && t1.FlagActive == true)
        //                    .GroupBy(t1 => 1)  // Group by constant to avoid grouping by status
        //                    .Select(g => new
        //                    {
        //                        Cnt_Pass = g.Count(t1 => t1.StatusID == 27),  // Count where StatusID = 27
        //                        Cnt_NotPass = g.Count(t1 => t1.StatusID == 28) // Count where StatusID = 28
        //                    })
        //                    .FirstOrDefault();

        //    var cntAll = _context.tr_QC_UnitCheckList_Defect
        //              .Where(t1 => t1.QCUnitCheckListID == model.QCUnitCheckListID && t1.FlagActive == true)
        //              .Count();





        //    if (queryHeader != null)
        //    {

        //        var resultHeader = new HeaderQCPdfData
        //        {
        //            QCName = queryHeader.QCName,
        //            ProjectName = queryHeader.ProjectName,
        //            UnitCode = queryHeader.UnitCode,
        //            CompanyVenderName = queryHeader.CompanyVenderName,
        //            FormName = queryHeader.FormName,
        //            QCInspectorName = queryHeader.QCInspectorName,
        //            PEInspectorName = queryHeader.PEInspectorName,
        //            QCStatusText = queryHeader.QCStatus == SystemConstant.UnitQCStatus.Pass ? SystemConstant.UnitQCStatusText.Pass
        //                         : queryHeader.QCStatus == SystemConstant.UnitQCStatus.NotPass ? SystemConstant.UnitQCStatusText.NotPass
        //                         : queryHeader.QCStatus == SystemConstant.UnitQCStatus.IsNotReadyInspect ? SystemConstant.UnitQCStatusText.IsNotReadyInspect
        //                         : queryHeader.QCStatus == SystemConstant.UnitQCStatus.IsPassCondition ? SystemConstant.UnitQCStatusText.IsPassCondition
        //                         : queryHeader.QCStatus == SystemConstant.UnitQCStatus.InProgress ? SystemConstant.UnitQCStatusText.InProgress
        //                         : "ไม่พบสถานะ"
        //        };

        //        if (refSeqCounts != null && statusCounts != null && cntAll > 0)
        //        {
        //            var resultSummary = new SummaryQCPdfData
        //                SumAllDefect = cntAll,
        //                SumPassDefect = statusCounts.Cnt_Pass,
        //                SumNotPassDefect = statusCounts.Cnt_NotPass
        //                //make list refSeqCounts use ListCalDefectBySeq here please
        //        }

        //        return new DataGenerateQCPDFResp
        //        {
        //            HeaderQCData = resultHeader,
        //            BodyListDefectQCData = queryBody
        //        };
        //    }
        //    else 
        //    {
        //        return null;
        //    }
        //}


        public DataGenerateQCPDFResp GetDataQCToGeneratePDF(DataToGenerateModel model)
        {
            var queryHeaderFooter = (from trQC in _context.tr_QC_UnitCheckList
                               join tmExQcType in _context.tm_Ext on trQC.QCTypeID equals tmExQcType.ID
                               join tmp in _context.tm_Project on trQC.ProjectID equals tmp.ProjectID
                               join tmu in _context.tm_Unit on trQC.UnitID equals tmu.UnitID
                               join tmpeunit in _context.tr_PE_Unit on tmu.UnitID equals tmpeunit.UnitID
                               join tmcv in _context.tm_CompanyVendor on tmu.CompanyVendorID equals tmcv.ID
                               join trfqc in _context.tr_Form_QCCheckList on trQC.CheckListID equals trfqc.CheckListID
                               join tmf in _context.tm_Form on trfqc.FormID equals tmf.ID
                               join tmuserqc in _context.tm_User on trQC.UpdateBy equals tmuserqc.ID
                               join tmuserpe in _context.tm_User on tmpeunit.UserID equals tmuserpe.ID
                               join truserresourc in _context.tr_UserResource on trQC.UpdateBy equals truserresourc.UserID
                               join qcsignpath in _context.tm_Resource on trQC.PESignResourceID equals qcsignpath.ID
                               join pesignpath in _context.tm_Resource on truserresourc.ResourceID equals pesignpath.ID
                               where trQC.ProjectID == model.ProjectID
                                  && trQC.UnitID == model.UnitID
                                  && trQC.ID == model.QCUnitCheckListID
                               select new
                               {
                                   QCName = tmExQcType.Name,
                                   ProjectName = tmp.ProjectName,
                                   UnitCode = tmu.UnitCode,
                                   CompanyVenderName = tmcv.Name,
                                   FormName = tmf.Name,
                                   QCInspectorName = tmuserqc.FirstName + " " + tmuserqc.LastName,
                                   PEInspectorName = tmuserpe.FirstName + " " + tmuserpe.LastName,
                                   QCStatus = trQC.QCStatusID,
                                   QCSignPath = qcsignpath.FilePath,
                                   PESignPath = pesignpath.FilePath
                               }).FirstOrDefault();

            var queryBody = (from t1 in _context.tr_QC_UnitCheckList_Defect
                             join t2 in _context.tm_DefectArea on t1.DefectAreaID equals t2.ID into defectAreaGroup
                             from t2 in defectAreaGroup.DefaultIfEmpty()
                             join t3 in _context.tm_DefectType on t1.DefectTypeID equals t3.ID into defectTypeGroup
                             from t3 in defectTypeGroup.DefaultIfEmpty()
                             join t4 in _context.tm_DefectDescription on t1.DefectDescriptionID equals t4.ID into defectDescriptionGroup
                             from t4 in defectDescriptionGroup.DefaultIfEmpty()
                             where t1.QCUnitCheckListID == model.QCUnitCheckListID
                             select new BodyQCPdfListDefectData
                             {
                                 RefSeqDefectText = t1.Seq.ToString(),
                                 DefectAreaText = t2.Name,
                                 DefectTypeText = t3.Name,
                                 DefectDescriptionText = t4.Name,
                                 IsMajorDefectText = t1.IsMajorDefect == true ? "Y" : "",
                                 DefectRemark = t1.Remark,
                                 DefectStatus = t1.StatusID,
                                 ListImageDefact = (from t5 in _context.tr_QC_UnitCheckList_Resource
                                                    join t6 in _context.tm_Resource on t5.ResourceID equals t6.ID
                                                    where t5.QCUnitCheckListID == model.QCUnitCheckListID && t5.DefectID == t1.ID
                                                    select new ListImageDefact
                                                    {
                                                        PathImageUrl = t6.FilePath
                                                    }).ToList()
                             }).ToList();


            // RefSeq counts
            var refSeqCounts = _context.tr_QC_UnitCheckList_Defect
                .Where(t1 => t1.QCUnitCheckListID == model.QCUnitCheckListID && t1.FlagActive == true)
                .GroupBy(t1 => t1.RefSeq)
                .Select(g => new ListCalDefectBySeq
                {
                    RefSeq = g.Key,
                    RefSeqCnt = g.Count()
                }).ToList();

            // Status counts (Pass and NotPass)
            var statusCounts = _context.tr_QC_UnitCheckList_Defect
                .Where(t1 => t1.QCUnitCheckListID == model.QCUnitCheckListID && t1.FlagActive == true)
                .GroupBy(t1 => 1) // Single group to calculate total counts
                .Select(g => new
                {
                    Cnt_Pass = g.Count(t1 => t1.StatusID == 27),  // StatusID = 27 is Pass
                    Cnt_NotPass = g.Count(t1 => t1.StatusID == 28) // StatusID = 28 is Not Pass
                })
                .FirstOrDefault();

            // Total defect count
            var cntAll = _context.tr_QC_UnitCheckList_Defect
                .Where(t1 => t1.QCUnitCheckListID == model.QCUnitCheckListID && t1.FlagActive == true)
                .Count();


            if (queryHeaderFooter != null && refSeqCounts != null && statusCounts != null && cntAll > 0)
            {

                var resultHeader = new HeaderQCPdfData
                {
                    QCName = queryHeaderFooter.QCName,
                    ProjectName = queryHeaderFooter.ProjectName,
                    UnitCode = queryHeaderFooter.UnitCode,
                    CompanyVenderName = queryHeaderFooter.CompanyVenderName,
                    FormName = queryHeaderFooter.FormName,
                    QCInspectorName = queryHeaderFooter.QCInspectorName,
                    PEInspectorName = queryHeaderFooter.PEInspectorName,
                    QCStatus = queryHeaderFooter.QCStatus,
                    QCStatusText = queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.Pass ? SystemConstant.UnitQCStatusText.Pass
                                 : queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.NotPass ? SystemConstant.UnitQCStatusText.NotPass
                                 : queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.IsNotReadyInspect ? SystemConstant.UnitQCStatusText.IsNotReadyInspect
                                 : queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.IsPassCondition ? SystemConstant.UnitQCStatusText.IsPassCondition
                                 : queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.InProgress ? SystemConstant.UnitQCStatusText.InProgress
                                 : "ไม่พบสถานะ"
                };

                // Prepare the summary data
                var resultSummary = new SummaryQCPdfData
                {
                    SumAllDefect = cntAll,
                    SumPassDefect = statusCounts.Cnt_Pass,
                    SumNotPassDefect = statusCounts.Cnt_NotPass,
                    CalDefectBySeq = refSeqCounts // List of RefSeq counts
                };

                var resultFooter = new FooterQCPdfData
                {
                    PESignaturePathImageUrl = queryHeaderFooter.PEInspectorName,
                    QCSignaturePathImageUrl = queryHeaderFooter.QCInspectorName
                };

                // Return the final response with header, body, and summary data
                return new DataGenerateQCPDFResp
                {
                    HeaderQCData = resultHeader,
                    BodyListDefectQCData = queryBody,
                    SummaryQCData = resultSummary,
                    FooterQCData = resultFooter
                };
            }
            else
            {
                return null;
            }
        }

        public DataDocumentModel GenerateDocumentNO(Guid projectID)
        {

            DataDocumentModel dataDocumentModel = new DataDocumentModel();


            tm_Project? project = _context.tm_Project.Where(o => o.ProjectID == projectID && o.FlagActive == true).FirstOrDefault();

            string documentPrefix = "";

            string formatYear = DateTime.Now.ToString("yyyy");
            int changeFormat = Int32.Parse(formatYear) + 543;
            formatYear = changeFormat.ToString().Substring(2);

            if (project != null)
            {
                documentPrefix = "C" + project.ProjectCode + formatYear + DateTime.Now.ToString("MM");
            }

            tr_Document? document = _context.tr_Document
                                .Where(o => o.DocumentPrefix == documentPrefix)
                                .OrderByDescending(o => o.UpdateDate)
                                .FirstOrDefault();

            int approveString;

            if (document == null)
            {
                approveString = 0;
            }
            else
            {
                approveString = Int32.Parse(document.DocuementRunning);
            }

            approveString += 1;


            string documentRunning = approveString.ToString("D3");
            string documentNo = documentPrefix + "-" + documentRunning;

            dataDocumentModel.documentRunning = documentRunning;
            dataDocumentModel.documentPrefix = documentPrefix;
            dataDocumentModel.documentNo = documentNo;

            return dataDocumentModel;  // Return the populated model
        }

        public bool SaveFileDocument(DataSaveTableResource model)
        {
            var newResource = new tm_Resource
            {
                ID = Guid.NewGuid(),
                FileName = model.FileName,
                FilePath = model.FilePath,
                MimeType = "file/pdf",
                FlagActive = true,
                CreateDate = DateTime.Now,
                CreateBy = model.UserID,
                UpdateDate = DateTime.Now,
                UpdateBy = model.UserID
            };
            _context.tm_Resource.Add(newResource);

            var newFormResource = new tr_Document
            {
                ID = Guid.NewGuid(),
                UnitFormID = model.UnitFormID,
                ResourceID = newResource.ID,
                DocumentNo = model.documentNo,
                DocumentPrefix = model.documentPrefix,
                DocuementRunning = model.documentRunning,
                FlagActive = true,
                CreateDate = DateTime.Now,
                CreateBy = model.UserID,
                UpdateDate = DateTime.Now,
                UpdateBy = model.UserID
            };
            _context.tr_Document.Add(newFormResource);

            _context.SaveChanges();
            return true;
        }


        public string GenerateQCPDF(Guid guid, DataGenerateCheckListResp dataGenerate, DataDocumentModel genDocumentNo)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var fontPath = _hosting.ContentRootPath + "/wwwroot/lib/fonts/BrowalliaUPC.ttf";

            FontManager.RegisterFont(System.IO.File.OpenRead(fontPath));

            var imageHeader = Directory.GetCurrentDirectory() + "/wwwroot/img/img1.png";
            var imageBox = Directory.GetCurrentDirectory() + "/wwwroot/img/box.png";
            var imageCheckBox = Directory.GetCurrentDirectory() + "/wwwroot/img/checkbox.png";
            var imageCheck = Directory.GetCurrentDirectory() + "/wwwroot/img/check.png";

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginTop(1, Unit.Centimetre);
                    page.MarginBottom(1, Unit.Centimetre);
                    page.MarginLeft(1, Unit.Centimetre);
                    page.MarginRight(1, Unit.Centimetre);

                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(16));
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(TextStyle
                               .Default
                               .FontFamily("BrowalliaUPC")
                               .FontSize(12));

                    page.Header().Column(column =>
                    {
                        IContainer DefaultCellStyle(IContainer container, string backgroundColor)
                        {
                            return container
                                .Border(1)
                                .BorderColor(Colors.Black)
                                .Background(backgroundColor)
                                .PaddingVertical(1)
                                .PaddingHorizontal(3)
                                .AlignCenter()
                                .AlignMiddle();
                        }

                        column.Item().Column(col1 =>
                        {
                            col1.Item().PaddingVertical(5).Width(150).Image(imageHeader);
                        });

                        column.Item().Border(1).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(3);
                            });

                            table.Cell().Row(1).Column(1).ColumnSpan(5).Element(CellStyle).AlignLeft().Text("เลขที่ใบตรวจ: " + genDocumentNo.documentNo).FontColor("#FF0000");

                            table.Cell().Row(2).Column(1).Element(CellStyle).AlignLeft().Text("โครงการ ");
                            table.Cell().Row(2).Column(2).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.ProjectName);
                            table.Cell().Row(2).Column(3).ColumnSpan(3).Element(CellStyle).AlignLeft().Text("วันที่ " + dataGenerate.HeaderData.PMSubmitDate);

                            table.Cell().Row(3).Column(1).Element(CellStyle).AlignLeft().Text("แปลงที่ ");
                            table.Cell().Row(3).Column(2).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.UnitCode);
                            table.Cell().Row(3).Column(3).RowSpan(3).Element(CellStyle).AlignLeft().Text("การตรวจ QC ");
                            table.Cell().Row(3).Column(4).Element(CellStyle).Width(15).Image(imageBox);
                            table.Cell().Row(3).Column(5).Element(CellStyle).AlignLeft().Text("ผ่านการตรวจจาก QC แล้ว");

                            table.Cell().Row(4).Column(1).Element(CellStyle).AlignLeft().Text("ผู้รับเหมา ");
                            table.Cell().Row(4).Column(2).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.CompanyName);
                            table.Cell().Row(4).Column(4).Element(CellStyle).Width(15).Image(imageBox);
                            table.Cell().Row(4).Column(5).Element(CellStyle).AlignLeft().Text("งวดนี้ไม่มีการตรวจ QC");

                            table.Cell().Row(5).Column(1).Element(CellStyle).AlignLeft().Text("ผู้ควบคุมงาน ");
                            table.Cell().Row(5).Column(2).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.PEName);

                            table.Cell().Row(6).Column(1).Element(x => DefaultCellStyle(x, "#00FF00")).AlignLeft().Text(dataGenerate.HeaderData.FormName).Bold();
                            table.Cell().Row(6).Column(2).ColumnSpan(4).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.FormDesc);

                            // you can extend existing styles by creating additional methods
                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White);

                        });
                    });

                    int countRow = 2;
                    page.Content().PaddingVertical(4).Column(col1 =>
                    {
                        IContainer DefaultCellStyle(IContainer container, string backgroundColor)
                        {
                            return container
                                .Border(1)
                                .BorderColor(Colors.Black)
                                .Background(backgroundColor)
                                .PaddingVertical(1)
                                .PaddingHorizontal(3)
                                .AlignCenter()
                                .AlignMiddle();
                        }

                        col1.Item().Table(table2 =>
                        {
                            table2.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(4);
                            });

                            table2.Cell().Row(1).Column(1).ColumnSpan(2).Element(CellStyle).AlignCenter().Text("รายการตรวจ");
                            table2.Cell().Row(1).Column(3).ColumnSpan(3).Element(CellStyle).AlignCenter().Text("ผลการตรวจ");
                            table2.Cell().Row(1).Column(6).RowSpan(2).Element(CellStyle).AlignCenter().Text("ความเห็น");

                            for (int group = 0; group < dataGenerate.BodyCheckListData.GroupDataModels.Count; group++)
                            {
                                table2.Cell().Row((uint)(countRow)).Column(1).ColumnSpan(2).Element(x => DefaultCellStyle(x, Colors.Grey.Medium)).AlignLeft().Text(dataGenerate.BodyCheckListData.GroupDataModels[group].GroupName).WrapAnywhere();
                                if (group == 0)
                                {
                                    table2.Cell().Row((uint)(countRow)).Column(3).Element(CellStyle).Text("ผ่าน");
                                    table2.Cell().Row((uint)(countRow)).Column(4).Element(CellStyle).Text("ไม่ผ่าน");
                                    table2.Cell().Row((uint)(countRow)).Column(5).Element(CellStyle).Text("ไม่มีรายการนี้").WrapAnywhere();
                                }
                                else
                                {
                                    table2.Cell().Row((uint)(countRow)).Column(3).Element(CellStyle).Text("");
                                    table2.Cell().Row((uint)(countRow)).Column(4).Element(CellStyle).Text("");
                                    table2.Cell().Row((uint)(countRow)).Column(5).Element(CellStyle).Text("");
                                    table2.Cell().Row((uint)(countRow)).Column(6).Element(CellStyle).Text("");
                                }

                                for (int package = 0; package < dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels.Count; package++)
                                {
                                    int number = countRow;

                                    if (package == 0)
                                    {
                                        number += 1;
                                    }

                                    table2.Cell().Row((uint)(number)).Column(1).RowSpan((uint)dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels.Count).Element(CellStyle).AlignLeft().Text(dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].PackageName).WrapAnywhere();

                                    for (int check = 0; check < dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels.Count; check++)
                                    {
                                        table2.Cell().Row((uint)(number + check)).Column(2).Element(CellStyle).AlignLeft().Text(dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels[check].CheckListName).WrapAnywhere();
                                        {

                                            if (dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels[check].StatusCheckList == SystemConstant.CheckList_Status.PASS)
                                            {
                                                table2.Cell().Row((uint)(number + check)).Column(3).Element(CellStyle).Width(15).Image(imageCheck); // icon
                                                table2.Cell().Row((uint)(number + check)).Column(4).Element(CellStyle).Width(15);
                                                table2.Cell().Row((uint)(number + check)).Column(5).Element(CellStyle).Width(15);
                                            }
                                            else if (dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels[check].StatusCheckList == SystemConstant.CheckList_Status.NOTPASS)
                                            {
                                                table2.Cell().Row((uint)(number + check)).Column(3).Element(CellStyle).Width(15);
                                                table2.Cell().Row((uint)(number + check)).Column(4).Element(CellStyle).Width(15).Image(imageCheck);// icon
                                                table2.Cell().Row((uint)(number + check)).Column(5).Element(CellStyle).Width(15);
                                            }
                                            else if (dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels[check].StatusCheckList == SystemConstant.CheckList_Status.NOWORK)
                                            {
                                                table2.Cell().Row((uint)(number + check)).Column(3).Element(CellStyle).Width(15);
                                                table2.Cell().Row((uint)(number + check)).Column(4).Element(CellStyle).Width(15);
                                                table2.Cell().Row((uint)(number + check)).Column(5).Element(CellStyle).Width(15).Image(imageCheck); // icon
                                            }
                                        }
                                    }
                                    table2.Cell().Row((uint)(number)).Column(6).RowSpan((uint)dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels.Count).Element(CellStyle).AlignLeft().Text(dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].PackageRemark).WrapAnywhere();
                                    int count = dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels.Count;
                                    countRow = number + count;
                                }
                            }

                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White);
                        });

                        col1.Item().Column(col1 =>
                        {
                            col1.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        });

                        col1.Item().Grid(grid =>
                        {
                            grid.VerticalSpacing(15);
                            grid.HorizontalSpacing(15);
                            grid.AlignLeft();
                            grid.Columns(12);

                            grid.Item(12).Text("รูปถ่ายงานที่เสร็จแล้ว").Bold().BackgroundColor("#6ce4ff");

                            for (int i = 0; i < dataGenerate.BodyImageData.GroupImages.Count; i++)
                            {
                                grid.Item(12).Text(dataGenerate.BodyImageData.GroupImages[i].GroupName).Bold().Underline();

                                for (int a = 0; a < dataGenerate.BodyImageData.GroupImages[i].ImageUploads.Count; a++)
                                {
                                    string pathImage = dataGenerate.BodyImageData.GroupImages[i].ImageUploads[a].PathImageUrl;
                                    //var imgPath = Directory.GetCurrentDirectory() + "/images/works" + i + ".jpg";
                                    var imgPath = _hosting.ContentRootPath + "/wwwroot/" + pathImage;
                                    if (System.IO.File.Exists(imgPath))
                                    {
                                        using var img = new FileStream(imgPath, FileMode.Open);

                                        grid.Item(6).Border(0.5f).Width(250).Height(250).Image(img);
                                    }
                                }

                            }
                        });
                    });

                    page.Footer().Table(table2 =>
                    {
                        string pathVendor = Directory.GetCurrentDirectory() + "/wwwroot/" + dataGenerate.FooterData.VendorData.VendorImageSignUrl;
                        var signVendor = new FileStream(pathVendor, FileMode.Open);

                        string pathPe = _hosting.ContentRootPath + "/" + dataGenerate.FooterData.PEData.PEImageSignUrl;
                        var signPe = new FileStream(pathPe, FileMode.Open);

                        string pathPm = _hosting.ContentRootPath + "/" + dataGenerate.FooterData.PMData.PMImageSignUrl;
                        var signPm = new FileStream(pathPm, FileMode.Open);

                        table2.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                        });

                        table2.Cell().Row(1).Column(1).AlignCenter().Width(60).Image(signVendor);
                        table2.Cell().Row(2).Column(1).AlignCenter().Text("ผู้รับเหมา");
                        table2.Cell().Row(3).Column(1).AlignCenter().Text("( " + dataGenerate.FooterData.VendorData.VendorName + " )");

                        table2.Cell().Row(1).Column(2).AlignCenter().Width(60).Image(signPe);
                        table2.Cell().Row(2).Column(2).AlignCenter().Text("วิศวกรผู้ควบคุมงาน");
                        table2.Cell().Row(3).Column(2).AlignCenter().Text("( " + dataGenerate.FooterData.PEData.PEName + " )");

                        table2.Cell().Row(1).Column(3).AlignCenter().Width(60).Image(signPm);
                        table2.Cell().Row(2).Column(3).AlignCenter().Text("Project Manager");
                        table2.Cell().Row(3).Column(3).AlignCenter().Text("( " + dataGenerate.FooterData.PMData.PMName + " )");

                        //QC
                        //table2.Cell().Row(1).Column(4).AlignCenter().Width(60).Image("");
                        table2.Cell().Row(2).Column(4).AlignCenter().Text("Quality Control (QC)");
                        table2.Cell().Row(3).Column(4).AlignCenter().Text("(                  )");


                        // Page number 
                        table2.Cell().Row(4).ColumnSpan(4).AlignRight().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                    });
                });
            });

            string returnPath = "Upload/temp/" + "DocumentNo" + "-" + guid + ".pdf";
            document.GeneratePdf(returnPath);
            //document.ShowInPreviewer();

            return returnPath;
        }
    }
}

