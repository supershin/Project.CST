using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.ConstructionTracking.Web.Controllers
{
    public class FormOverallController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IUnitService _unitService;
        private readonly IFormOverallService _formOverallService;

        public FormOverallController(IProjectService projectService
            ,IUnitService unitService
            ,IFormOverallService formOverallService)
        {
            _projectService = projectService;
            _unitService = unitService;
            _formOverallService = formOverallService;
        }
        
        public IActionResult Index()
        {
            FormOverallView formOverallView = new FormOverallView()
            {
                SelectProjectList = GetProjectSelectList(),
                SelectUnitTypeList = GetUnitTypeSelectList()
            };

            return View(formOverallView);
        }

        public List<SelectListItem> GetProjectSelectList()
        {
            var selectLists = new List<SelectListItem>();
            var lst = _projectService.GetProjectList();
            foreach (var item in lst)
            {
                selectLists.Add(new SelectListItem
                {
                    Value = item.ProjectID.ToString(),
                    Text = item.ProjectName
                });
            }
            return selectLists;
        }

        public List<SelectListItem> GetUnitTypeSelectList()
        {
            var selectLists = new List<SelectListItem>();
            var list = _unitService.GetUnitTypeList();
            foreach(var item in list)
            {
                selectLists.Add(new SelectListItem
                {
                    Value = item.ID.ToString(),
                    Text = item.Name
                }) ;
            }
            return selectLists;
        }

        public JsonResult ProjectFromList(Guid formID, int typeId)
        {
            ProjectFormListView form = _formOverallService.GetProjectFormList(formID, typeId);
            return Json(
                new
                {
                    success = true,
                    data = form
                });
        }
    }
}

