using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MFormModel;
using Project.ConstructionTracking.Web.Services;
using PackageModel = Project.ConstructionTracking.Web.Models.MFormModel.PackageModel;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.ConstructionTracking.Web.Controllers
{
    public class MasterFormController : BaseController
    {
        private readonly IMasterFormService _masterForm;

        public MasterFormController(IMasterFormService masterForm)
        {
            _masterForm = masterForm;
        }

        public IActionResult Index()
        {
            ProjectTypeListModel projectTypeList = new ProjectTypeListModel()
            {
                ProjectTypeList = _masterForm.GetProjectTypeList()
            };

            return View(projectTypeList);
        }

        public IActionResult Detail(int ID)
        {
            ViewBag.ID = ID;

            FormTypeDetailModel qcList = new FormTypeDetailModel()
            {
                DataQcList = _masterForm.GetQcList(ID)
            };

            return View(qcList);
        }

        [HttpPost]
        public JsonResult JsonAjaxGridFormTypeList(DTParamModel param, MasterFormModel criteria)
        {
            try
            {
                var resultData = _masterForm.GetFormTypeList(param, criteria);
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
        public JsonResult ActionFormType(FormTypeModel model)
        {
            try
            {
                FormTypeResp resp = _masterForm.ActionFormType(model);
                if (resp == null) throw new Exception("เกิดข้อผิดพลาดในการอัพเดทประเภทฟอร์ม");

                return Json(new
                {
                    success = true,
                    data = resp,
                });
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult DeleteFormType(FormTypeModel model)
        {
            try
            {
                FormTypeResp delete = _masterForm.ActionFormType(model);
                if (delete == null) throw new Exception("เกิดข้อผิดพลาดในการลบประเภทฟอร์ม");

                return Json(new
                {
                    success = true,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult GetDetail(int FormTypeId)
        {
            try
            {
                DetailFormType detail = _masterForm.GetDetailFormType(FormTypeId);
                if (detail == null) throw new Exception("ไม่พบข้อมูลประเภทฟอร์ม");

                return Json(new
                {
                    success = true,
                    data = detail,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult AjaxGridFormList(DTParamModel param, int formTypeID)
        {
            try
            {
                var resultData = _masterForm.GetFormList(param, formTypeID);
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
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult AjaxGridFormGroupList(DTParamModel param, int formID)
        {
            try
            {
                var resultData = _masterForm.GetFormGroupList(param, formID);
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
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult AjaxGridFormPackageList(DTParamModel param, int groupID)
        {
            try
            {
                var resultData = _masterForm.GetFormPackageList(param, groupID);
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
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult AjaxGridFormCheckList(DTParamModel param, int packageID)
        {
            try
            {
                var resultData = _masterForm.GetFormCheckList(param, packageID);
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
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult GetFormDetail(int formID)
        {
            try
            {
                FormDetail detail = _masterForm.GetFormDetail(formID);
                if (detail == null) throw new Exception("ไม่พบข้อมูลฟอร์ม");

                return Json(new
                {
                    success = true,
                    data = detail,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult GetGroupDetail(int groupID)
        {
            try
            {
                GroupDetail detail = _masterForm.GetGroupDetail(groupID);
                if (detail == null) throw new Exception("ไม่พบข้อมูลกลุ่มฟอร์ม");

                return Json(new
                {
                    success = true,
                    data = detail,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult GetPackageDetail(int packageID)
        {
            try
            {
                PackageDetail detail = _masterForm.GetPackageDetail(packageID);
                if (detail == null) throw new Exception("ไม่พบข้อมูลแพ็คเกจฟอร์ม");

                return Json(new
                {
                    success = true,
                    data = detail,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult GetCheckDetail(int checkID)
        {
            try
            {
                CheckDetail detail = _masterForm.GetCheckListDetail(checkID);
                if (detail == null) throw new Exception("ไม่พบข้อมูลแพ็คเกจฟอร์ม");

                return Json(new
                {
                    success = true,
                    data = detail,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult ActionForm(FormModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                FormResp resp = _masterForm.ActionForm(model);
                if (resp == null) throw new Exception("เกิดข้อผิดพลาดในการอัพเดทฟอร์ม");

                return Json(new
                {
                    success = true,
                    data = resp,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }
        [HttpPost]
        public JsonResult DeleteForm(FormModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                FormResp delete = _masterForm.ActionForm(model);
                if (delete == null) throw new Exception("เกิดข้อผิดพลาดในการลบฟอร์ม");

                return Json(new
                {
                    success = true,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult ActionGroup(GroupModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                GroupResp resp = _masterForm.ActionFormGroup(model);
                if (resp == null) throw new Exception("เกิดข้อผิดพลาดในการอัพเดทกลุ่มฟอร์ม");

                return Json(new
                {
                    success = true,
                    data = resp,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }
        [HttpPost]
        public JsonResult DeleteGroup(GroupModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                GroupResp delete = _masterForm.ActionFormGroup(model);
                if (delete == null) throw new Exception("เกิดข้อผิดพลาดในการลบกลุ่มฟอร์ม");

                return Json(new
                {
                    success = true,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult ActionPackage(PackageModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                PackageResp resp = _masterForm.ActionFormPackage(model);
                if (resp == null) throw new Exception("เกิดข้อผิดพลาดในการอัพเดทแพ็คเกจฟอร์ม");

                return Json(new
                {
                    success = true,
                    data = resp,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }
        [HttpPost]
        public JsonResult DeletePackage(PackageModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                PackageResp delete = _masterForm.ActionFormPackage(model);
                if (delete == null) throw new Exception("เกิดข้อผิดพลาดในการลบแพ็คเกจฟอร์ม");

                return Json(new
                {
                    success = true,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult ActionCheckList(CheckListModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                CheckListResp resp = _masterForm.ActionFormCheckList(model);
                if (resp == null) throw new Exception("เกิดข้อผิดพลาดในการอัพเดทฟอร์มรายการตรวจสอบ");

                return Json(new
                {
                    success = true,
                    data = resp,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }
        [HttpPost]
        public JsonResult DeleteCheckList(CheckListModel model)
        {
            try
            {
                var userID = Request.Cookies["CST.ID"];
                var RoleID = Request.Cookies["CST.Role"];
                model.RequestUserID = Guid.Parse(userID);
                model.RequestRoleID = Int32.Parse(RoleID);

                CheckListResp delete = _masterForm.ActionFormCheckList(model);
                if (delete == null) throw new Exception("เกิดข้อผิดพลาดในการลบฟอร์มรายการตรวจสอบ");

                return Json(new
                {
                    success = true,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new[] { ex.Message }
                });
            }
        }
    }
}

