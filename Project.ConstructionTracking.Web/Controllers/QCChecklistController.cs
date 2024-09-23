using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models.QCModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class QCChecklistController : BaseController
    {
        public QCChecklistController()
        {

        }


        public IActionResult Index(QcActionModel model)
        {
            if(model.QcUnitCheckListID == null && model.QcUnitStatusID == null && model.Seq == null)
            {
                TempData["QcActionModel"] = model; // Store the model in TempData
                return RedirectToAction("CheckListDetail");
            }
            else
            {
                // return to qc5
                return RedirectToAction("Index");
            }
        }

        public IActionResult CheckListDetail()
        {
            var model = TempData["QcActionModel"] as QcActionModel; // Retrieve the model from TempData

            return View(model); // Pass the model to the view
        }

    }
}
