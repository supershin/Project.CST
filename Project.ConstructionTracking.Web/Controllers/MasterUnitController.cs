using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MUnitModel;
using Project.ConstructionTracking.Web.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.ConstructionTracking.Web.Controllers
{
    public class MasterUnitController : BaseController
    {
        private readonly IMasterUnitService _masterUnitService;
        public MasterUnitController(IMasterUnitService masterUnitService)
        {
            _masterUnitService = masterUnitService;
        }

        public IActionResult Index()
        {
            Guid userId = Guid.Parse(ViewBag.ID);
            int userRole = Int32.Parse(ViewBag.UserRole);

            if (userRole == SystemConstant.UserRole.ADMIN) userId = Guid.Empty;

            MasterUnitResp resp = _masterUnitService.GetModelUnitResp(userId, userRole);
           
            return View(resp);
        }

        [HttpPost]
        public JsonResult UnitList(DTParamModel param, MasterUnitModel criteria)
        {
            try
            {
                var resultData = _masterUnitService.ListMasterUnit(param, criteria);

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
        public JsonResult CreateUnit(CreateUnitModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                var resultData = _masterUnitService.CreateUnit(model);

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
        public JsonResult GetDetail(Guid unitID)
        {
            try
            {
                var resultData = _masterUnitService.GetProjectID(unitID);

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
        public JsonResult EditUnit(EditUnitModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                var resultData = _masterUnitService.EditUnit(model);

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
        public JsonResult DeleteUnit(Guid unitID)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                Guid RequestUserID = Guid.Parse(userID);
                int RequestRoleID = Int32.Parse(RoleID);

                var resultData = _masterUnitService.DeleteUnit(unitID, RequestUserID);

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
        public JsonResult GetPEFromProject(Guid projectID)
        {
            try
            {
                var resultData = _masterUnitService.GetPEFromProject(projectID);

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
        public JsonResult ActionMappingPE(ActionMappingPeModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                Guid RequestUserID = Guid.Parse(userID);

                var resultData = _masterUnitService.ActionMappingPE(model, RequestUserID);

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

