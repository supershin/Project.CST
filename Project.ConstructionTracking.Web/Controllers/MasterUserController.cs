using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MUnitModel;
using Project.ConstructionTracking.Web.Models.MUserModel;
using Project.ConstructionTracking.Web.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.ConstructionTracking.Web.Controllers
{
    public class MasterUser : BaseController
    {
        private readonly AppSettings _appSettings;
        private readonly IMasterUserService _masterUserService;
        private readonly IHostEnvironment _hosting;

        public MasterUser(IOptions<AppSettings> options,
            IMasterUserService masterUserService,
            IHostEnvironment hostEnvironment)
        {
            _appSettings = options.Value;
            _masterUserService = masterUserService;
            _hosting = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            UnitRespModel resp = _masterUserService.GetUnitResp();

            //string KeyPassword = _appSettings.PasswordKey;
            return View(resp);
        }

        public IActionResult Detail(string param)
        {
            string decode = HashExtension.DecodeFrom64(param);
            Guid userID = Guid.Parse(decode);

            DetailUserResp detailResp = _masterUserService.DetailUser(userID);
            detailResp.respModel = _masterUserService.GetUnitResp(detailResp.UserID);

            return View(detailResp);
        }

        [HttpPost]
        public JsonResult UserList(DTParamModel param, MasterUserModel criteria)
        {
            try
            {
                var resultData = _masterUserService.UserList(param, criteria);

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
        public JsonResult UserCreate(CreateUserModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);
                model.PasswordKey = _appSettings.PasswordKey;
                var resultData = _masterUserService.CreateUser(model);

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
        public JsonResult UserEdit(EditUserModel model)
        {
            try
            {
                //model.ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
                model.ApplicationPath = _hosting.ContentRootPath;

                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);
                model.KeyPassword = _appSettings.PasswordKey;
                if (model.SignUser != null)
                {
                    validateUnitEquipmentSign(model.SignUser);
                }

                var resultData = _masterUserService.EditUser(model);

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
        public JsonResult UserDelete(Guid userId)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                Guid RequestUserID = Guid.Parse(userID);
                int RequestRoleID = Int32.Parse(RoleID);

                var resultData = _masterUserService.DeleteUser(userId, RequestUserID);

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

