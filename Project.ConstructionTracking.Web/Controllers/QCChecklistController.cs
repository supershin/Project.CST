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
        private readonly IHostEnvironment _hosting;

        public QCChecklistController(IQcCheckListService qcCheckListService
            , IHostEnvironment hosting)
        {
            _qcCheckListService = qcCheckListService;
            _hosting = hosting;
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
                    var userID = Request.Cookies["CST.ID"];
                    Guid ID = Guid.Parse(userID);

                    resp = _qcCheckListService.CreateDuplicateQcCheckList((Guid)id, seq, ID);
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
                ID = resp.QCCheckListID != Guid.Empty ? resp.QCCheckListID : id,
                Seq = resp.QCCheckListID != Guid.Empty ? resp.Seq : seq,
                QcTypeID = qctypeid,
                QcCheckListID = qcchecklistid,
            };

            QcCheckListDetailResp dataModel = _qcCheckListService.GetQcCheckListDetail(model);
            ViewBag.QcID = dataModel.QcCheckList != null ? dataModel.QcCheckList.ID : Guid.Empty;

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

        [HttpPost]
        public JsonResult SaveQcCheckList(SaveTransQCCheckListModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                Guid user = Guid.Parse(userID);
                int role = Int32.Parse(RoleID);

                if (model.PeSignResource != null)
                {
                    validateUnitEquipmentSign(model.PeSignResource);
                }

                model.ApplicationPath = _hosting.ContentRootPath;
                var resultData = _qcCheckListService.SaveQcCheckList(model, user, role);
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
        private void validateUnitEquipmentSign(string signUser)
        {
            if (string.IsNullOrEmpty(signUser))
                throw new Exception("โปรดระบุลายเซ็นต์");
        }

        [HttpPost]
        public IActionResult DeleteImage(Guid qcID, int? detailID, Guid resourceID)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                Guid user = Guid.Parse(userID);
                var result = _qcCheckListService.DeleteImage(qcID, detailID, resourceID, user);
                if (result)
                {
                    return Ok(new { success = true, message = "Image deleted successfully." });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to delete image." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SubmitQcCheckList(SaveTransQCCheckListModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                Guid user = Guid.Parse(userID);
                int role = Int32.Parse(RoleID);

                if (model.PeSignResource != null)
                {
                    validateUnitEquipmentSign(model.PeSignResource);
                }

                model.ApplicationPath = _hosting.ContentRootPath;
                var saveData = _qcCheckListService.SaveQcCheckList(model, user, role);

                SubmitQcCheckListModel submitModel = new SubmitQcCheckListModel()
                {
                    QcID = saveData.QcID,
                    ProjectID = saveData.ProjectID,
                    UnitID = saveData.UnitID,
                    CheckListID = saveData.CheckListID,
                    QcTypeID = saveData.QcTypeID,
                    Seq = saveData.Seq
                };

                bool resultSubmit = _qcCheckListService.SubmitQcCheckList(submitModel, user, role);
                var resultData = new SubmitQcCheckListModel();

                if (resultSubmit) resultData = submitModel;
                    
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
