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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using static Project.ConstructionTracking.Web.Models.PJMApproveModel;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.EntityFrameworkCore;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IGeneratePDFRepo
	{
	    dynamic GetDataToGeneratePDF(DataToGenerateModel model);

        DataGenerateQCPDFResp GetDataQCToGeneratePDF(DataToGenerateModel model);

        DataDocumentModel GenerateDocumentNO(Guid projectID);

        bool SaveFileDocument(DataSaveTableResource model);

        string GenerateQCPDF(Guid guid, DataGenerateQCPDFResp dataForGenPdf, DataDocumentModel genDocumentNo);

        //qc1-4
        DataGenerateQCPDFResp GetDataQC1To4ForGeneratePDF(DataToGenerateModel model);
        string GenerateQCPDF2(Guid guid, DataGenerateQCPDFResp dataQCGenerate, DataDocumentModel genDocumentNo);
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
                               join truserresourc in _context.tr_UserResource.Where(ur => ur.FlagActive == true) on trQC.UpdateBy equals truserresourc.UserID into truserresourcJoin
                               from truserresourc in truserresourcJoin.DefaultIfEmpty()
                               join pesignpath in _context.tm_Resource on trQC.PESignResourceID equals pesignpath.ID
                               join qcsignpath in _context.tm_Resource on truserresourc.ResourceID equals qcsignpath.ID
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
                                   Seq = trQC.Seq,
                                   UpdateDate = trQC.UpdateDate,
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
                             where t1.QCUnitCheckListID == model.QCUnitCheckListID && t1.FlagActive == true
                             //orderby t1.StatusID == 28 , t1.StatusID == 27 , t1.ID
                             select new BodyQCPdfListDefectData
                             {
                                 RefSeqDefectText = t2.Name,
                                 DefectAreaText = t3.Name,
                                 DefectTypeText = t4.Name,
                                 DefectDescriptionText = t4.Name,
                                 IsMajorDefectText = t1.IsMajorDefect == true ? "Y" : "",
                                 DefectRemark = t1.Remark,
                                 DefectStatus = t1.StatusID,
                                 ListImageDefact = (from t5 in _context.tr_QC_UnitCheckList_Resource
                                                    join t6 in _context.tm_Resource on t5.ResourceID equals t6.ID
                                                    where t5.QCUnitCheckListID == model.QCUnitCheckListID && t5.DefectID == t1.ID && t5.IsSign == null
                                                    select new ListImageDefact
                                                    {
                                                        PathImageUrl = t6.FilePath
                                                    }).ToList()
                             }).OrderByDescending(e=>e.DefectStatus).ToList();


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
                    Seq = FormatExtension.NullToString(queryHeaderFooter.Seq),
                    UpdateDate = FormatExtension.FormatDateToDayMonthNameYearTime(queryHeaderFooter.UpdateDate),
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
                    PESignaturePathImageUrl = queryHeaderFooter.PESignPath,
                    QCSignaturePathImageUrl = queryHeaderFooter.QCSignPath
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
                documentPrefix = "QC" + project.ProjectCode + formatYear + DateTime.Now.ToString("MM");
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
                QCUnitCheckListID = model.QCUnitCheckListID,
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


        //public string GenerateQCPDF(Guid guid, DataGenerateQCPDFResp dataQCGenerate, DataDocumentModel genDocumentNo)
        //{
        //    QuestPDF.Settings.License = LicenseType.Community;
        //    var fontPath = _hosting.ContentRootPath + "/wwwroot/lib/fonts/BrowalliaUPC.ttf";

        //    FontManager.RegisterFont(System.IO.File.OpenRead(fontPath));

        //    var imageHeader = Directory.GetCurrentDirectory() + "/wwwroot/img/img1.png";
        //    var imageBox = Directory.GetCurrentDirectory() + "/wwwroot/img/box.png";
        //    var imageCheckBox = Directory.GetCurrentDirectory() + "/wwwroot/img/checkbox.png";
        //    var imageCheck = Directory.GetCurrentDirectory() + "/wwwroot/img/check.png";

        //    var document = QuestPDF.Fluent.Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Size(PageSizes.A4);
        //            page.MarginTop(1, Unit.Centimetre);
        //            page.MarginBottom(1, Unit.Centimetre);
        //            page.MarginLeft(1, Unit.Centimetre);
        //            page.MarginRight(1, Unit.Centimetre);

        //            page.PageColor(Colors.White);
        //            page.DefaultTextStyle(x => x.FontSize(16));
        //            page.PageColor(Colors.White);
        //            page.DefaultTextStyle(TextStyle
        //                       .Default
        //                       .FontFamily("BrowalliaUPC")
        //                       .FontSize(12));

        //            page.Header().Column(column =>
        //            {
        //                column.Item().Column(col1 =>
        //                {
        //                    col1.Item().PaddingVertical(5).Width(150).Image(imageHeader);
        //                });

        //                column.Item().Border(1).Table(table =>
        //                {
        //                    table.ColumnsDefinition(columns =>
        //                    {
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                    });


        //                    table.Cell().Row(1).Column(1).ColumnSpan(4).AlignLeft().Text(" ผลการตรวจ " + dataQCGenerate.HeaderQCData?.QCName).FontSize(20).Bold();
        //                    table.Cell().Row(1).Column(5).ColumnSpan(2).AlignLeft().Text("ครั้งที่ " + dataQCGenerate.HeaderQCData?.Seq).FontSize(20).Bold();

        //                    table.Cell().Row(2).Column(1).ColumnSpan(2).AlignLeft().Text(text =>
        //                    {
        //                        text.Span("  ตรวจใน ");
        //                        text.Span(dataQCGenerate.HeaderQCData?.FormName).Underline();
        //                    });
        //                    table.Cell().Row(2).Column(3).ColumnSpan(2).AlignLeft().Text(text =>
        //                    {
        //                        string checkboxSymbol = dataQCGenerate.HeaderQCData?.QCStatus == 4 ? "☑" : "☐";
        //                        text.Span(checkboxSymbol + " ").FontColor("#FF0000").FontSize(12);
        //                        text.Span("ไม่พร้อมให้ตรวจ").FontColor("#FF0000").FontSize(12);
        //                    });
        //                    table.Cell().Row(2).Column(5).ColumnSpan(2).AlignLeft().Text(text =>
        //                    {
        //                        text.Span("วันที่ตรวจ ");
        //                        text.Span(dataQCGenerate.HeaderQCData?.UpdateDate).Underline();
        //                    });

        //                    table.Cell().Row(3).Column(1).ColumnSpan(2).AlignLeft().Text(text =>
        //                    {
        //                        text.Span("  โครงการ ");
        //                        text.Span(dataQCGenerate.HeaderQCData?.ProjectName).Underline();
        //                    });
        //                    table.Cell().Row(3).Column(3).ColumnSpan(2).AlignLeft().Text(text =>
        //                    {
        //                        text.Span("แปลงที่ ");
        //                        text.Span(dataQCGenerate.HeaderQCData?.UnitCode).Underline();
        //                    });
        //                    table.Cell().Row(3).Column(5).ColumnSpan(2).AlignLeft().Text(text =>
        //                    {
        //                        text.Span("บริษัทผู้รับเหมา ");
        //                        text.Span(dataQCGenerate.HeaderQCData?.CompanyVenderName).Underline();
        //                    });

        //                    table.Cell().Row(4).Column(1).ColumnSpan(2).AlignLeft().Text(text =>
        //                    {
        //                        text.Span("  วิศวกรผู้ควบคุมงาน ");
        //                        text.Span(dataQCGenerate.HeaderQCData?.PEInspectorName).Underline();
        //                    });
        //                    table.Cell().Row(4).Column(3).ColumnSpan(2).AlignLeft().Text(text =>
        //                    {
        //                        text.Span("QC ผู้ตรวจสอบ ");
        //                        text.Span(dataQCGenerate.HeaderQCData?.QCInspectorName).Underline();
        //                    });

        //                });
        //            });

        //            page.Content()
        //                //.PaddingVertical(4)
        //                .Column(col1 =>
        //            {
        //                IContainer DefaultCellStyle(IContainer container, string backgroundColor)
        //                {
        //                    return container
        //                        .Border(1)
        //                        .BorderColor(Colors.Black)
        //                        .Background(backgroundColor)
        //                        .PaddingVertical(1)
        //                        .PaddingHorizontal(3)
        //                        .AlignCenter()
        //                        .AlignTop();
        //                }

        //                col1.Item().Table(table2 =>
        //                {
        //                    table2.ColumnsDefinition(columns =>
        //                    {
        //                        columns.RelativeColumn(1); // Column for index
        //                        columns.RelativeColumn(3); // Column for "รายการ"
        //                        columns.RelativeColumn(3); // Column for "รูปรายการ"
        //                        columns.RelativeColumn(2); // Column for "ความเห็นเพิ่มเติม"
        //                        columns.RelativeColumn(1); // Column for "Major Defect"
        //                        columns.RelativeColumn(1); // Column for "ผ่าน"
        //                        columns.RelativeColumn(1); // Column for "ไม่ผ่าน"
        //                    });

        //                    // Header row
        //                    table2.Header(header =>
        //                    {
        //                        header.Cell().Element(CellStyle).Text("ลำดับ").FontSize(15).Bold();
        //                        header.Cell().Element(CellStyle).Text("รายการ").FontSize(15).Bold();
        //                        header.Cell().Element(CellStyle).Text("รูปรายการ").FontSize(15).Bold();
        //                        header.Cell().Element(CellStyle).Text("ความเห็นเพิ่มเติม").FontSize(15).Bold();
        //                        header.Cell().Element(CellStyle).Text("Major Defect").FontSize(15).Bold();
        //                        header.Cell().Element(CellStyle).Text("ผ่าน").FontSize(15).Bold();
        //                        header.Cell().Element(CellStyle).Text("ไม่ผ่าน").FontSize(15).Bold();
        //                    });

        //                    int index = 1;
        //                    foreach (var data in dataQCGenerate?.BodyListDefectQCData)
        //                    {
        //                        table2.Cell().Element(CellStyle).Text(index.ToString());  // Index column

        //                        // Multi-line "รายการ" column
        //                        table2.Cell().Element(CellStyle).Text(text =>
        //                        {
        //                            text.Line("ตำแหน่ง: " + data.RefSeqDefectText);
        //                            text.Line("หมวดงาน: " + data.DefectAreaText);
        //                            text.Line("รายการแก้ไข: " + data.DefectTypeText);
        //                        });

        //                        // Display image URLs in "รูปรายการ" column
        //                        table2.Cell().Element(CellStyle).Grid(grid =>
        //                        {
        //                            grid.AlignLeft();  // Align the grid content to the left
        //                            grid.Columns(6);    // Create a 6-column grid to display images

        //                            foreach (var image in data.ListImageDefact)
        //                            {
        //                                string pathImage = image.PathImageUrl;
        //                                var imgPath = _hosting.ContentRootPath + "/wwwroot/" + pathImage;

        //                                if (System.IO.File.Exists(imgPath))
        //                                {
        //                                    using var img = new FileStream(imgPath, FileMode.Open, FileAccess.Read);

        //                                    // Display each image and let QuestPDF handle the natural size
        //                                    grid.Item(6).AlignCenter().AlignMiddle()  // Center the image both horizontally and vertically
        //                                        .Border(0.5f)                        // Optional border for styling
        //                                        .Width(125)
        //                                        .Height(100)
        //                                        .Image(img);                         // Automatically adjust size based on image
        //                                }
        //                            }
        //                        });

        //                        table2.Cell().Element(CellStyle).Text(data.DefectRemark);      // ความเห็นเพิ่มเติม
        //                        table2.Cell().Element(CellStyle).Text(data.IsMajorDefectText); // Major Defect

        //                        // Handle "ผ่าน" and "ไม่ผ่าน" columns based on DefectStatus
        //                        if (data.DefectStatus == 27)
        //                        {
        //                            table2.Cell().Element(CellStyle).Text("✓"); // "ผ่าน" column (checked)
        //                            table2.Cell().Element(CellStyle).Text("");  // "ไม่ผ่าน" column (empty)
        //                        }
        //                        else
        //                        {
        //                            table2.Cell().Element(CellStyle).Text("");  // "ผ่าน" column (empty)
        //                            table2.Cell().Element(CellStyle).Text("✗"); // "ไม่ผ่าน" column (checked)
        //                        }

        //                        index++;
        //                    }


        //                    IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White);
        //                });

        //                col1.Item().Border(1).Table(table3 =>
        //                {
        //                    table3.ColumnsDefinition(columns =>
        //                    {
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                        columns.RelativeColumn(2);
        //                    });

        //                    table3.Cell().Row(1).Column(1).ColumnSpan(6).AlignLeft().Text(" สรุปจำนวน").Bold();
        //                    table3.Cell().Row(1).Column(3).ColumnSpan(3).AlignLeft().Text(text =>
        //                    {
        //                        text.Span("  รายการทั้งหมด : ").Bold();
        //                        text.Span(dataQCGenerate.SummaryQCData?.SumAllDefect.ToString());
        //                    });
        //                    table3.Cell().Row(2).Column(1).ColumnSpan(6).Text(text =>
        //                    {
        //                        foreach (var data in dataQCGenerate?.SummaryQCData.CalDefectBySeq)
        //                        {
        //                            // Instead of writing to a new row, combine the content into a single cell using `text.Line()`
        //                            text.Line(" จำนวนรายการครั้งที่ " + data.RefSeq + " : " + data.RefSeqCnt + " รายการ");
        //                        }
        //                    });
        //                    table3.Cell().Row(2).Column(3).ColumnSpan(3).Text(text =>
        //                    {
        //                        text.Line(" ผ่าน : " + dataQCGenerate.SummaryQCData?.SumPassDefect.ToString());
        //                        text.Line(" ไม่ผ่าน : " + dataQCGenerate.SummaryQCData?.SumNotPassDefect.ToString());
        //                    });

        //                });
        //            });

        //            page.Footer().Table(table2 =>
        //            {
        //                string pathQc = _hosting.ContentRootPath + "/" + dataQCGenerate.FooterQCData?.QCSignaturePathImageUrl;
        //                var signQc = new FileStream(pathQc, FileMode.Open);

        //                string pathPe = _hosting.ContentRootPath + "/wwwroot/" + dataQCGenerate.FooterQCData?.PESignaturePathImageUrl;
        //                var signPe = new FileStream(pathPe, FileMode.Open);

        //                table2.ColumnsDefinition(columns =>
        //                {
        //                    columns.RelativeColumn(6);
        //                    columns.RelativeColumn(6);
        //                });

        //                // Engineer signature
        //                table2.Cell().Row(1).Column(1).AlignCenter().Width(60).Image(signPe);
        //                table2.Cell().Row(2).Column(1).AlignCenter().Text("วิศวกรผู้ควบคุมงาน");
        //                table2.Cell().Row(3).Column(1).AlignCenter().Text("( " + dataQCGenerate.HeaderQCData?.PEInspectorName + " )");

        //                // QC signature
        //                table2.Cell().Row(1).Column(2).AlignCenter().Width(60).Image(signQc);
        //                table2.Cell().Row(2).Column(2).AlignCenter().Text("Quality Control (QC)");
        //                table2.Cell().Row(3).Column(2).AlignCenter().Text("( " + dataQCGenerate.HeaderQCData?.QCInspectorName + " )");

        //                // Page number in column 2
        //                table2.Cell().Row(4).Column(2).AlignRight().Text(text =>
        //                {
        //                    text.Span("Page ");
        //                    text.CurrentPageNumber();
        //                    text.Span(" of ");
        //                    text.TotalPages();
        //                });
        //            });
        //        });
        //    });

        //    string returnPath = "Upload/temp/" + "QCDocumentNo" + "-" + guid + ".pdf";
        //    document.GeneratePdf(returnPath);
        //    //document.ShowInPreviewer();

        //    return returnPath;
        //}

        public string GenerateQCPDF(Guid guid, DataGenerateQCPDFResp dataQCGenerate, DataDocumentModel genDocumentNo)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var fontPath = _hosting.ContentRootPath + "/wwwroot/lib/fonts/BrowalliaUPC.ttf";

            FontManager.RegisterFont(System.IO.File.OpenRead(fontPath));

            var imageHeader = Directory.GetCurrentDirectory() + "/wwwroot/img/img1.png";
            var imageBox = Directory.GetCurrentDirectory() + "/wwwroot/img/box.png";
            var imageCheckBox = Directory.GetCurrentDirectory() + "/wwwroot/img/checkbox.png";
            var imageCheck = Directory.GetCurrentDirectory() + "/wwwroot/img/check.png";
            var imageQCPass = Directory.GetCurrentDirectory() + "/wwwroot/img/QCpass.jpg";
            var imageQCPassWithCondition = Directory.GetCurrentDirectory() + "/wwwroot/img/QCPWC.jpg";

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
                        column.Item().Column(col1 =>
                        {
                            col1.Item().PaddingVertical(5).Width(150).Image(imageHeader);
                        });

                        column.Item().Border(1).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });


                            table.Cell().Row(1).Column(1).ColumnSpan(4).AlignLeft().Text(" ผลการตรวจ " + dataQCGenerate.HeaderQCData?.QCName).FontSize(20).Bold();
                            if(dataQCGenerate.HeaderQCData?.QCStatus == SystemConstant.UnitQCStatus.Pass)
                            {
                                table.Cell().Row(1).Column(4).RowSpan(4).AlignMiddle().Width(80).Image(imageQCPass);
                            }
                            else if(dataQCGenerate.HeaderQCData?.QCStatus == SystemConstant.UnitQCStatus.IsPassCondition)
                            {
                                table.Cell().Row(1).Column(4).RowSpan(4).AlignMiddle().Width(80).Image(imageQCPassWithCondition);
                            }
                            table.Cell().Row(1).Column(6).ColumnSpan(1).AlignLeft().Text("ครั้งที่ " + dataQCGenerate.HeaderQCData?.Seq).FontSize(20).Bold();

                            table.Cell().Row(2).Column(1).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("  ตรวจใน ");
                                text.Span(dataQCGenerate.HeaderQCData?.FormName).Underline();
                            });
                            table.Cell().Row(2).Column(3).ColumnSpan(1).AlignLeft().Text(text =>
                            {
                                string checkboxSymbol = dataQCGenerate.HeaderQCData?.QCStatus == SystemConstant.UnitQCStatus.IsNotReadyInspect ? "☑" : "☐";
                                text.Span(checkboxSymbol + " ").FontColor("#FF0000").FontSize(12);
                                text.Span("ไม่พร้อมให้ตรวจ").FontColor("#FF0000").FontSize(12);
                            });                          
                            table.Cell().Row(2).Column(5).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("วันที่ตรวจ ");
                                text.Span(dataQCGenerate.HeaderQCData?.UpdateDate).Underline();
                            });

                            table.Cell().Row(3).Column(1).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("  โครงการ ");
                                text.Span(dataQCGenerate.HeaderQCData?.ProjectName).Underline();
                            });
                            table.Cell().Row(3).Column(3).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("แปลงที่ ");
                                text.Span(dataQCGenerate.HeaderQCData?.UnitCode).Underline();
                            });
                            table.Cell().Row(3).Column(5).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("บริษัทผู้รับเหมา ");
                                text.Span(dataQCGenerate.HeaderQCData?.CompanyVenderName).Underline();
                            });

                            table.Cell().Row(4).Column(1).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("  วิศวกรผู้ควบคุมงาน ");
                                text.Span(dataQCGenerate.HeaderQCData?.PEInspectorName).Underline();
                            });
                            table.Cell().Row(4).Column(3).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("QC ผู้ตรวจสอบ ");
                                text.Span(dataQCGenerate.HeaderQCData?.QCInspectorName).Underline();
                            });
                            table.Cell().Row(4).Column(5).ColumnSpan(2).AlignLeft().Text("เลขที่ใบตรวจ: " + genDocumentNo.documentNo);
                        });
                    });

                    page.Content().Column(col1 =>
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
                                .AlignTop();
                        }

                        col1.Item().Table(table2 =>
                        {
                            table2.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // Column for index
                                columns.RelativeColumn(3); // Column for "รายการ"
                                columns.RelativeColumn(3); // Column for "รูปรายการ"
                                columns.RelativeColumn(2); // Column for "ความเห็นเพิ่มเติม"
                                columns.RelativeColumn(1); // Column for "Major Defect"
                                columns.RelativeColumn(1); // Column for "ผ่าน"
                                columns.RelativeColumn(1); // Column for "ไม่ผ่าน"
                            });

                            // Header row
                            table2.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ลำดับ").FontSize(15).Bold();
                                header.Cell().Element(CellStyle).Text("รายการ").FontSize(15).Bold();
                                header.Cell().Element(CellStyle).Text("รูปรายการ").FontSize(15).Bold();
                                header.Cell().Element(CellStyle).Text("ความเห็นเพิ่มเติม").FontSize(15).Bold();
                                header.Cell().Element(CellStyle).Text("Major Defect").FontSize(15).Bold();
                                header.Cell().Element(CellStyle).Text("ผ่าน").FontSize(15).Bold();
                                header.Cell().Element(CellStyle).Text("ไม่ผ่าน").FontSize(15).Bold();
                            });

                            int index = 1;
                            foreach (var data in dataQCGenerate?.BodyListDefectQCData)
                            {
                                table2.Cell().Element(CellStyle).Text(index.ToString());  // Index column

                                // Multi-line "รายการ" column
                                table2.Cell().Element(CellStyle).AlignLeft().Text(text =>
                                {
                                    text.Line("ตำแหน่ง: " + data.RefSeqDefectText);
                                    text.Line("หมวดงาน: " + data.DefectAreaText);
                                    text.Line("รายการแก้ไข: " + data.DefectTypeText);
                                });

                                // Display image URLs in "รูปรายการ" column
                                table2.Cell().Element(CellStyle).Grid(grid =>
                                {
                                    grid.AlignLeft();  // Align the grid content to the left
                                    grid.Columns(6);    // Create a 6-column grid to display images

                                    foreach (var image in data.ListImageDefact)
                                    {
                                        string pathImage = image.PathImageUrl;
                                        var imgPath = _hosting.ContentRootPath + "/wwwroot/" + pathImage;

                                        if (System.IO.File.Exists(imgPath))
                                        {
                                            using var img = new FileStream(imgPath, FileMode.Open, FileAccess.Read);

                                            // Display each image and let QuestPDF handle the natural size
                                            grid.Item(6).AlignCenter().AlignMiddle()  // Center the image both horizontally and vertically
                                                .Border(0.5f)                        // Optional border for styling
                                                .Width(125)
                                                .Height(100)
                                                .Image(img);                         // Automatically adjust size based on image
                                        }
                                    }
                                });

                                table2.Cell().Element(CellStyle).AlignLeft().Text(data.DefectRemark);      // ความเห็นเพิ่มเติม
                                table2.Cell().Element(CellStyle).Text(data.IsMajorDefectText); // Major Defect

                                // Handle "ผ่าน" and "ไม่ผ่าน" columns based on DefectStatus
                                if (data.DefectStatus == 27)
                                {
                                    table2.Cell().Element(CellStyle).Text("✓"); // "ผ่าน" column (checked)
                                    table2.Cell().Element(CellStyle).Text("");  // "ไม่ผ่าน" column (empty)
                                }
                                else
                                {
                                    table2.Cell().Element(CellStyle).Text("");  // "ผ่าน" column (empty)
                                    table2.Cell().Element(CellStyle).Text("✗"); // "ไม่ผ่าน" column (checked)
                                }

                                index++;
                            }


                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White);
                        });

                        col1.Item().Border(1).Table(table3 =>
                        {
                            table3.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table3.Cell().Row(1).Column(1).ColumnSpan(6).AlignLeft().Text(" สรุปจำนวน").Bold();
                            table3.Cell().Row(1).Column(3).ColumnSpan(3).AlignLeft().PaddingTop(2).Text(text =>
                            {
                                text.Span("  รายการทั้งหมด : ").Bold();
                                text.Span(dataQCGenerate.SummaryQCData?.SumAllDefect.ToString());
                            });
                            table3.Cell().Row(2).Column(1).ColumnSpan(6).Text(text =>
                            {
                                foreach (var data in dataQCGenerate?.SummaryQCData.CalDefectBySeq)
                                {
                                    // Instead of writing to a new row, combine the content into a single cell using `text.Line()`
                                    text.Line(" จำนวนรายการครั้งที่ " + data.RefSeq + " : " + data.RefSeqCnt + " รายการ");
                                }
                            });
                            table3.Cell().Row(2).Column(3).ColumnSpan(3).Text(text =>
                            {
                                text.Line("  ผ่าน : " + dataQCGenerate.SummaryQCData?.SumPassDefect.ToString());
                                text.Line("  ไม่ผ่าน : " + dataQCGenerate.SummaryQCData?.SumNotPassDefect.ToString());
                            });

                        });
                    });

                    // Footer Setup
                    page.Footer().Table(table =>
                    {
                        // Paths to the signature images
                        string pathQc = _hosting.ContentRootPath + "/" + dataQCGenerate.FooterQCData?.QCSignaturePathImageUrl;
                        string pathPe = _hosting.ContentRootPath + "/wwwroot/" + dataQCGenerate.FooterQCData?.PESignaturePathImageUrl;

                        // Load images using FileStream
                        FileStream? signQcStream = null;
                        FileStream? signPeStream = null;

                        if (System.IO.File.Exists(pathQc))
                        {
                            signQcStream = new FileStream(pathQc, FileMode.Open, FileAccess.Read);
                        }

                        if (System.IO.File.Exists(pathPe))
                        {
                            signPeStream = new FileStream(pathPe, FileMode.Open, FileAccess.Read);
                        }

                        // Define columns for signatures
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(6);
                            columns.RelativeColumn(6);
                        });

                        // Engineer Signature
                        if (signPeStream != null)
                        {
                            table.Cell().Row(1).Column(1).AlignCenter().Width(60).Image(signPeStream); // PE signature
                        }
                        else
                        {
                            table.Cell().Row(1).Column(1).AlignCenter().Text("PE Signature Missing");
                        }
                        table.Cell().Row(2).Column(1).AlignCenter().Text("วิศวกรผู้ควบคุมงาน");
                        table.Cell().Row(3).Column(1).AlignCenter().Text("( " + dataQCGenerate.HeaderQCData?.PEInspectorName + " )");

                        // QC Signature
                        if (signQcStream != null)
                        {
                            table.Cell().Row(1).Column(2).AlignCenter().Width(60).Image(signQcStream); // QC signature
                        }
                        else
                        {
                            table.Cell().Row(1).Column(2).AlignCenter().Text("QC Signature Missing");
                        }
                        table.Cell().Row(2).Column(2).AlignCenter().Text("Quality Control (QC)");
                        table.Cell().Row(3).Column(2).AlignCenter().Text("( " + dataQCGenerate.HeaderQCData?.QCInspectorName + " )");

                        // Page number in column 2
                        table.Cell().Row(4).Column(2).AlignRight().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });

                        // Ensure to close the file streams after usage
                        signQcStream?.Dispose();
                        signPeStream?.Dispose();
                    });
                });
            });

            string returnPath = "Upload/temp/" + "QCDocumentNo" + "-" + guid + ".pdf";
            document.GeneratePdf(returnPath);

            return returnPath;
        }

        public DataGenerateQCPDFResp GetDataQC1To4ForGeneratePDF(DataToGenerateModel model)
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
                                     join truserresourc in _context.tr_UserResource.Where(ur => ur.FlagActive == true) on trQC.UpdateBy equals truserresourc.UserID into truserresourcJoin
                                     from truserresourc in truserresourcJoin.DefaultIfEmpty()
                                     join pesignpath in _context.tm_Resource on trQC.PESignResourceID equals pesignpath.ID
                                     join qcsignpath in _context.tm_Resource on truserresourc.ResourceID equals qcsignpath.ID
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
                                         Seq = trQC.Seq,
                                         UpdateDate = trQC.UpdateDate,
                                         QCSignPath = qcsignpath.FilePath,
                                         PESignPath = pesignpath.FilePath,
                                         Info = (from trqcla in _context.tr_QC_UnitCheckList_Action
                                                 where trqcla.QCUnitCheckListID == model.QCUnitCheckListID
                                                 select new
                                                 {
                                                     Remark = trqcla.Remark,
                                                     MainImage = (from trqclr in _context.tr_QC_UnitCheckList_Resource 
                                                                  join tmr in _context.tm_Resource on trqclr.ResourceID equals tmr.ID
                                                                  where trqclr.QCUnitCheckListID == trqcla.QCUnitCheckListID
                                                                  && trqclr.QCUnitCheckListDetailID == null && trqclr.FlagActive == true
                                                                  select new
                                                                  {
                                                                      FilePath = tmr.FilePath
                                                                  }).ToList()
                                                 }).FirstOrDefault()
                                     }).FirstOrDefault();

            List<MainImage> mainImages = new List<MainImage>();
            MainInfo info = new MainInfo();

            if (queryHeaderFooter.Info != null)
            {
                if (queryHeaderFooter.Info.MainImage != null && queryHeaderFooter.Info.MainImage.Count > 0)
                {
                    foreach (var i in queryHeaderFooter.Info.MainImage)
                    {
                        MainImage image = new MainImage()
                        {
                            FilePath = i.FilePath
                        };

                        mainImages.Add(image);
                    };
                }

                info = new MainInfo()
                {
                    MainRemark = queryHeaderFooter.Info.Remark,
                    MainImages = mainImages
                };
            }

            DataGenerateQCPDFResp resp = new DataGenerateQCPDFResp()
            {
                HeaderQCData = new HeaderQCPdfData()
                {
                    QCName = queryHeaderFooter.QCName,
                    ProjectName = queryHeaderFooter.ProjectName,
                    UnitCode = queryHeaderFooter.UnitCode,
                    CompanyVenderName = queryHeaderFooter.CompanyVenderName,
                    FormName = queryHeaderFooter.FormName,
                    QCInspectorName = queryHeaderFooter.QCInspectorName,
                    PEInspectorName = queryHeaderFooter.PEInspectorName,
                    Seq = FormatExtension.NullToString(queryHeaderFooter.Seq),
                    UpdateDate = FormatExtension.FormatDateToDayMonthNameYearTime(queryHeaderFooter.UpdateDate),
                    QCStatus = queryHeaderFooter.QCStatus,
                    QCStatusText = queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.Pass ? SystemConstant.UnitQCStatusText.Pass
                                 : queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.NotPass ? SystemConstant.UnitQCStatusText.NotPass
                                 : queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.IsNotReadyInspect ? SystemConstant.UnitQCStatusText.IsNotReadyInspect
                                 : queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.IsPassCondition ? SystemConstant.UnitQCStatusText.IsPassCondition
                                 : queryHeaderFooter.QCStatus == SystemConstant.UnitQCStatus.InProgress ? SystemConstant.UnitQCStatusText.InProgress
                                 : "ไม่พบสถานะ",
                    Info = info
                },
                BodyQCPdf = new BodyQCPdfData(),
                FooterQCData = new FooterQCPdfData()
                {
                    QCSignaturePathImageUrl = queryHeaderFooter.QCSignPath,
                    PESignaturePathImageUrl = queryHeaderFooter.PESignPath
                },
            };
            tm_Project? project = _context.tm_Project
                .Where(o => o.ProjectID == model.ProjectID && o.FlagActive == true)
                .FirstOrDefault();

            // get data master
            var qcCheckList = (from trqcl in _context.tr_QC_UnitCheckList
                               join trqcla in _context.tr_QC_UnitCheckList_Action on trqcl.ID equals trqcla.QCUnitCheckListID
                               join tmqcl in _context.tm_QC_CheckList on trqcl.QCTypeID equals tmqcl.QCTypeID
                               where tmqcl.ProjectTypeID == project.ProjectTypeID
                               && trqcl.QCTypeID == model.QCTypeID && trqcl.FlagActive == true
                               && trqcla.ActionType == SystemConstant.ActionType.SUBMIT
                               && trqcl.ID == model.QCUnitCheckListID
                               select new
                               {
                                   QCCheckListID = trqcl.ID,
                                   CheckListID = trqcl.CheckListID,
                                   QCAction = trqcla.ActionType,
                                   QCActionDate = trqcla.ActionDate
                               }).FirstOrDefault();

            var getDetailCheckList = (from tmqcld in _context.tm_QC_CheckListDetail
                                      join trqcld in _context.tr_QC_UnitCheckList_Detail
                                     .Where(o => o.QCUnitCheckListID == qcCheckList.QCCheckListID)
                                          on tmqcld.ID equals trqcld.CheckListDetailID into transDetail
                                      from trqcld in transDetail.DefaultIfEmpty() // Left join
                                      where tmqcld.QCCheckListID == qcCheckList.CheckListID
                                            && tmqcld.FlagActive == true
                                            && tmqcld.ParentID == null
                                      select new
                                      {
                                          TmCheckListDetail = tmqcld,
                                          TrCheckListDetail = trqcld,
                                      }).ToList();

            List<QcCheckListDetailData> listDetail = new List<QcCheckListDetailData>();
            foreach (var detail in getDetailCheckList)
            {
                var getParentDetail = (from tmcld in _context.tm_QC_CheckListDetail
                                       join trqcld in _context.tr_QC_UnitCheckList_Detail
                                       .Where(o => o.QCUnitCheckListID == qcCheckList.QCCheckListID)
                                           on tmcld.ID equals trqcld.CheckListDetailID into transDetail
                                       from trqcld in transDetail.DefaultIfEmpty() // Left Join 
                                       where tmcld.ParentID == detail.TmCheckListDetail.ID
                                       select new
                                       {
                                           TmCheckListParentDetail = tmcld,
                                           TrCheckListParentDetail = trqcld
                                       }).ToList();

                List<DetailImage> listDetailImage = new List<DetailImage>();
                if (detail.TrCheckListDetail != null)
                {
                    var query = from trqclr in _context.tr_QC_UnitCheckList_Resource
                                join tmr in _context.tm_Resource on trqclr.ResourceID equals tmr.ID into deTm
                                from tmr in deTm.DefaultIfEmpty()  // Left join
                                where trqclr.QCUnitCheckListDetailID == detail.TrCheckListDetail.ID
                                && trqclr.QCUnitCheckListID == qcCheckList.QCCheckListID
                                select new { tmr.FilePath };

                    var getListImageDetail = query.AsEnumerable().Select(e => e.FilePath).ToList();  // Convert the result to a list
  
                    if (getListImageDetail != null && getListImageDetail.Count > 0)
                    {
                        foreach (var path in getListImageDetail)
                        {
                            DetailImage img = new DetailImage()
                            {
                                FilePath = path
                            };

                            listDetailImage.Add(img);
                        }
                    }
                }

                QcCheckListDetailData detailData = new QcCheckListDetailData()
                {
                    DetailID = detail.TrCheckListDetail != null ? detail.TrCheckListDetail.ID : null,
                    DetailName = detail.TmCheckListDetail.Name,
                    StatusID = detail.TrCheckListDetail != null ? detail.TrCheckListDetail.StatusID : null ,
                    DetailRemark = detail.TrCheckListDetail != null ? detail.TrCheckListDetail.Remark : null,
                    PassBySeq = detail.TrCheckListDetail != null ? detail.TrCheckListDetail.PassBySeq : null,
                    DetailImages = listDetailImage,
                    ParentDetailDatas = new List<ParentDetailData>()
                };

                if(getParentDetail != null)
                {
                    foreach (var parent in getParentDetail)
                    {
                        List<ParentImage> listParentImage = new List<ParentImage>();
                        if (parent.TrCheckListParentDetail != null)
                        {
                            var getListImageParent = (from trqclr in _context.tr_QC_UnitCheckList_Resource
                                                      join tmr in _context.tm_Resource on trqclr.ResourceID equals tmr.ID
                                                      where trqclr.QCUnitCheckListDetailID == parent.TrCheckListParentDetail.ID
                                                      && trqclr.QCUnitCheckListID == qcCheckList.QCCheckListID
                                                      select tmr.FilePath);

                            if (getListImageParent != null && getListImageParent.Count() > 0)
                                foreach (var image in getListImageParent)
                                {
                                    ParentImage img = new ParentImage()
                                    {
                                        FilePath = image
                                    };

                                    listParentImage.Add(img);
                                }
                        }
                        ParentDetailData parentData = new ParentDetailData()
                        {
                            ParentDetailID = parent.TrCheckListParentDetail != null ? parent.TrCheckListParentDetail.ID : null,
                            ParentDetailName = parent.TmCheckListParentDetail.Name,
                            ParentStatusID = parent.TrCheckListParentDetail != null ? parent.TrCheckListParentDetail.StatusID : null,
                            ParentDetailRemark = parent.TrCheckListParentDetail != null ? parent.TrCheckListParentDetail.Remark : null,
                            ParentPassBySeq = parent.TrCheckListParentDetail != null ? parent.TrCheckListParentDetail.PassBySeq : null,
                            ParentImages = listParentImage,
                        };

                        detailData.ParentDetailDatas.Add(parentData);
                    }
                }

                listDetail.Add(detailData);
            }

            resp.BodyQCPdf.QcCheckListDetailDatas = listDetail;

            return resp;
        }

        public string GenerateQCPDF2(Guid guid, DataGenerateQCPDFResp dataQCGenerate, DataDocumentModel genDocumentNo)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var fontPath = _hosting.ContentRootPath + "/wwwroot/lib/fonts/BrowalliaUPC.ttf";

            FontManager.RegisterFont(System.IO.File.OpenRead(fontPath));

            var imageHeader = Directory.GetCurrentDirectory() + "/wwwroot/img/img1.png";
            var imageBox = Directory.GetCurrentDirectory() + "/wwwroot/img/box.png";
            var imageCheckBox = Directory.GetCurrentDirectory() + "/wwwroot/img/checkbox.png";
            var imageCheck = Directory.GetCurrentDirectory() + "/wwwroot/img/check.png";
            var imageQCPass = Directory.GetCurrentDirectory() + "/wwwroot/img/QCpass.jpg";
            var imageQCPassWithCondition = Directory.GetCurrentDirectory() + "/wwwroot/img/QCPWC.jpg";

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
                        column.Item().Column(col1 =>
                        {
                            col1.Item().PaddingVertical(5).Width(150).Image(imageHeader);
                        });

                        column.Item().Border(1).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });


                            table.Cell().Row(1).Column(1).ColumnSpan(4).AlignLeft().Text(" ผลการตรวจ " + dataQCGenerate.HeaderQCData?.QCName).FontSize(20).Bold();
                            if (dataQCGenerate.HeaderQCData?.QCStatus == SystemConstant.UnitQCStatus.Pass)
                            {
                                table.Cell().Row(1).Column(4).RowSpan(4).AlignMiddle().Width(80).Image(imageQCPass);
                            }

                            table.Cell().Row(1).Column(6).ColumnSpan(1).AlignLeft().Text("ครั้งที่ " + dataQCGenerate.HeaderQCData?.Seq).FontSize(20).Bold();

                            table.Cell().Row(2).Column(1).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("  ตรวจใน ");
                                text.Span(dataQCGenerate.HeaderQCData?.FormName).Underline();
                            });
                            table.Cell().Row(2).Column(3).ColumnSpan(1).AlignLeft().Text(text =>
                            {
                                string checkboxSymbol = dataQCGenerate.HeaderQCData?.QCStatus == SystemConstant.UnitQCStatus.IsNotReadyInspect ? "☑" : "☐";
                                text.Span(checkboxSymbol + " ").FontColor("#FF0000").FontSize(12);
                                text.Span("ไม่พร้อมให้ตรวจ").FontColor("#FF0000").FontSize(12);
                            });
                            table.Cell().Row(2).Column(5).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("วันที่ตรวจ ");
                                text.Span(dataQCGenerate.HeaderQCData?.UpdateDate).Underline();
                            });

                            table.Cell().Row(3).Column(1).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("  โครงการ ");
                                text.Span(dataQCGenerate.HeaderQCData?.ProjectName).Underline();
                            });
                            table.Cell().Row(3).Column(3).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("แปลงที่ ");
                                text.Span(dataQCGenerate.HeaderQCData?.UnitCode).Underline();
                            });
                            table.Cell().Row(3).Column(5).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("บริษัทผู้รับเหมา ");
                                text.Span(dataQCGenerate.HeaderQCData?.CompanyVenderName).Underline();
                            });

                            table.Cell().Row(4).Column(1).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("  วิศวกรผู้ควบคุมงาน ");
                                text.Span(dataQCGenerate.HeaderQCData?.PEInspectorName).Underline();
                            });
                            table.Cell().Row(4).Column(3).ColumnSpan(2).AlignLeft().Text(text =>
                            {
                                text.Span("QC ผู้ตรวจสอบ ");
                                text.Span(dataQCGenerate.HeaderQCData?.QCInspectorName).Underline();
                            });
                            table.Cell().Row(4).Column(5).ColumnSpan(2).AlignLeft().Text("เลขที่ใบตรวจ: " + genDocumentNo.documentNo);
                        });
                    });

                    page.Content().Column(col1 =>
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
                                .AlignTop();
                        }

                        col1.Item().Table(table2 =>
                        {
                            table2.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); 
                                columns.RelativeColumn(3); 
                                columns.RelativeColumn(1); 
                                columns.RelativeColumn(1); 
                                columns.RelativeColumn(2); 
                                columns.RelativeColumn(4); 
                            });

                            // Header row
                            table2.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("").FontSize(12).Bold();
                                header.Cell().Element(CellStyle).Text("รายการตรวจ").FontSize(12).Bold();
                                header.Cell().Element(CellStyle).Text("ผ่าน").FontSize(12).Bold();
                                header.Cell().Element(CellStyle).Text("ไม่ผ่าน").FontSize(12).Bold();
                                header.Cell().Element(CellStyle).Text("จำนวนครั้งที่ตรวจผ่าน").FontSize(12).Bold();
                                header.Cell().Element(CellStyle).Text("ความเห็นเพิ่มเติม").FontSize(12).Bold();
                            });

                            int index = 1;
                            int index2 = 1;
                            foreach (var data in dataQCGenerate?.BodyQCPdf.QcCheckListDetailDatas)
                            {
                                table2.Cell().Row((uint)index2).Column(1).Element(CellStyle).Text(index.ToString());  // Index column
                                table2.Cell().Row((uint)index2).Column(2).Element(CellStyle).AlignLeft().Text(data.DetailName).WrapAnywhere();
                                if (dataQCGenerate.HeaderQCData?.QCStatus != SystemConstant.UnitQCStatus.IsNotReadyInspect)
                                {
                                    
                                    if (data.ParentDetailDatas != null && data.ParentDetailDatas.Count > 0)
                                    {
                                        table2.Cell().Row((uint)index2).Column(3).Element(CellStyle).Text(""); // "ผ่าน" column (checked)
                                        table2.Cell().Row((uint)index2).Column(4).Element(CellStyle).Text("");  // "ไม่ผ่าน" column (empty)
                                        table2.Cell().Row((uint)index2).Column(5).Element(CellStyle).Text("");
                                        table2.Cell().Row((uint)index2).Column(6).Element(CellStyle).Text("");

                                        int indexParent = index2 + 1;
                                        foreach(var data2 in data.ParentDetailDatas)
                                        {
                                            table2.Cell().Row((uint)indexParent).Column(1).Element(CellStyle).Text("");  // Index column
                                            table2.Cell().Row((uint)indexParent).Column(2).Element(CellStyle).AlignLeft().Text(data2.ParentDetailName).WrapAnywhere();
                                            if (data2.ParentStatusID == SystemConstant.Qc_CheckList_Status.PASS)
                                            {
                                                table2.Cell().Row((uint)indexParent).Column(3).Element(CellStyle).Text("✓"); // "ผ่าน" column (checked)
                                                table2.Cell().Row((uint)indexParent).Column(4).Element(CellStyle).Text("");  // "ไม่ผ่าน" column (empty)
                                                table2.Cell().Row((uint)indexParent).Column(5).Element(CellStyle).Text(data2.ParentPassBySeq.ToString());
                                            }
                                            else
                                            {
                                                table2.Cell().Row((uint)indexParent).Column(3).Element(CellStyle).Text("");  // "ผ่าน" column (empty)
                                                table2.Cell().Row((uint)indexParent).Column(4).Element(CellStyle).Text("✓"); // "ไม่ผ่าน" column (checked)
                                                if (data2.ParentPassBySeq == 0)
                                                {
                                                    table2.Cell().Row((uint)indexParent).Column(5).Element(CellStyle).Text("N/A");
                                                }
                                                else
                                                {
                                                    table2.Cell().Row((uint)indexParent).Column(5).Element(CellStyle).Text("");
                                                }
                                            }
                                            table2.Cell().Row((uint)indexParent).Column(6).Element(CellStyle).AlignLeft().Text(data2.ParentDetailRemark);// ความเห็นเพิ่มเติม
                                            indexParent++;
                                        }
                                        index2 = indexParent;
                                    }
                                    else
                                    {
                                        if (data.StatusID == SystemConstant.Qc_CheckList_Status.PASS)
                                        {
                                            table2.Cell().Row((uint)index2).Column(3).Element(CellStyle).Text("✓"); // "ผ่าน" column (checked)
                                            table2.Cell().Row((uint)index2).Column(4).Element(CellStyle).Text("");  // "ไม่ผ่าน" column (empty)
                                            table2.Cell().Row((uint)index2).Column(5).Element(CellStyle).Text(data.PassBySeq.ToString());
                                        }
                                        else
                                        {
                                            table2.Cell().Row((uint)index2).Column(3).Element(CellStyle).Text("");  // "ผ่าน" column (empty)
                                            table2.Cell().Row((uint)index2).Column(4).Element(CellStyle).Text("✓"); // "ไม่ผ่าน" column (checked)
                                            if (data.PassBySeq == 0)
                                            {
                                                table2.Cell().Row((uint)index2).Column(5).Element(CellStyle).Text("N/A");
                                            }
                                            else
                                            {
                                                table2.Cell().Row((uint)index2).Column(5).Element(CellStyle).Text("");
                                            }
                                        }
                                        table2.Cell().Row((uint)index2).Column(6).Element(CellStyle).AlignLeft().Text(data.DetailRemark);      // ความเห็นเพิ่มเติม
                                        index2++;
                                    }

                                }
                                else
                                {
                                    table2.Cell().Row((uint)index2).Column(3).Element(CellStyle).Text(""); // "ผ่าน" column (checked)
                                    table2.Cell().Row((uint)index2).Column(4).Element(CellStyle).Text("");  // "ไม่ผ่าน" column (empty)
                                    table2.Cell().Row((uint)index2).Column(5).Element(CellStyle).Text("");
                                    table2.Cell().Row((uint)index2).Column(6).Element(CellStyle).Text("");
                                    index2++;
                                }
                                index++;
                            }

                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White);
                        });

                        col1.Item().Column(col1 =>
                        {
                            col1.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        });

                        col1.Item().Border(1).Table(table3 =>
                        {
                            table3.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(8);
                                columns.RelativeColumn(4);
                            });

                            table3.Cell().Row(1).Column(1).ColumnSpan(2).AlignLeft().Text(" แสดงรูปรายการที่ไม่ผ่าน").Bold();
                           
                            if (dataQCGenerate.HeaderQCData?.QCStatus == SystemConstant.UnitQCStatus.IsNotReadyInspect)
                            {
                                table3.Cell().Row(2).Column(1).AlignLeft().Text("สภาพไม่พร้อมให้ตรวจ").Bold();
                                table3.Cell().Row(2).Column(2).Text("ความเห็นเพิ่มเติม");
                                if (dataQCGenerate.HeaderQCData.Info != null)
                                {
                                    // add image 
                                    table3.Cell().Row(3).Column(1).Grid(grid =>
                                    {
                                        grid.VerticalSpacing(15);
                                        grid.HorizontalSpacing(15);
                                        grid.AlignLeft();
                                        grid.Columns(8);

                                        if (dataQCGenerate.HeaderQCData.Info.MainImages != null)
                                        {
                                            foreach (var image in dataQCGenerate.HeaderQCData.Info.MainImages)
                                            {
                                                string pathImage = image.FilePath;
                                                var imgPath = _hosting.ContentRootPath + "/" + pathImage;

                                                if (System.IO.File.Exists(imgPath))
                                                {
                                                    using var img = new FileStream(imgPath, FileMode.Open);

                                                    // Display each image and let QuestPDF handle the natural size
                                                    grid.Item(4).AlignCenter().AlignMiddle()  // Center the image both horizontally and vertically
                                                        .Border(0.5f)                        // Optional border for styling
                                                        .Width(100)
                                                        .Height(100)
                                                        .Image(img);                         // Automatically adjust size based on image
                                                }
                                            }
                                        }

                                    });

                                    table3.Cell().Row(3).Column(2).Text(dataQCGenerate.HeaderQCData.Info.MainRemark);
                                }

                            }
                            else
                            {
                                int index2 = 1;
                                int i = 2;
                                foreach (var data in dataQCGenerate?.BodyQCPdf.QcCheckListDetailDatas)
                                {
                                    if (data.ParentDetailDatas != null && data.ParentDetailDatas.Count > 0)
                                    {
                                        if(data.ParentDetailDatas.Any(o => o.ParentStatusID == SystemConstant.Qc_CheckList_Status.NOTPASS))
                                        {
                                            table3.Cell().Row((uint)i).Column(1).AlignLeft().Text(index2 + ". " + data.DetailName).Bold();
                                            table3.Cell().Row((uint)i).Column(2).Text("ความเห็นเพิ่มเติม");
                                        }
                                        int mainRow = i + 1;
                                        int parentRow = mainRow + 1;
                                        foreach (var data2 in data.ParentDetailDatas)
                                        {
                                            if (data2.ParentStatusID == SystemConstant.Qc_CheckList_Status.NOTPASS)
                                            {
                                                table3.Cell().Row((uint)mainRow).Column(1).AlignLeft().Text(data2.ParentDetailName);
                                                table3.Cell().Row((uint)parentRow).Column(1).Grid(grid =>
                                                {
                                                    grid.VerticalSpacing(15);
                                                    grid.HorizontalSpacing(15);
                                                    grid.AlignLeft();
                                                    grid.Columns(8);

                                                    foreach (var image in data2.ParentImages)
                                                    {
                                                        string pathImage = image.FilePath;
                                                        var imgPath = _hosting.ContentRootPath + "/" + pathImage;

                                                        if (System.IO.File.Exists(imgPath))
                                                        {
                                                            using var img = new FileStream(imgPath, FileMode.Open);

                                                            // Display each image and let QuestPDF handle the natural size
                                                            grid.Item(4).AlignCenter().AlignMiddle()  // Center the image both horizontally and vertically
                                                                .Border(0.5f)                        // Optional border for styling
                                                                .Width(100)
                                                                .Height(100)
                                                                .Image(img);                         // Automatically adjust size based on image
                                                        }
                                                    }
                                                });
                                                table3.Cell().Row((uint)parentRow).Column(2).AlignLeft().Text(data2.ParentDetailRemark);
                                                mainRow = parentRow + 1;
                                                parentRow = mainRow + 1;
                                            }
                                        }

                                        i = parentRow;
                                    }
                                    else
                                    {
                                        int rowNum = i + 1;
                                        if(data.StatusID == SystemConstant.Qc_CheckList_Status.NOTPASS)
                                        {
                                            table3.Cell().Row((uint)i).Column(1).AlignLeft().Text(index2 + ". " + data.DetailName).Bold();
                                            table3.Cell().Row((uint)i).Column(2).Text("ความเห็นเพิ่มเติม");

                                            table3.Cell().Row((uint)rowNum).Column(1).Grid(grid =>
                                            {
                                                grid.VerticalSpacing(15);
                                                grid.HorizontalSpacing(15);
                                                grid.AlignLeft();
                                                grid.Columns(8);

                                                foreach (var image2 in data.DetailImages)
                                                {
                                                    string pathImage = image2.FilePath;
                                                    var imgPath = _hosting.ContentRootPath + "/" + pathImage;

                                                    if (System.IO.File.Exists(imgPath))
                                                    {
                                                        using var img = new FileStream(imgPath, FileMode.Open);

                                                        // Display each image and let QuestPDF handle the natural size
                                                        grid.Item(4).AlignCenter().AlignMiddle()  // Center the image both horizontally and vertically
                                                            .Border(0.5f)                        // Optional border for styling
                                                            .Width(100)
                                                            .Height(100)
                                                            .Image(img);                         // Automatically adjust size based on image
                                                    }
                                                }
                                            });
                                            table3.Cell().Row((uint)rowNum).Column(2).AlignLeft().Text(data.DetailRemark);
                                            i = rowNum + 1;
                                        }
                                    }
                                    index2++;
                                }
                            }
                        });
                    });

                    // Footer Setup
                    page.Footer().Table(table =>
                    {
                        // Paths to the signature images
                        string pathQc = _hosting.ContentRootPath + "/" + dataQCGenerate.FooterQCData?.QCSignaturePathImageUrl;
                        string pathPe = _hosting.ContentRootPath + "/" +  dataQCGenerate.FooterQCData?.PESignaturePathImageUrl;

                        // Load images using FileStream
                        FileStream? signQcStream = null;
                        FileStream? signPeStream = null;

                        if (System.IO.File.Exists(pathQc))
                        {
                            signQcStream = new FileStream(pathQc, FileMode.Open);
                        }

                        if (System.IO.File.Exists(pathPe))
                        {
                            signPeStream = new FileStream(pathPe, FileMode.Open);
                        }

                        // Define columns for signatures
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(6);
                            columns.RelativeColumn(6);
                        });

                        // Engineer Signature
                        if (signPeStream != null)
                        {
                            table.Cell().Row(1).Column(1).AlignCenter().Width(60).Image(signPeStream); // PE signature
                        }
                        else
                        {
                            table.Cell().Row(1).Column(1).AlignCenter().Text("PE Signature Missing");
                        }
                        table.Cell().Row(2).Column(1).AlignCenter().Text("วิศวกรผู้ควบคุมงาน");
                        table.Cell().Row(3).Column(1).AlignCenter().Text("( " + dataQCGenerate.HeaderQCData?.PEInspectorName + " )");

                        // QC Signature
                        if (signQcStream != null)
                        {
                            table.Cell().Row(1).Column(2).AlignCenter().Width(60).Image(signQcStream); // QC signature
                        }
                        else
                        {
                            table.Cell().Row(1).Column(2).AlignCenter().Text("QC Signature Missing");
                        }
                        table.Cell().Row(2).Column(2).AlignCenter().Text("Quality Control (QC)");
                        table.Cell().Row(3).Column(2).AlignCenter().Text("( " + dataQCGenerate.HeaderQCData?.QCInspectorName + " )");

                        // Page number in column 2
                        table.Cell().Row(4).Column(2).AlignRight().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });

                        // Ensure to close the file streams after usage
                        signQcStream?.Dispose();
                        signPeStream?.Dispose();
                    });
                });
            });

            string returnPath = "Upload/temp/" + "QCDocumentNo" + "-" + guid + ".pdf";
            document.GeneratePdf(returnPath);

            return returnPath;
        }
    }
}

