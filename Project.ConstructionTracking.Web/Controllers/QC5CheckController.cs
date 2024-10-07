using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using Project.ConstructionTracking.Web.Services;
using System;
using System.Text.RegularExpressions;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class QC5CheckController : BaseController
    {
        private readonly IQC5CheckService _QC5CheckService;
        private readonly IHostEnvironment _hosting;
        private readonly IGetDDLService _getDDLService;

        public QC5CheckController(IQC5CheckService QC5CheckService, IHostEnvironment hosting, IGetDDLService getDDLService)
        {
            _QC5CheckService = QC5CheckService;
            _hosting = hosting;
            _getDDLService = getDDLService;
        }

        public IActionResult Index(Guid projectId, Guid unitId, int Seq)
        {
            var filterunitData = new QC5DetailModel { ProjectID = projectId, UnitID = unitId, Seq = Seq };

            QC5DetailModel QC5CheckDetail = _QC5CheckService.GetQC5CheckDetail(filterunitData);

            ViewBag.ProjectId = QC5CheckDetail?.ProjectID;
            ViewBag.ProjectName = QC5CheckDetail?.ProjectName;
            ViewBag.UnitId = QC5CheckDetail?.UnitID;
            ViewBag.UnitCode = QC5CheckDetail?.UnitCode;
            ViewBag.QC5UnitChecklistID = QC5CheckDetail?.QC5UnitChecklistID;
            ViewBag.QC5UnitChecklistActionID = QC5CheckDetail?.QC5UnitChecklistActionID;
            ViewBag.UnitStatusName = QC5CheckDetail?.UnitStatusName;
            ViewBag.QC5UnitChecklistRemark = QC5CheckDetail?.QC5UnitChecklistRemark;
            ViewBag.PathQC5SignatureImage = QC5CheckDetail?.PathQC5SignatureImage;
            ViewBag.QC5SignatureDate = QC5CheckDetail?.QC5SignatureDate;
            ViewBag.QC5UnitStatusID = QC5CheckDetail?.QC5UnitStatusID;
            ViewBag.QC5UnitChecklistRemark = QC5CheckDetail?.QC5UnitChecklistRemark;
            ViewBag.Seq = Seq;
            ViewBag.QC5UpdateByName = QC5CheckDetail?.QC5UpdateByName;
            ViewBag.ActionType = QC5CheckDetail?.ActionType == "save" ? "บันทึกร่าง" : "ยืนยันแล้ว";
            ViewBag.ActionTypeEn = QC5CheckDetail?.ActionType;
            // Autocomplete 1
            var filterModel = new GetDDL { Act = "DefectArea", ID = QC5CheckDetail?.ProjectTypeID, searchTerm = "" };
            List<GetDDL> ListDefectArea = _getDDLService.GetDDLList(filterModel);
            ViewBag.ListDefectArea = ListDefectArea;


            var PEUnit = new GetDDL { Act = "PEUnit" , GuID = unitId};
            List<GetDDL> PEUnitID = _getDDLService.GetDDLList(PEUnit);
            ViewBag.PEID = (PEUnitID != null && PEUnitID.Count > 0) ? PEUnitID[0].ValueGuid : Guid.Empty;
            ViewBag.PEName = (PEUnitID != null && PEUnitID.Count > 0) ? PEUnitID[0].Text : "ยังไม่ได้ระบุ PE/SE ผู้ดูแล Unit แปลงนี้";

            var ddlModel = new GetDDL { Act = "ImageQC5Unit", GuID = QC5CheckDetail?.QC5UnitChecklistID };
            List<GetDDL> ImageQC5UnitList = _getDDLService.GetDDLList(ddlModel);
            ViewBag.ImageQC5UnitList = ImageQC5UnitList;



            var FilterData = new QC5ChecklistModel
            {
                QCUnitCheckListID = QC5CheckDetail?.QC5UnitChecklistID,
            };

            List<QC5ChecklistModel> listQCUnitCheckListDefects = _QC5CheckService.GetQCUnitCheckListDefects(FilterData);


            return View(listQCUnitCheckListDefects);
        }

        [HttpGet]
        public IActionResult GetDDLDefectType(int defectAreaId, string searchTerm = "")
        {
            //int iduse = Int32.TryParse(defectAreaId);

            var ddlModel = new GetDDL { Act = "DefectType", ID = defectAreaId, searchTerm = searchTerm };
            List<GetDDL> ListDefectType = _getDDLService.GetDDLList(ddlModel);
            return Json(ListDefectType);
        }

        [HttpGet]
        public IActionResult GetDDLDefectDescription(int defectTypeId, string searchTerm = "")
        {
            var ddlModel = new GetDDL { Act = "DefectDescription", ID = defectTypeId, searchTerm = searchTerm };
            List<GetDDL> ListDefectDescription = _getDDLService.GetDDLList(ddlModel);
            return Json(ListDefectDescription);
        }


        [HttpGet]
        public IActionResult GetQC5DefactEdit(int DefectID)
        {
            var filter = new QC5DefectModel { DefectID = DefectID };
            QC5DefectModel defectData = _QC5CheckService.GetQC5DefactEdit(filter);
            return Json(defectData); // Return the defect data as JSON
        }


        [HttpPost]
        public IActionResult SaveDefectData(QC5IUDModel model)
        {
            try
            {
                Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                model.ApplicationPath = _hosting.ContentRootPath;
                _QC5CheckService.InsertQCUnitCheckListDefect(model , userid);

                // Return success response
                return Json(new { success = true, message = "บันทึกข้อมูลสำเร็จ" });
            }
            catch (Exception ex)
            {
                // Return error response with the exception message
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult EditDefectData(QC5IUDModel model)
        {
            try
            {
                Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                model.ApplicationPath = _hosting.ContentRootPath;
                model.UserID = userid;
                _QC5CheckService.UpdateQCUnitCheckListDefect(model);

                // Return success response
                return Json(new { success = true, message = "บันทึกข้อมูลสำเร็จ" });
            }
            catch (Exception ex)
            {
                // Return error response with the exception message
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }


        [HttpPost]
        public IActionResult RemoveImage(Guid resourceId)
        {
            try
            {
                Guid UserID = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                _QC5CheckService.RemoveImage(resourceId, UserID);

                // Return success response
                return Json(new { success = true, message = "ลบรูปภาพสำเร็จ" });
            }
            catch (Exception ex)
            {
                // Return error response with the exception message
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }
        

        [HttpPost]
        public IActionResult RemoveDefectQC5Unit(QC5IUDModel model)
        {
            try
            {
                Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                model.UserID = userid;
                _QC5CheckService.RemoveQCUnitCheckListDefect(model);

                // Return success response
                return Json(new { success = true, message = "ลบข้อมูลสำเร็จ" });
            }
            catch (Exception ex)
            {
                // Return error response with the exception message
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult SaveSubmitQC5UnitCheckList(QC5SaveSubmitModel model)
        {
            try
            {
                // Deserialize the signature data if it's sent as a JSON string
                //if (Request.Form.ContainsKey("Sign"))
                //{
                //    var signJson = Request.Form["Sign"];
                //    model.Sign = JsonConvert.DeserializeObject<SignatureQC5>(signJson);
                //}

                Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                model.ApplicationPath = _hosting.ContentRootPath;
                model.UserID = userid;

                // Your service logic to save the data
                _QC5CheckService.SaveSubmitQC5UnitCheckList(model);


                // Return success response
                return Json(new { success = true, message = "บันทึกข้อมูลสำเร็จ" });
            }
            catch (Exception ex)
            {
                // Return error response with the exception message
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }


        [HttpPost]
        public IActionResult SaveSignature(QC5SaveSubmitModel model)
        {
            try
            {
                if (Request.Form.ContainsKey("Sign"))
                {
                    var signJson = Request.Form["Sign"];
                    model.Sign = JsonConvert.DeserializeObject<SignatureQC5>(signJson);
                }

                Guid userid = Guid.TryParse(Request.Cookies["CST.ID"], out var tempUserGuid) ? tempUserGuid : Guid.Empty;
                string ApplicationPath = _hosting.ContentRootPath;

                _QC5CheckService.SaveSignature(model.Sign , ApplicationPath, model.QCUnitCheckListID , userid);


                // Return success response
                return Json(new { success = true, message = "บันทึกข้อมูลสำเร็จ" });
            }
            catch (Exception ex)
            {
                // Return error response with the exception message
                return Json(new { success = false, message = $"ผิดพลาด : {ex.Message}" });
            }
        }

    }
}
