using System;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Services;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.ConstructionTracking.Web.Controllers
{
    public class GeneratePDFController  : BaseController
    {
        private readonly IGeneratePDFService _generatePDFService;
        private readonly IHostEnvironment _hosting;

        public GeneratePDFController(IGeneratePDFService generatePDFService,
            IHostEnvironment hostEnvironment)
        {
            _generatePDFService = generatePDFService;
            _hosting = hostEnvironment;
        }

        [HttpPost]
        public IActionResult GeneratePDFCheckList([FromBody]DataToGenerateModel model)
        {
            try
            {
                //string path = GetDataForGenerateCheckList(model);
                string dataResp = GetDataForGenerateCheckList(model);

                return Json(
                              new
                              {
                                  success = true,
                                  pdfPath = dataResp,
                              }
                    );
            }
            catch (Exception ex)
            {
                return Json(
                            new
                            {
                                success = false,
                                message = ex.Message, //InnerException(ex),
                                data = new[] { ex.Message },
                            }
               );
            }
        }

        public string GetDataForGenerateCheckList(DataToGenerateModel model)
        {
            var guid = Guid.NewGuid();
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    DataGenerateCheckListResp dataForGenPdf = _generatePDFService.GetDataToGeneratePDF(model);

                    DataDocumentModel genDocumentNo = _generatePDFService.GenerateDocumentNO(model.ProjectID);

                    string pathUrl = GeneratePDF(guid, dataForGenPdf, genDocumentNo);

                    var userID = Request.Cookies["CST.ID"];

                    var SaveTableResourc = new DataSaveTableResource { UnitFormID = dataForGenPdf.HeaderData.UnitFormID 
                                                                     , documentRunning = genDocumentNo.documentRunning
                                                                     , documentPrefix = genDocumentNo.documentPrefix
                                                                     , documentNo = genDocumentNo.documentNo
                                                                     , FilePath = pathUrl
                                                                     , FileName = guid.ToString()
                                                                     , UserID = Guid.Parse(userID)
                    };

                    bool ResultSave = _generatePDFService.SaveFileDocument(SaveTableResourc);

                    //string path = MegreMyPdfs(guid, reportDetail.OrderNO);

                    //_approve.SaveFilePDF(guid, transId, report1.OrderNO, path);

                    scope.Complete();
                    //service add resource  
                    return pathUrl;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        public string GeneratePDF(Guid guid, DataGenerateCheckListResp dataGenerate, DataDocumentModel genDocumentNo)
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
                                if(group == 0)
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

                                        grid.Item(6).Border(0.5f).Width(250).Image(img);
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

