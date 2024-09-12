using System;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Services;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

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

                    string pathUrl = GeneratePDF(guid, dataForGenPdf);

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

        public string GeneratePDF(Guid guid, DataGenerateCheckListResp dataGenerate)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var fontPath = _hosting.ContentRootPath + "/wwwroot/lib/fonts/BrowalliaUPC.ttf";

            FontManager.RegisterFont(System.IO.File.OpenRead(fontPath));

            var imageBox = Directory.GetCurrentDirectory() + "/wwwroot/img/icon/box.png";
            var imageCheckBox = Directory.GetCurrentDirectory() + "/wwwroot/img/icon/checkbox.png";
            var imageCheck = Directory.GetCurrentDirectory() + "/wwwroot/img/icon/check.png";

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
                               .FontSize(16));

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

                            table.Cell().Row(1).Column(1).ColumnSpan(5).Element(CellStyle).AlignLeft().Text("เลขที่ใบตรวจ: " + "C400H007-092024-00001").FontColor("#FF0000");

                            table.Cell().Row(2).Column(1).Element(CellStyle).AlignLeft().Text("โครงการ ");
                            table.Cell().Row(2).Column(2).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.ProjectName);
                            table.Cell().Row(2).Column(3).ColumnSpan(3).Element(CellStyle).AlignLeft().Text("วันที่ " + dataGenerate.HeaderData.PMSubmitDate);

                            table.Cell().Row(3).Column(1).Element(CellStyle).AlignLeft().Text("แปลงที่ ");
                            table.Cell().Row(3).Column(2).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.UnitCode);
                            table.Cell().Row(3).Column(3).RowSpan(3).Element(CellStyle).AlignLeft().Text("การตรวจ QC ");
                            table.Cell().Row(3).Column(4).Element(CellStyle).Width(15).Image(imageBox);
                            table.Cell().Row(3).Column(5).Element(CellStyle).AlignLeft().Text("ผ่านการตรวจจาก QC แล้ว");

                            table.Cell().Row(4).Column(1).Element(CellStyle).AlignLeft().Text("ผู้รับเหมา ");
                            table.Cell().Row(4).Column(2).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.VendorName);
                            table.Cell().Row(4).Column(4).Element(CellStyle).Width(15).Image(imageBox);
                            table.Cell().Row(4).Column(5).Element(CellStyle).AlignLeft().Text("งวดนี้ไม่มีการตรวจ QC");

                            table.Cell().Row(5).Column(1).Element(CellStyle).AlignLeft().Text("ผู้ควบคุมงาน ");
                            table.Cell().Row(5).Column(2).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.PEName);

                            table.Cell().Row(6).Column(1).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.FormName).BackgroundColor("#32ff32");
                            table.Cell().Row(6).Column(2).ColumnSpan(4).Element(CellStyle).AlignLeft().Text(dataGenerate.HeaderData.FormDesc);

                            // you can extend existing styles by creating additional methods
                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White);
                  
                        });
                    });

                    //int countRow = 1;
                    //page.Content().PaddingVertical(4).Column(col1 =>
                    //{
                    //    col1.Item().Row(row =>
                    //    {
                    //        row.RelativeItem(6).AlignRight().Text("รายการตรวจ").FontColor("#FF0000").FontSize(18).SemiBold();
                    //        row.RelativeItem(3).AlignRight().Text("ผลการตรวจ").FontSize(18).SemiBold().BackgroundColor("#6ce4ff");
                    //        row.RelativeItem(3).AlignRight().Text("ความคิดเห็น").FontSize(18).SemiBold().BackgroundColor("#6ce4ff");
                    //    });

                    //    col1.Item().Table(table =>
                    //    {
                    //        for(int group = 0; group < dataGenerate.BodyCheckListData.GroupDataModels.Count; group++)
                    //        {
                    //            table.ColumnsDefinition(columns =>
                    //            {
                    //                columns.RelativeColumn(3);
                    //                columns.RelativeColumn(3);
                    //                columns.RelativeColumn(1);
                    //                columns.RelativeColumn(1);
                    //                columns.RelativeColumn(1);
                    //                columns.RelativeColumn(3);
                    //            });

                    //            table.Cell().Row((uint)(group + countRow)).Column(1).Padding(2).PaddingLeft(4).Text(dataGenerate.BodyCheckListData.GroupDataModels[group].GroupName);
                    //            table.Cell().Row((uint)(group + countRow)).Column(3).Padding(2).PaddingLeft(4).Text("ผ่าน");
                    //            table.Cell().Row((uint)(group + countRow)).Column(4).Padding(2).PaddingLeft(4).Text("ไม่ผ่าน");
                    //            table.Cell().Row((uint)(group + countRow)).Column(5).Padding(2).PaddingLeft(4).Text("ไม่มีรายการนี้");

                    //            for(int package = 0; package < dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels.Count; package++)
                    //            {
                    //                table.Cell().Row((uint)(group + countRow + package + 1)).Column(1).RowSpan((uint)dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels.Count).Padding(2).PaddingLeft(4).Text(dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].PackageName);

                    //                for (int check = 0; check < dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels.Count; check++)
                    //                {
                    //                    table.Cell().Row((uint)(group + countRow + package + 1 + check)).Column(2).Padding(2).PaddingLeft(4).Text(dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels[check].CheckListName);
                    //                    if(dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels[check].StatusCheckList == SystemConstant.CheckList_Status.PASS)
                    //                    {
                    //                        table.Cell().Row((uint)(group + countRow + package + 1 + check)).Column(3).Width(30).Image(imageCheck); // icon
                    //                    }
                    //                    if (dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels[check].StatusCheckList == SystemConstant.CheckList_Status.NOTPASS)
                    //                    {
                    //                        table.Cell().Row((uint)(group + countRow + package + 1 + check)).Column(4).Width(30).Image(imageCheck);// icon
                    //                    }
                    //                    if (dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels[check].StatusCheckList == SystemConstant.CheckList_Status.NOWORK)
                    //                    {
                    //                        table.Cell().Row((uint)(group + countRow + package + 1 + check)).Column(5).Width(30).Image(imageCheck); // icon
                    //                    }
                    //                    table.Cell().Row((uint)(group + countRow + package + 1 + check)).Column(6).RowSpan((uint)dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels.Count).Padding(2).PaddingLeft(4).Text(dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].PackageRemark);
                    //                }

                    //                countRow += dataGenerate.BodyCheckListData.GroupDataModels[group].PackageDataModels[package].CheckListDataModels.Count;
                    //            }
                    //        }

                    //    });
                    //});

                    //page.Content()
                    //  .PaddingBottom(20)
                    //  .PaddingTop(20)
                    //  .AlignCenter()
                    //  .Grid(grid =>
                    //  {
                    //      grid.VerticalSpacing(15);
                    //      grid.HorizontalSpacing(15);
                    //      grid.AlignLeft();
                    //      grid.Columns(12);

                    //      for( int i = 0; i < dataGenerate.BodyImageData.GroupImages.Count ; i++)
                    //      {
                    //          grid.Item(12).Text(dataGenerate.BodyImageData.GroupImages[i].GroupName).FontSize(18).Bold().Underline().BackgroundColor("#6ce4ff");

                    //          for (int a = 0; a < dataGenerate.BodyImageData.GroupImages[i].ImageUploads.Count ; a++)
                    //          {
                    //              string pathImage = dataGenerate.BodyImageData.GroupImages[i].ImageUploads[a].PathImageUrl;
                    //              //var imgPath = Directory.GetCurrentDirectory() + "/images/works" + i + ".jpg";
                    //              var imgPath = _hosting.ContentRootPath + "/" + pathImage;
                    //              if (System.IO.File.Exists(imgPath))
                    //              {
                    //                  using var img = new FileStream(imgPath, FileMode.Open);

                    //                  grid.Item(6).Border(0.5f).Width(250).Image(img);
                    //              }
                    //          }

                    //      }

                    //  });

                    page.Footer().Border(0.5f).Table(table2 =>
                    {
                        var signVendor = new FileStream(Directory.GetCurrentDirectory() + "/" + dataGenerate.FooterData.VendorData.VendorImageSignUrl, FileMode.Open);
                        var signPe = new FileStream(Directory.GetCurrentDirectory() + "/" + dataGenerate.FooterData.PEData.PEImageSignUrl, FileMode.Open);
                        var signPm = new FileStream(Directory.GetCurrentDirectory() + "/" + dataGenerate.FooterData.PMData.PMImageSignUrl, FileMode.Open);
                        
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

                        table2.Cell().Row(1).Column(2).AlignCenter().Width(60).Image(signVendor);
                        table2.Cell().Row(2).Column(2).AlignCenter().Text("วิศวกรผู้ควบคุมงาน");
                        table2.Cell().Row(3).Column(2).AlignCenter().Text("( " + dataGenerate.FooterData.PEData.PEName + " )");

                        table2.Cell().Row(1).Column(3).AlignCenter().Width(60).Image(signVendor);
                        table2.Cell().Row(2).Column(3).AlignCenter().Text("Project Manager");
                        table2.Cell().Row(3).Column(3).AlignCenter().Text("( " + dataGenerate.FooterData.PMData.PMName + " )");

                        //QC
                        table2.Cell().Row(1).Column(4).AlignCenter().Width(60).Image("");
                        table2.Cell().Row(2).Column(4).AlignCenter().Text("ผู้รับเหมา");
                        table2.Cell().Row(3).Column(4).AlignCenter().Text("(          )");
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

