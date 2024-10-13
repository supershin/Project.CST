using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using Project.ConstructionTracking.Web.Models.QCModel;
using Project.ConstructionTracking.Web.Services;
using QuestPDF.Infrastructure;
using static Project.ConstructionTracking.Web.Commons.SystemConstant;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class SummaryUnitQCController : BaseController
    {
        private readonly IQcSummaryService _qcSummaryService;
        private readonly IQC5CheckService _QC5CheckService;
        public SummaryUnitQCController(IQcSummaryService qcSummaryService, IQC5CheckService qC5CheckService)
        {
            _qcSummaryService = qcSummaryService;
            _QC5CheckService = qC5CheckService;
        }

        public IActionResult Index(Guid projectId, string projectName, Guid unitId)
        {
            QcSummaryResp qcSummary = _qcSummaryService.GetQcSummary(projectId, unitId);
            qcSummary.ProjectName = projectName;

            return View(qcSummary);  
        }

        //[HttpPost]
        //public IActionResult CheckQC5MaxSeqStatus(Guid projectId, Guid unitId)
        //{
        //    try
        //    {
        //        var filterunitData = new QC5MaxSeqStatusChecklistModel { ProjectID = projectId, UnitID = unitId };

        //        QC5MaxSeqStatusChecklistModel QC5MaxSeqDetail = _QC5CheckService.CheckQC5MaxSeqStatusChecklist(filterunitData);

        //        if (QC5MaxSeqDetail != null)
        //        {
        //            int? Seq = QC5MaxSeqDetail.Seq + 1;

        //            if (QC5MaxSeqDetail.QCStatusID != 1 && QC5MaxSeqDetail.Seq != 5 && QC5MaxSeqDetail.ActionType == "submit")
        //            {
        //                return RedirectToAction("Index", "QC5Check", new { projectId, unitId, Seq });
        //            }
        //            else if (QC5MaxSeqDetail.ActionType != "submit")
        //            {
        //                return Json(new { success = false, message = "กรุณา Submit รายการตรวจQC ก่อนหน้า" });
        //            }
        //            else if (QC5MaxSeqDetail.Seq == 5)
        //            {
        //                return Json(new { success = false, message = "ไม่สามารถสร้างรายการตรวจเพิ่มได้เนื่องจากครบจำนวนครั้งที่กำหนด" });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = "รายการตรวจQC นี้ผ่านแล้ว" });
        //            }
        //        }
        //        else if (QC5MaxSeqDetail == null)
        //        {
        //            int? Seq = 1;

        //            return RedirectToAction("Index", "QC5Check", new { projectId, unitId, Seq });
        //        }

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "ผิดพลาดค้นหาข้อมูลไม่สำเร็จ" });
        //    }
        //}

        [HttpPost]
        public IActionResult CheckQC5MaxSeqStatus(Guid Project_ID, Guid Unit_ID)
        {
            try
            {
                var filterunitData = new QC5MaxSeqStatusChecklistModel { ProjectID = Project_ID, UnitID = Unit_ID };

                QC5MaxSeqStatusChecklistModel QC5MaxSeqDetail = _QC5CheckService.CheckQC5MaxSeqStatusChecklist(filterunitData);

                if (QC5MaxSeqDetail != null)
                {
                    int? NewSeq = QC5MaxSeqDetail.Seq + 1;

                    if (QC5MaxSeqDetail.QCStatusID != 1 && QC5MaxSeqDetail.Seq != 5 && QC5MaxSeqDetail.ActionType == "submit")
                    {
                        // Return success with NewSeq for redirection
                        return Json(new { success = true, NewSeq = NewSeq });
                    }
                    else if (QC5MaxSeqDetail.ActionType != "submit")
                    {
                        return Json(new { success = false, message = "กรุณา Submit รายการตรวจQC ก่อนหน้า" });
                    }
                    else if (QC5MaxSeqDetail.Seq == 5)
                    {
                        return Json(new { success = false, message = "ไม่สามารถสร้างรายการตรวจเพิ่มได้เนื่องจากครบจำนวนครั้งที่กำหนด" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "รายการตรวจQC นี้ผ่านแล้ว" });
                    }
                }
                else if (QC5MaxSeqDetail == null)
                {
                    int? NewSeq = 1;

                    // Return success with NewSeq = 1 for new sequence redirection
                    return Json(new { success = true, NewSeq = NewSeq });
                }

                return View();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ผิดพลาดค้นหาข้อมูลไม่สำเร็จ" });
            }
        }



    }
}
