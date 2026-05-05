using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;
using Project.ConstructionTracking.Web.Commons;
using Microsoft.CodeAnalysis;
using System;
using NuGet.Protocol.Plugins;
using Project.ConstructionTracking.Web.Infras.Services;
using ClosedXML.Excel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitPaymentController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        private readonly IGetDDLService _getDDLService;
        private readonly IUnitFormPaymentService _UnitFormPaymentService;
        private readonly IHostEnvironment _hosting;
        private readonly IConfiguration _config;
        private readonly string _VendorPortal;

        public UnitPaymentController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService, IUnitFormPaymentService UnitFormPaymentService, IHostEnvironment hosting,
            IConfiguration configuration)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
            _UnitFormPaymentService = UnitFormPaymentService;
            _hosting = hosting;
            _config = configuration;
            _VendorPortal = configuration["VendorPortal:Url"];
        }
        public IActionResult Index()
        {

            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;


            var ExtPaymentModel = new GetDDL { Act = "Ext", ID = SystemConstant.Ext_Type.PercentPayment };
            List<GetDDL> ListExtPayment = _getDDLService.GetDDLList(ExtPaymentModel);
            ViewBag.DDLPercentPayment = ListExtPayment;


            var ExtStatusGrPayment = new GetDDL { Act = "Ext", ID = SystemConstant.Ext_Type.StatusGrPayment };
            List<GetDDL> ListExtStatusGrPayment = _getDDLService.GetDDLList(ExtStatusGrPayment);
            ViewBag.DDLExtStatusGrPayment = ListExtStatusGrPayment;

            var userID = Request.Cookies["CST.ID"];

            var en = new WorkPeriodModel
            {
                act = "Getworkperiodlist",
                project_id = "",
                unit_id = "",
                unit_status = "",
                user_id = userID

            };
            List<WorkPeriodModel> WorkPeriodlists = _unitstatusProvider.sp_get_workperiod(en);
            return View(WorkPeriodlists);
        }

        [HttpGet]
        public IActionResult GetDDLUnitPass(Guid ProjectID)
        {
            var ddlModel = new GetDDL { Act = "GetListUnitPass", GuID = ProjectID };
            List<GetDDL> ListUnitPass = _getDDLService.GetDDLList(ddlModel);
            return Json(ListUnitPass);
        }

        [HttpGet]
        public IActionResult GetUnitFormGRDetail(Guid UnitFormID)
        {
            var Model = new UnitFormPaymentModel.getUnitFormGRDetail { UnitFormID = UnitFormID };
            UnitFormPaymentModel.getUnitFormGRDetail UnitFormGRDetail = _UnitFormPaymentService.getUnitFormGRDetail(Model);
            return Json(UnitFormGRDetail);
        }

        public IActionResult FetchListUnitFormGRPaymentTable(Guid UnitFormID)
        {
            var Model = new UnitFormPaymentModel.getListUnitFormGRPaymentTable
            {
                UnitFormID = UnitFormID
            };

            List<UnitFormPaymentModel.getListUnitFormGRPaymentTable> ModelTableListUnitFormGRPayment = _UnitFormPaymentService.GetListUnitFormGRPaymentTable(Model);

            return PartialView("PartialTableListUnitFormGRPayment", ModelTableListUnitFormGRPayment);
        }

        public IActionResult FetchListUnitFormViewGRPaymentTable(Guid UnitFormID)
        {
            try
            {
                var model = new UnitFormPaymentModel.getListUnitFormGRPaymentTable
                {
                    UnitFormID = UnitFormID
                };

                List<UnitFormPaymentModel.getListUnitFormGRPaymentTable> modelTableListUnitFormGRPayment = _UnitFormPaymentService.GetListUnitFormGRPaymentTable(model);

                // Return JSON instead of a partial view
                return Json(modelTableListUnitFormGRPayment);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return error message in JSON format
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult SaveUnitFormGRPaymentData(UnitFormPaymentModel.IUDGRPayment Model)
        {
            try
            {
                // Check for missing or invalid required fields
                if (string.IsNullOrWhiteSpace(Model.GRNO))
                {
                    return Json(new { success = false, message = "กรุณาระบุ GR" });
                }

                if (string.IsNullOrWhiteSpace(Model.PONO))
                {
                    return Json(new { success = false, message = "กรุณาระบุ PO" });
                }

                if (!Model.PercentPayment.HasValue || Model.PercentPayment <= 0)
                {
                    return Json(new { success = false, message = "กรุณาระบุเปอร์เซ็นต์ที่มากกว่า 0" });
                }

                string returnmessage = "";
                int syncStatusID = 0;
                int RoleID = int.TryParse(Request.Cookies["CST.Role"], out var tempRoleInt) ? tempRoleInt : -1;

                if (RoleID == SystemConstant.UserRole.ADMIN)
                {
                    Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;

                    // Check the total percentage for the UnitFormID
                    var Filters = new GetDDL { Act = "GetListUnitFormPayment", GuID = Model.UnitFormID };
                    List<GetDDL> CheckPercentPayment = _getDDLService.GetDDLList(Filters);
                    decimal totalValuedecimalSum = CheckPercentPayment?.Where(x => x.Valuedecimal.HasValue).Sum(x => x.Valuedecimal.Value) ?? 0;

                    if (totalValuedecimalSum >= 100)
                    {
                        returnmessage = "งวดงานนี้เบิกครบ 100% แล้ว";
                        return Json(new { success = false, message = returnmessage });
                    }

                    if (totalValuedecimalSum + Model.PercentPayment > 100)
                    {
                        returnmessage = "ไม่สามารถเบิกงวดงานเกิน 100% ได้";
                        return Json(new { success = false, message = returnmessage });
                    }

                    var CheckSortUnitform = new GetDDL { Act = "CheckSortUnitform", GuID = Model.UnitFormID };
                    List<GetDDL> SortUnitform = _getDDLService.GetDDLList(CheckSortUnitform);

                    var ChkItem = SortUnitform.FirstOrDefault();
                    if (ChkItem != null && ChkItem.Value > 1)
                    {
                        var CheckUnitPayment = new GetDDL { Act = "CheckUnitPayment", GuID = Model.UnitID, GuID2 = Model.UnitFormID };
                        List<GetDDL> ResCheckUnitPayment = _getDDLService.GetDDLList(CheckUnitPayment);

                        // Retrieve the single item (or null if empty)
                        var singleItem = ResCheckUnitPayment.FirstOrDefault();

                        if (singleItem != null)
                        {
                            if (singleItem.Value3 != 4 && singleItem.Value3 != 11)
                            {
                                returnmessage = (singleItem.Value3 > 5) ? "งวดงานก่อนหน้ายังไม่ผ่านการปลดล็อค" : "งวดงานก่อนหน้ายังไม่ผ่าน";
                                return Json(new { success = false, message = returnmessage });
                            }
                            else if (singleItem.Value2 < 1)
                            {
                                returnmessage = "ไม่สามารเบิกข้ามงวดได้";
                                return Json(new { success = false, message = returnmessage });
                            }
                        }
                    }

                    // Insert the new GR payment if all validations pass
                    Model.UserID = userid;
                    Model.ApplicationPath = _hosting.ContentRootPath;

                    syncStatusID = _UnitFormPaymentService.InsertNewGRPayment(Model);
                    if (syncStatusID == 31) {
                        try
                        {
                            //var ModelPaymentMail = new UnitPaymentMail { 
                            //       VendorFullName = "Sittikron Handsome"
                            //      ,VendorEmail = "firsty.shabby@gmail.com"
                            //      ,ProjectName = "Test"
                            //      ,UnitCode = "A3021"
                            //};

                            UnitPaymentMail ModelPaymentMail = _UnitFormPaymentService.getUnitFormSendmMailDetail(FormatExtension.AsGuid(Model.UnitFormID));

                            UnitPaymentSendMailData(ModelPaymentMail);
                        }
                        catch (Exception)
                        {

                        }
                    }


                    var ChkFilters = new GetDDL { Act = "GetListUnitFormPayment2", GuID = Model.UnitFormID };
                    List<GetDDL> CheckBackPercentPayment = _getDDLService.GetDDLList(ChkFilters);
                    decimal ChktotalValuedecimalSum = CheckBackPercentPayment?.Where(x => x.Valuedecimal.HasValue).Sum(x => x.Valuedecimal.Value) ?? 0;
                    if (ChktotalValuedecimalSum == 100)
                    {
                        returnmessage = "เบิกงวดงานนี้ครบ 100% แล้ว";
                        return Json(new { success = true, message = returnmessage });
                    }
                    else
                    {
                        return Json(new { success = true, message = returnmessage });
                    }
                }
                else
                {
                    returnmessage = "สิทธิ์บันทึกเบิกงวดไม่ถูกต้อง";
                    return Json(new { success = false, message = returnmessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult RemoveUnitFormGRPaymentData(UnitFormPaymentModel.IUDGRPayment Model)
        {
            try
            {
                string returnmessage = "";
                int RoleID = int.TryParse(Request.Cookies["CST.Role"], out var tempRoleInt) ? tempRoleInt : -1;
                if (RoleID == SystemConstant.UserRole.ADMIN)
                {
                    Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;

                    var Filters = new GetDDL { Act = "GetUnitFormPayment", GuID = Model.ID };
                    List<GetDDL> CheckPercentPayment = _getDDLService.GetDDLList(Filters);

                    if (CheckPercentPayment?.Count > 0)
                    {
                        if (CheckPercentPayment[0].Value == SystemConstant.Ext.SyncFail)
                        {
                            Model.UserID = userid;
                            returnmessage = _UnitFormPaymentService.RemoveGRPayment(Model);
                            return Json(new { success = true, message = returnmessage });
                        }
                        else
                        {
                            returnmessage = "ไม่สามารถลบเบิกงวดงานนี้ได้เนื่องจาก Sync Success แล้ว";
                            return Json(new { success = false, message = returnmessage });
                        }
                    }
                    else
                    {
                        returnmessage = "หาเบิกงวดงานนี้ไม่พบ";
                        return Json(new { success = false, message = returnmessage });
                    }
                }
                else
                {
                    returnmessage = "สิทธิ์เบิกงวดไม่ถูกต้อง";
                    return Json(new { success = false, message = returnmessage });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult SyncUnitFormGRPaymentData(UnitFormPaymentModel.IUDGRPayment Model)
        {
            try
            {
                string returnmessage = "";
                int syncStatusID = 0;
                int RoleID = int.TryParse(Request.Cookies["CST.Role"], out var tempRoleInt) ? tempRoleInt : -1;
                if (RoleID == SystemConstant.UserRole.ADMIN)
                {
                    Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;

                    var Filters = new GetDDL { Act = "GetUnitFormPayment", GuID = Model.ID };
                    List<GetDDL> CheckPercentPayment = _getDDLService.GetDDLList(Filters);

                    if (CheckPercentPayment?.Count > 0)
                    {
                        if (CheckPercentPayment[0].Value == SystemConstant.Ext.SyncFail)
                        {
                            Model.UserID = userid;
                            Model.ApplicationPath = _hosting.ContentRootPath;
                            syncStatusID = _UnitFormPaymentService.SyncGRPayment(Model);
                            if (syncStatusID == 31)
                            {
                                try
                                {
                                    UnitPaymentMail ModelPaymentMail = _UnitFormPaymentService.getUnitFormSendmMailDetail(FormatExtension.AsGuid(Model.UnitFormID));
                                    UnitPaymentSendMailData(ModelPaymentMail);
                                }
                                catch (Exception)
                                {

                                }
                            }
                            var ChkFilters = new GetDDL { Act = "GetListUnitFormPayment2", GuID = Model.UnitFormID };
                            List<GetDDL> CheckBackPercentPayment = _getDDLService.GetDDLList(ChkFilters);
                            decimal ChktotalValuedecimalSum = CheckBackPercentPayment?.Where(x => x.Valuedecimal.HasValue).Sum(x => x.Valuedecimal.Value) ?? 0;
                            if (ChktotalValuedecimalSum == 100)
                            {
                                returnmessage = "เบิกงวดงานนี้ครบ 100% แล้ว";
                                return Json(new { success = true, message = returnmessage });
                            }
                            else
                            {
                                return Json(new { success = true, message = returnmessage });
                            }
                        }
                        else
                        {
                            returnmessage = "ไม่สามารถ Sync เบิกงวดงานนี้ได้เนื่องจาก Sync Success แล้ว";
                            return Json(new { success = false, message = returnmessage });
                        }
                    }
                    else
                    {
                        returnmessage = "หาเบิกงวดงานนี้ไม่พบ";
                        return Json(new { success = false, message = returnmessage });
                    }
                }
                else
                {
                    returnmessage = "สิทธิ์เบิกงวดไม่ถูกต้อง";
                    return Json(new { success = false, message = returnmessage });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult SearchClick(string projectId, string unitSearch, string status)
        {

            var userID = Request.Cookies["CST.ID"];

            var en = new WorkPeriodModel
            {
                act = "Getworkperiodlist",
                project_id = projectId == null ? "" : projectId,
                unit_id = unitSearch == null ? "" : unitSearch,
                unit_status = status == null ? "" : status,
                user_id = userID

            };
            List<WorkPeriodModel> UnitFormpaymentlist = _unitstatusProvider.sp_get_workperiod(en);

            return PartialView("PartialTable", UnitFormpaymentlist);
        }

        private void UnitPaymentSendMailData(UnitPaymentMail model)
        {
            ViewBag.VendorPortalUrl = _VendorPortal;

            string template = RenderRazorViewtoString(this, "Template_UnitPayment_SendMail", model);
            var email = new EmailModel();
            email.Host = _config["Email:HOST"];
            email.From = _config["Email:FROM"];
            email.Sender = _config["Email:SENDER"];
            email.Username = _config["Email:USER_NAME"];
            email.Password = _config["Email:PASSWORD"];
            email.PORT = Convert.ToInt32(_config["Email:PORT"]);
            if (!string.IsNullOrEmpty(model.VendorEmail.ToStringNullable()))
                email.To = new List<string> { model.VendorEmail };
            email.Subject = _config["Email:Subject:HEADER_TEXT"];
            email.Body = template;

            (new MailService()).SendMail(email);
        }

        [HttpGet]
        public IActionResult ExportToExcel(string projectId, string unitSearch)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("รายงานการ Sysn Online Billing");

                DateTime Datenow = DateTime.Now;

                var ddlModel2 = new GetDDL { Act = "ProjectAdmin", GuID = Commons.FormatExtension.ConvertStringToGuid(projectId) };
                List<GetDDL> ProjectName = _getDDLService.GetDDLList(ddlModel2);

                // Add filter values
                worksheet.Cell(1, 1).Value = "ตัวเลือกการค้นหา";
                worksheet.Cell(2, 1).Value = "โครงการ :";
                worksheet.Cell(2, 2).Value = ProjectName != null && ProjectName.Count > 0 ? ProjectName[0].Text : projectId == null ? "ทั้งหมด" : "ไม่พบชื่อโครงการนี้" ;
                worksheet.Cell(3, 1).Value = "Exprort วันที่ :";
                worksheet.Cell(3, 2).Value = Commons.FormatExtension.FormatDateToDayMonthNameYearTime(Datenow); ;
                worksheet.Range(1, 1, 1, 3).Merge();
                worksheet.Range(2, 2, 2, 3).Merge();
                worksheet.Range(3, 2, 3, 3).Merge();

                // Row 1: Main headers
                worksheet.Cell(4, 1).Value = "ลำดับ";
                worksheet.Cell(4, 2).Value = "โครงการ";
                worksheet.Cell(4, 3).Value = "แปลง";
                worksheet.Cell(4, 4).Value = "งวดที่";
                worksheet.Cell(4, 5).Value = "PO";
                worksheet.Cell(4, 6).Value = "GR";
                worksheet.Cell(4, 7).Value = "บริษัท";
                worksheet.Cell(4, 8).Value = "ผู้รับเหมา";
                worksheet.Cell(4, 9).Value = "เปอร์เซ็นต์";
                worksheet.Cell(4, 10).Value = "หมายเหตุ";
                worksheet.Cell(4, 11).Value = "สถานะ";
                worksheet.Cell(4, 12).Value = "ข้อผิดพลาด";
                worksheet.Cell(4, 13).Value = "วันที่บันทึกล่าสุด";
                worksheet.Cell(4, 14).Value = "ผู้บันทึกล่าสุด";

                // Define the header range properly
                var headerRange = worksheet.Range(4, 1, 4, 14); // Corrected to include all columns in the range

                // Style the header row
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray; // Optional: Add a background color for clarity


                var en = new ReportExportSyncOnlineBillingModel
                {
                    act = "ReportExportSyncOnlineBilling",
                    project_id = projectId == null ? "" : projectId,
                    unit_id = unitSearch == null ? "" : unitSearch,
                };

                List<ReportExportSyncOnlineBillingModel> reportData = _unitstatusProvider.sp_get_report_ExportSyncOnlineBilling(en);

                if (reportData == null || reportData.Count == 0)
                {
                    // Handle the case with no data
                    worksheet.Cell(5, 1).Value = "ไม่มีข้อมูล";
                    worksheet.Range("A5:N5").Merge(); // Adjusted for 13 columns
                    worksheet.Cell(6, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(6, 1).Style.Font.Bold = true;
                }
                else
                {
                    // Populate data rows
                    int row = 5; // Starting row for data
                    foreach (var item in reportData)
                    {
                        worksheet.Cell(row, 1).Value = item.index;
                        worksheet.Cell(row, 2).Value = item.ProjectName;
                        worksheet.Cell(row, 3).Value = item.UnitCode;
                        worksheet.Cell(row, 4).Value = item.FormName;
                        worksheet.Cell(row, 5).Value = item.GRNO;
                        worksheet.Cell(row, 6).Value = item.PONO;
                        worksheet.Cell(row, 7).Value = item.CompanyVenderName;
                        worksheet.Cell(row, 8).Value = item.VenderName;
                        worksheet.Cell(row, 9).Value = item.PercentPayment;
                        worksheet.Cell(row, 10).Value = item.Remark;
                        worksheet.Cell(row, 11).Value = item.SyncStatusName;
                        worksheet.Cell(row, 12).Value = item.SyncMessage;
                        worksheet.Cell(row, 13).Value = item.UpdateDate;
                        worksheet.Cell(row, 14).Value = item.UpdateBy;

                        worksheet.Range(row, 1, row, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Range(row, 1, row, 14).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        row++;
                    }

                }

                // Adjust columns to fit the content
                worksheet.Columns().AdjustToContents();


                // Return the file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"รายงานการSysn_online_billing.xlsx"
                    );
                }
            }
        }
    }
}
