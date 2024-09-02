using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MProjectModel;
using Project.ConstructionTracking.Web.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.ConstructionTracking.Web.Controllers
{
    public class MasterProjectController : BaseController
    {
        private readonly IMasterProjectService _masterProjectService;
        public MasterProjectController(IMasterProjectService masterProjectService)
        {
            _masterProjectService = masterProjectService;
        }

        public IActionResult Index()
        {
            InformationProjectResp modelResp = _masterProjectService.InfomationForProejct();
            return View(modelResp);
        }

        [HttpPost]
        public JsonResult JsonAjaxGridProjectList(DTParamModel param, MasterProjectModel criteria)
        {
            try
            {
                var resultData = _masterProjectService.ListMasterProject(param, criteria);

                return Json(
                          new
                          {
                              success = true,
                              data = resultData,
                              param.draw,
                              iTotalRecords = param.TotalRowCount,
                              iTotalDisplayRecords = param.TotalRowCount
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
                                param.draw,
                                iTotalRecords = 0,
                                iTotalDisplayRecords = 0
                            }
               );
            }
        }

        [HttpPost]
        public JsonResult CreateProject(CreateProjectModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                var resultData = _masterProjectService.CreateProject(model);

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
        public JsonResult DetailProject(DetailProjectModel model)
        {
            try
            {
                var resultData = _masterProjectService.DetailProjectInformation(model.ProjectID);

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
        public JsonResult EditProject(EditProjectModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                var resultData = _masterProjectService.EditProject(model);

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
        public JsonResult DeleteProject(Guid projectId)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                Guid RequestUserID = Guid.Parse(userID);
                int RequestRoleID = Int32.Parse(RoleID);

                var resultData = _masterProjectService.DeleteProject(projectId, RequestUserID);

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

