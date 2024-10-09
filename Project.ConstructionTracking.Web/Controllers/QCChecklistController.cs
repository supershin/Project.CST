using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models.QCModel;
using Project.ConstructionTracking.Web.Services;
using QuestPDF.Infrastructure;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class QCChecklistController : BaseController
    {
        private readonly IQcCheckListService _qcCheckListService;

        public QCChecklistController(IQcCheckListService qcCheckListService)
        {
            _qcCheckListService = qcCheckListService;
        }

        public IActionResult Index(QcActionModel model)
        {
            if (model.QcUnitCheckListID == null && model.QcUnitStatusID == null && model.Seq == null)
            {
                TempData["QcActionModel"] = JsonSerializer.Serialize(model); // Serialize model to JSON
                return RedirectToAction("CheckListDetail");
            }
            else
            {
                // return to qc5
                return RedirectToAction("Index");
            }
        }

        public IActionResult CheckListDetail(Guid? id, int qcchecklistid, int seq, int qctypeid, Guid projectid, Guid unitid, bool? iscreate)
        {
            DuplicateModelResp resp = new DuplicateModelResp();
            if (iscreate == true && id != Guid.Empty )
            {
                try
                {
                    resp = _qcCheckListService.CreateDuplicateQcCheckList((Guid)id, seq);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            GetDetailModel model = new GetDetailModel()
            {
                ProjectID = projectid,
                UnitID = unitid,
                ID = resp != null ? resp.QCCheckListID : Guid.Empty,
                Seq = resp != null ? resp.Seq : seq,
                QcTypeID = qctypeid,
                QcCheckListID = qcchecklistid,
            };

            QcCheckListDetailResp dataModel = _qcCheckListService.GetQcCheckListDetail(model);

            return View(dataModel); 
        }

        [HttpPost]
        public JsonResult VerifyQcCheckList(QcActionModel model)
        {
            try
            {
                var resultData = _qcCheckListService.VerifyQcCheckList(model);

                return Json(
                          new
                          {
                              success = true,
                              data = resultData,
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
    }
}
