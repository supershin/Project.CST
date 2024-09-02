using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MCompanyModel;
using Project.ConstructionTracking.Web.Models.MProjectModel;
using Project.ConstructionTracking.Web.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.ConstructionTracking.Web.Controllers
{
    public class MasterCompanyController : BaseController
    {
        private readonly IMasterCompanyService _masterCompanyService;
        public MasterCompanyController(IMasterCompanyService masterCompanyService)
        {
            _masterCompanyService = masterCompanyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int companyID)
        {
            DetailCompanyModel resultData = _masterCompanyService.GetDetailCompanyVendor(companyID);
            if (resultData == null) throw new Exception("ไม่พบข้อมูล Company Vendor");

            return View(resultData);
        }

        [HttpPost]
        public JsonResult GetListMasterCompany(DTParamModel param, MasterCompanyModel criteria)
        {
            try
            {
                var resultData = _masterCompanyService.GetCompanyList(param, criteria);
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
        public JsonResult GetListMasterVendor(DTParamModel param, MasterCompanyModel criteria)
        {
            try
            {
                var resultData = _masterCompanyService.GetVendorList(param, criteria);
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
        public JsonResult CreateCompanyVendor(CreateCompanyVendorModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                CompanyVendorResp resultData = _masterCompanyService.CreateCompanyVendor(model);
                if (resultData == null) throw new Exception("เกิดข้อผิดพลาดในการสร้างข้อมูล");
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
                                data = new[] { ex.Message }
                            }
               );
            }
        }

        [HttpPost]
        public JsonResult MappingCompanyVendorProject(CompanyMappingProjectModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                DetailCompanyModel resultData = _masterCompanyService.CompanyVendorMappingProject(model);
                if (resultData == null) throw new Exception("เกิดข้อผิดพลาดในการ​ Mapping ข้อมูล");
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
                                data = new[] { ex.Message }
                            }
               );
            }
        }

        [HttpPost]
        public JsonResult CreateVendor(CreateVendorModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                CreateVendorResp resultData = _masterCompanyService.CreateVendor(model);
                if (resultData == null) throw new Exception("เกิดข้อผิดพลาดในการสร้างข้อมูล Vendor");
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
                                data = new[] { ex.Message }
                            }
               );
            }
        }

        [HttpPost]
        public JsonResult ActionDelete(DeleteCompanyVendorModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                DeleteCompanyVendorResp resultData = _masterCompanyService.ActionDeleteCompanyVendor(model);
                if (resultData == null) throw new Exception("เกิดข้อผิดพลาดในการสร้างลบข้อมูล");
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
                                data = new[] { ex.Message }
                            }
               );
            }
        }

        [HttpPost]
        public JsonResult ActionVendor(ActionVendorModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                ActionVendorResp resultData = _masterCompanyService.ActionVendor(model);
                if (resultData == null) throw new Exception("เกิดข้อผิดพลาดในการแก้ไข Vendor");
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
                                data = new[] { ex.Message }
                            }
               );
            }
        }
    }
}

