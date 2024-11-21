using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Library.DAL;
using Project.ConstructionTracking.Web.Models.StoreProcedureModel;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;
using Project.ConstructionTracking.Web.Commons;
using Microsoft.CodeAnalysis;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UnitPaymentController : BaseController
    {
        private readonly MasterManagementProviderProject _unitstatusProvider;
        private readonly IGetDDLService _getDDLService;
        private readonly IUnitFormPaymentService _UnitFormPaymentService;
        private readonly IHostEnvironment _hosting;

        public UnitPaymentController(MasterManagementProviderProject unitstatusProvider, IGetDDLService getDDLService , IUnitFormPaymentService UnitFormPaymentService, IHostEnvironment hosting)
        {
            _unitstatusProvider = unitstatusProvider;
            _getDDLService = getDDLService;
            _UnitFormPaymentService = UnitFormPaymentService;
            _hosting = hosting;
        }
        public IActionResult Index()
        {
            
            var ddlModel = new GetDDL { Act = "ProjectAdmin" };
            List<GetDDL> ListProject = _getDDLService.GetDDLList(ddlModel);
            ViewBag.DDLProject = ListProject;

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
                int RoleID = int.TryParse(Request.Cookies["CST.Role"], out var tempRoleInt) ? tempRoleInt : -1;

                if (RoleID == SystemConstant.UserRole.ADMIN)
                {
                    Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;

                    // Check the total percentage for the UnitFormID
                    var Filters = new GetDDL { Act = "GetListUnitFormPayment", GuID = Model.UnitFormID };
                    List<GetDDL> CheckPercentPayment = _getDDLService.GetDDLList(Filters);
                    decimal totalValuedecimalSum = CheckPercentPayment?.Where(x => x.Valuedecimal.HasValue).Sum(x => x.Valuedecimal.Value) ?? 0;

                    if (totalValuedecimalSum > 100)
                    {
                        returnmessage = "งวดงานนี้เบิกครบ 100% แล้ว";
                        return Json(new { success = false, message = returnmessage });
                    }

                    if (totalValuedecimalSum + Model.PercentPayment > 100)
                    {
                        returnmessage = "ไม่สามารถเบิกงวดงานเกิน 100% ได้";
                        return Json(new { success = false, message = returnmessage });
                    }

                    // Insert the new GR payment if all validations pass
                    Model.UserID = userid;
                    returnmessage = _UnitFormPaymentService.InsertNewGRPayment(Model);
                    return Json(new { success = true, message = returnmessage });
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

                    if (CheckPercentPayment.Count > 0)
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
                int RoleID = int.TryParse(Request.Cookies["CST.Role"], out var tempRoleInt) ? tempRoleInt : -1;
                if (RoleID == SystemConstant.UserRole.ADMIN)
                {
                    Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;

                    var Filters = new GetDDL { Act = "GetUnitFormPayment", GuID = Model.ID };
                    List<GetDDL> CheckPercentPayment = _getDDLService.GetDDLList(Filters);

                    if (CheckPercentPayment.Count > 0)
                    {
                        if (CheckPercentPayment[0].Value == SystemConstant.Ext.SyncFail)
                        {
                            Model.UserID = userid;
                            returnmessage = _UnitFormPaymentService.SyncGRPayment(Model);
                            return Json(new { success = true, message = returnmessage });
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
        public IActionResult SearchClick(string projectId, string unitSearch)
        {

            var userID = Request.Cookies["CST.ID"];

            var en = new WorkPeriodModel
            {
                act = "Getworkperiodlist",
                project_id = projectId == null ? "" : projectId,
                unit_id = unitSearch == null ? "" : unitSearch,
                unit_status = "",
                user_id = userID

            };
            List<WorkPeriodModel> UnitFormpaymentlist = _unitstatusProvider.sp_get_workperiod(en);

            return PartialView("PartialTable", UnitFormpaymentlist);
        }
    }
}
