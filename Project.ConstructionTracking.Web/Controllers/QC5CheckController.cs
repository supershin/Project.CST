using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using Project.ConstructionTracking.Web.Services;
using System.Text.RegularExpressions;

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

            ViewBag.ProjectId = QC5CheckDetail.ProjectID;
            ViewBag.ProjectName = QC5CheckDetail.ProjectName;
            ViewBag.UnitId = QC5CheckDetail?.UnitID;
            ViewBag.UnitCode = QC5CheckDetail?.UnitCode;
            ViewBag.UnitStatusName = QC5CheckDetail?.UnitStatusName;
            ViewBag.Seq = QC5CheckDetail?.Seq;
            ViewBag.QC5UpdateByName = QC5CheckDetail?.QC5UpdateByName;

            // Autocomplete 1
            var filterModel = new GetDDL { Act = "DefectArea", ID = QC5CheckDetail?.ProjectTypeID, searchTerm = "" };
            List<GetDDL> ListDefectArea = _getDDLService.GetDDLList(filterModel);
            ViewBag.ListDefectArea = ListDefectArea;

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

        [HttpPost]
        public IActionResult SaveDefectData(QC5IUDModel model)
        {
            try
            {
                // Call the service to insert the data
                _QC5CheckService.InsertQCUnitCheckListDefect(model);

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
