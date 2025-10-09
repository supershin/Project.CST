using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Models.SendMail;
using Project.ConstructionTracking.Web.Services;
using System;
using System.Transactions;
using static Project.ConstructionTracking.Web.Models.PJMApproveModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class PJMApproveRepo : IPJMApproveRepo
    {
        private readonly ContructionTrackingDbContext _context;
        private readonly IGeneratePDFRepo _generatePDFRepo;
        private readonly IGeneratePDFService _generatePDFService;

        public PJMApproveRepo(ContructionTrackingDbContext context, IGeneratePDFRepo generatePDFRepo, IGeneratePDFService generatePDFService)
        {
            _context = context;
            _generatePDFRepo = generatePDFRepo;
            _generatePDFService = generatePDFService;
        }

        public List<PJMApproveModel.GetlistUnitDetail> GetListPJMApprove(PJMApproveModel.filterData filterData)
        {
            // First Query: Get Unit Details
            var unitDetails = (from t1 in _context.tr_UnitForm
                                join t2 in _context.tm_Project on t1.ProjectID equals t2.ProjectID into projects
                                from project in projects.DefaultIfEmpty()
                                join t3 in _context.tm_Unit on t1.UnitID equals t3.UnitID into units
                                from unit in units.DefaultIfEmpty()
                                join t4 in _context.tm_Vendor on t1.VendorID equals t4.ID into vendors
                                from vendor in vendors.DefaultIfEmpty()
                                join t5 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)2 } equals new { t5.UnitFormID, t5.RoleID } into unitFormActions
                                from unitFormAction in unitFormActions.DefaultIfEmpty()
                                join tpjm in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)3 } equals new { tpjm.UnitFormID, tpjm.RoleID } into PJmunitFormActions
                                from PJmunitFormAction in PJmunitFormActions.DefaultIfEmpty()
                                join t7 in _context.tm_Form on t1.FormID equals t7.ID into forms
                                from form in forms.DefaultIfEmpty()
                                where unitFormAction.StatusID == 6 || PJmunitFormAction.StatusID == 8 || PJmunitFormAction.StatusID == 9
                                orderby unit.UnitCode, t1.FormID
                                select new PJMApproveModel.GetlistUnitDetail
                                {
                                    UnitFormID = t1.ID,
                                    UnitFormActionID = unitFormAction.ID,
                                    ProjectName = project.ProjectName,
                                    UnitCode = unit.UnitCode,
                                    VendorID = t1.VendorID,
                                    VendorName = vendor.Name,
                                    FormID = t1.FormID,
                                    FormName = form.Name,
                                    RoleID = unitFormAction.RoleID,
                                    ActionType = unitFormAction.ActionType,
                                    PJMActionType = PJmunitFormAction.ActionType,
                                    ActionDate = unitFormAction.ActionDate.HasValue
                                    ? unitFormAction.ActionDate.Value.ToString("dd/MM/yyyy")
                                    : "", // Convert to string dd/MM/yyyy
                                    StatusID = t1.StatusID
                                }).ToList();
            return unitDetails;
        }

        public List<PJMApproveModel.GetlistChecklistPC> GetChecklistPJMApprove(PJMApproveModel.GetlistChecklistPC filterData)
        {
            var result = (from t1 in _context.tr_UnitForm
                          join t2 in _context.tm_FormGroup on t1.FormID equals t2.FormID into formGroups
                          from t2 in formGroups.DefaultIfEmpty()
                          join t3 in _context.tr_UnitFormPassCondition on new { UnitFormID = (Guid?)t1.ID, GroupID = (int?)t2.ID } equals new { t3.UnitFormID, t3.GroupID } into passConditions
                          from t3 in passConditions.DefaultIfEmpty()
                          join t4 in _context.tm_Project on t1.ProjectID equals t4.ProjectID into projects
                          from t4 in projects.DefaultIfEmpty()
                          join t5 in _context.tm_Unit on t1.UnitID equals t5.UnitID into units
                          from t5 in units.DefaultIfEmpty()
                          join t6 in _context.tm_Form on t1.FormID equals t6.ID into forms
                          from t6 in forms.DefaultIfEmpty()
                          join t8 in _context.tr_UnitFormAction.Where(a => a.RoleID == 3) on t1.ID equals t8.UnitFormID into actions
                          from t8 in actions.DefaultIfEmpty()
                          join t8PM in _context.tr_UnitFormAction.Where(a => a.RoleID == 2) on t1.ID equals t8PM.UnitFormID into Pmactions
                          from t8PM in Pmactions.DefaultIfEmpty()
                          join t9 in _context.tm_User on t8.UpdateBy equals t9.ID into users
                          from t9 in users.DefaultIfEmpty()
                          where t1.ID == filterData.UnitFormID && t3.ID != null
                          select new GetlistChecklistPC
                          {
                              UnitFormID = t1.ID,
                              ProjectID = t1.ProjectID,
                              ProjectName = t4.ProjectName,
                              UnitID = t1.UnitID,
                              UnitCode = t5.UnitCode,
                              UnitFormStatus = t1.StatusID,
                              FormID = t1.FormID,
                              FormName = t6.Name,
                              Grade = t1.Grade,
                              GroupID = t2.ID,
                              GroupName = t2.Name,
                              PC_ID = t3.ID,
                              LockStatusID = t3.LockStatusID,
                              PC_StatusID = t3.StatusID,
                              PC_FlagActive = t3.FlagActive,
                              PE_Remark = t3.PE_Remark,
                              PM_Remark = t3.PM_Remark,
                              PJM_Remark = t3.PJM_Remark,
                              PEUnLock_Remark = t3.PEUnLock_Remark,
                              PMUnLock_Remark = t3.PMUnLock_Remark,
                              PM_Actiontype = t8PM.ActionType,
                              PJM_Actiontype = t8.ActionType,
                              PJM_ActionBy = t9.FirstName + ' ' + t9.LastName,
                              PJM_ActionDate = t8.ActionDate,
                              PJM_StatusID = t8.StatusID,   
                              PJMUnitFormRemark = t8.Remark
                          }).ToList();

            return result;
        }

        public List<PJMApproveModel.GetImageUnlock> GetImageUnlock(PJMApproveModel.GetImageUnlock model)
        {
            var result = (from t1Image in _context.tr_UnitFormResource
                          join t2Image in _context.tm_Resource on t1Image.ResourceID equals t2Image.ID into resources
                          from resource in resources.DefaultIfEmpty()
                          where t1Image.UnitFormID == model.UnitFormID && t1Image.PassConditionID == model.PassConditionID && t1Image.RoleID == 1
                          select new GetImageUnlock
                          {
                              MasterResourceID = resource.ID,
                              FileName = resource.FileName,
                              FilePath = resource.FilePath
                          }).ToList();

            return result;
        }

        public string SaveOrUpdateUnitFormAction(PJMApproveModel.PJMApproveIU model)
        {
            string returnUrlDoc = string.Empty;

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(3)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    // Determine the StatusID based on the ListPCIC
                    int? statusToUpdate = model.ListPCIC != null && model.ListPCIC.Any(pc => pc.StatusID == 9 && pc.PC_FlagActive != 0) ? 9 : 8;

                    var unitFormAction = _context.tr_UnitFormAction.FirstOrDefault(a => a.UnitFormID == model.UnitFormID && a.RoleID == 3);

                    if (unitFormAction == null)
                    {
                        unitFormAction = new tr_UnitFormAction
                        {
                            UnitFormID = model.UnitFormID,
                            RoleID = 3,
                            ActionType = model.ActionType,
                            StatusID = statusToUpdate,
                            Remark = string.IsNullOrEmpty(model.Remark) ? "" : model.Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now),
                            ActionDate = DateTime.Now,
                            UpdateBy = model.UserID,
                            UpdateDate = DateTime.Now,
                            CreateBy = model.UserID,
                            CraeteDate = DateTime.Now
                        };

                        _context.tr_UnitFormAction.Add(unitFormAction);
                    }
                    else
                    {
                        unitFormAction.ActionType = model.ActionType;
                        unitFormAction.StatusID = statusToUpdate;
                        if (!string.IsNullOrEmpty(model.Remark))
                        {
                            if (unitFormAction.Remark != model.Remark)
                            {
                                unitFormAction.Remark = model.Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now);
                            }
                        }
                        else
                        {
                            unitFormAction.Remark = "";
                        }
                        unitFormAction.ActionDate = DateTime.Now;
                        unitFormAction.UpdateDate = DateTime.Now;
                        unitFormAction.UpdateBy = model.UserID;

                        _context.tr_UnitFormAction.Update(unitFormAction);
                    }

                    _context.SaveChanges();

                    int? statusForm = (statusToUpdate == 8) ? 7 : 8;
                    UpdateUnitForm(model.UnitFormID, model.ActionType, statusForm, model.UserID);

                    if (model.ListPCIC != null && model.ListPCIC.Count > 0)
                    {
                        foreach (var passConditionModel in model.ListPCIC)
                        {
                            var passCondition = _context.tr_UnitFormPassCondition.FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.GroupID == passConditionModel.Group_ID && pc.ID == passConditionModel.PC_ID && pc.FlagActive == true);

                            if (passCondition != null)
                            {
                                passCondition.StatusID = passConditionModel.StatusID;
                                if (!string.IsNullOrEmpty(passConditionModel.PJM_Remark))
                                {
                                    if (passCondition.PJM_Remark != passConditionModel.PJM_Remark)
                                    {
                                        passCondition.PJM_Remark = passConditionModel.PJM_Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now);
                                    }
                                }
                                else
                                {
                                    passCondition.PJM_Remark = "";
                                }
                                passCondition.UpdateBy = model.UserID;
                                passCondition.UpdateDate = DateTime.Now;

                                _context.tr_UnitFormPassCondition.Update(passCondition);
                            }
                            _context.SaveChanges();
                        }
                    }

                    InsertImagesPM(model, model.UserID, 3);

                    var modelgenpdf = new DataToGenerateModel
                    {
                        ProjectID = FormatExtension.AsGuid(model.ProjectID),
                        UnitID = FormatExtension.AsGuid(model.UnitID),
                        FormID = FormatExtension.AsInt(model.FormID)
                    };

                    if (model.ActionType == "submit" && model.allPassed == true)
                    {
                        try
                        {
                            returnUrlDoc = GenerateAndSavePDF(modelgenpdf, model.UserID);
                        }
                        catch (Exception pdfEx)
                        {
                            throw new Exception("ปลิ้น PDF ไม่สำเร็จ", pdfEx);
                        }
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("บันทึกลงฐานข้อมูลไม่สำเร็จ", ex);
                }
            }

            return returnUrlDoc;
        }

        public List<PJMRespondModel> GetPJMRespondSendEmailData(Guid unitFormId)
        {
            var result = (from t1 in _context.tr_UnitForm
                          join t2 in _context.tr_ProjectPermission on t1.ProjectID equals t2.ProjectID
                          join t3 in _context.tm_User on t2.UserID equals t3.ID
                          join t4 in _context.tr_PE_Unit on new { UserID = (Guid?)t3.ID, t1.UnitID } equals new { t4.UserID, t4.UnitID } into t4Group
                          from t4 in t4Group.DefaultIfEmpty()
                          join t5 in _context.tm_Project on t1.ProjectID equals t5.ProjectID into t5Group
                          from t5 in t5Group.DefaultIfEmpty()
                          join t6 in _context.tm_Unit on t1.UnitID equals t6.UnitID into t6Group
                          from t6 in t6Group.DefaultIfEmpty()
                          join t7 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)SystemConstant.UserRole.PJM } equals new { t7.UnitFormID, t7.RoleID } into t7Group
                          from t7 in t7Group.DefaultIfEmpty()
                          join t8 in _context.tm_User on t7.UpdateBy equals t8.ID into t8Group
                          from t8 in t8Group.DefaultIfEmpty()
                          join t9 in _context.tm_Form on t1.FormID equals t9.ID into t9Group
                          from t9 in t9Group.DefaultIfEmpty()
                          join t10 in _context.tm_UnitFormStatus on t1.StatusID equals t10.ID into t10Group
                          from t10 in t10Group.DefaultIfEmpty()
                          where t1.ID == unitFormId
                                && t1.FlagActive == true
                                && t2.FlagActive == true
                                && t3.FlagActive == true
                                && (t3.RoleID == SystemConstant.UserRole.PE || t3.RoleID == SystemConstant.UserRole.PM)
                                && (t3.RoleID != SystemConstant.UserRole.PE || t4 != null)
                          select new PJMRespondModel
                          {
                              StatusID = t1.StatusID,
                              StatusName = t10.Name,
                              Fullname = t3.RoleID == 1
                                            ? "(PE) " + t3.FirstName + " " + t3.LastName
                                            : t3.RoleID == 2
                                            ? "(PM) " + t3.FirstName + " " + t3.LastName
                                            : t3.FirstName + " " + t3.LastName,
                              FormName = t9 != null ? t9.Name + " " + t9.Description : null,
                              PJMFullName = t8.FirstName + " " + t8.LastName,
                              PJMActionDate = FormatExtension.FormatDateToDayMonthNameYearTime(t7.ActionDate),
                              PJMRemark = t7.Remark,
                              ProjectName = t5.ProjectName,
                              UnitCode = t6.UnitCode,
                              Email = t3.Email,
                              //Email = "firsty.shabby@gmail.com",
                              ListPJMRespondPassCondition = (from pc in _context.tr_UnitFormPassCondition
                                                            join gn in _context.tm_FormGroup on pc.GroupID equals gn.ID into gnGroup
                                                            from gn in gnGroup.DefaultIfEmpty()
                                                            join stn in _context.tr_RoleActionStatus on pc.StatusID equals stn.ID into stnGroup
                                                            from stn in stnGroup.DefaultIfEmpty()
                                                            where pc.UnitFormID == t1.ID && pc.FlagActive == true
                                                            select new PJMRespondPassConditionModel
                                                            {
                                                                FormGroupName = gn.Name,
                                                                PCStatusID = pc.StatusID,
                                                                PCStatusName = stn.Name,
                                                                RemarkPEPassCodition = pc.PE_Remark,
                                                                RemarkPMPassCodition = pc.PM_Remark,
                                                                RemarkPJMPassCodition = pc.PJM_Remark,
                                                            }).ToList()
                          }).ToList();

            return result;
        }

        private string GenerateAndSavePDF(DataToGenerateModel model, Guid? UserID)
        {
            try
            {
                DataGenerateCheckListResp dataForGenPdf = _generatePDFService.GetDataToGeneratePDF(model);

                DataDocumentModel genDocumentNo = _generatePDFRepo.GenerateDocumentNO(model.ProjectID, "PE");

                Guid NewGuid = Guid.NewGuid();

                string pathUrl = _generatePDFRepo.GeneratePDF(NewGuid, dataForGenPdf, genDocumentNo);

                var SaveTableResourc = new DataSaveTableResource
                {
                    UnitFormID = dataForGenPdf.HeaderData.UnitFormID,
                    QCUnitCheckListID = null,
                    documentRunning = genDocumentNo.documentRunning,
                    documentPrefix = genDocumentNo.documentPrefix,
                    documentNo = genDocumentNo.documentNo,
                    FilePath = pathUrl,
                    FileName = NewGuid.ToString(),
                    UserID = FormatExtension.AsGuid(UserID)
                };

                bool ResultSave = _generatePDFRepo.SaveFileDocument(SaveTableResourc);

                return pathUrl;
            }
            catch (Exception ex)
            {
                throw new Exception("ปลิ้น PDF ไม่สำเร็จ", ex);
            }
        }

        private void UpdateUnitForm(Guid? unitformID, string? actiontype, int? StatusID , Guid? UserID)
        {
            var UnitForm = _context.tr_UnitForm.FirstOrDefault(tr => tr.ID == unitformID);

            if (UnitForm != null)
            {
                UnitForm.StatusID = actiontype == "save" ? 9 : StatusID;
                UnitForm.UpdateBy = UserID;
                UnitForm.UpdateDate = DateTime.Now;

                _context.tr_UnitForm.Update(UnitForm);
                _context.SaveChanges();
            }
        }

        private void InsertImagesPM(PJMApproveIU model, Guid? userID, int RoleID)
        {
            if (model.Images != null && model.Images.Count > 0)
            {
                var folder = DateTime.Now.ToString("yyyyMM");
                var dirPath = Path.Combine(model.ApplicationPath, "wwwroot", "Upload", "document", folder, "PJMImage");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                foreach (var image in model.Images)
                {
                    if (image.Length > 0)
                    {
                        Guid guidId = Guid.NewGuid(); // Generate a new Guid for the file
                        string fileName = guidId + ".jpg"; // Set the file name with .jpg extension
                        var filePath = Path.Combine(dirPath, fileName); // Determine the full file path

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                        }

                        // Prepare the relative file path and replace backslashes with forward slashes
                        string relativeFilePath = Path.Combine("Upload", "document", folder, "PJMImage", fileName).Replace("\\", "/");

                        // Save the image details in the tm_Resource table
                        var newResource = new tm_Resource
                        {
                            ID = Guid.NewGuid(),
                            FileName = fileName,
                            FilePath = relativeFilePath, // Store the relative path with forward slashes
                            MimeType = "image/jpeg", // Ensure the MimeType is set to "image/jpeg"
                            FlagActive = true,
                            CreateBy = userID,
                            CreateDate = DateTime.Now,
                            UpdateBy = userID,
                            UpdateDate = DateTime.Now,
                        };
                        _context.tm_Resource.Add(newResource);

                        // Link this resource to the pass condition, if necessary
                        var newFormResource = new tr_UnitFormResource
                        {
                            FormID = model.FormID,
                            RoleID = RoleID,
                            UnitFormID = model.UnitFormID,
                            ResourceID = newResource.ID,
                            CreateDate = DateTime.Now,
                            CreateBy = userID,
                            UpdateBy = userID,
                            UpdateDate = DateTime.Now,
                        };
                        _context.tr_UnitFormResource.Add(newFormResource);
                    }
                }

                _context.SaveChanges();
            }
        }

        public AdminRespond GetAdminPJMRespond(Guid unitFormId)
        {
            int ROLE_PE = (int)SystemConstant.UserRole.PE;
            int ROLE_PM = (int)SystemConstant.UserRole.PM;
            int ROLE_PJM = (int)SystemConstant.UserRole.PJM;
            int ROLE_ADMIN = (int)SystemConstant.UserRole.ADMIN;

            // ===== 1) ดึงรายละเอียด UnitForm =====
            var uf = (from t1 in _context.tr_UnitForm
                      join t2 in _context.tr_UnitFormAction
                          on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)ROLE_PE }
                          equals new { t2.UnitFormID, t2.RoleID } into t2Group
                      from t2 in t2Group.DefaultIfEmpty()

                      join t3 in _context.tm_User on t2.UpdateBy equals t3.ID into t3Group
                      from t3 in t3Group.DefaultIfEmpty()

                      join t4 in _context.tr_UnitFormAction
                          on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)ROLE_PM }
                          equals new { t4.UnitFormID, t4.RoleID } into t4Group
                      from t4 in t4Group.DefaultIfEmpty()

                      join t4Pjm in _context.tr_UnitFormAction
                          on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)ROLE_PJM }
                          equals new { t4Pjm.UnitFormID, t4Pjm.RoleID } into t4PjmGroup
                      from t4Pjm in t4PjmGroup.DefaultIfEmpty()

                      join t5 in _context.tm_User on t4.UpdateBy equals t5.ID into t5Group
                      from t5 in t5Group.DefaultIfEmpty()

                      join t5Pjm in _context.tm_User on t4Pjm.UpdateBy equals t5Pjm.ID into t5PjmGroup
                      from t5Pjm in t5Group.DefaultIfEmpty()

                      join t6 in _context.tm_UnitFormStatus on t1.StatusID equals t6.ID into t6Group
                      from t6 in t6Group.DefaultIfEmpty()

                      join t7 in _context.tm_Project on t1.ProjectID equals t7.ProjectID into t7Group
                      from t7 in t7Group.DefaultIfEmpty()

                      join t8 in _context.tm_Unit on t1.UnitID equals t8.UnitID into t8Group
                      from t8 in t8Group.DefaultIfEmpty()

                      join t9 in _context.tm_Form on t1.FormID equals t9.ID into t9Group
                      from t9 in t9Group.DefaultIfEmpty()

                      where t1.ID == unitFormId
                      select new
                      {
                          ProjectID = t1.ProjectID,
                          StatusName = t6 != null ? t6.Name : "",
                          StatusID = t1.StatusID,
                          FormName = t9 != null ? (t9.Name ?? "") + " " + (t9.Description ?? "") : "",
                          PMFullname = t5 != null ? (t5.FirstName ?? "") + " " + (t5.LastName ?? "") : "",
                          ActionDate = FormatExtension.FormatDateToDayMonthNameYearTime(t4.ActionDate),
                          PJMFullname = t5Pjm != null ? (t5Pjm.FirstName ?? "") + " " + (t5Pjm.LastName ?? "") : "",
                          PJMActionDate = FormatExtension.FormatDateToDayMonthNameYearTime(t4Pjm.ActionDate),
                          ProjectName = t7 != null ? t7.ProjectName : "",
                          UnitCode = t8 != null ? t8.UnitCode : "",
                          PMRemark = t4 != null ? t4.Remark : "",
                          PJMRemark = t4 != null ? t4Pjm.Remark : ""
                      }).FirstOrDefault();

            if (uf == null) return null;

            var response = new AdminRespond
            {
                StatusName = uf.StatusName,
                StatusID = uf.StatusID,
                FormName = uf.FormName,
                PMFullname = uf.PMFullname,
                ActionDate = uf.ActionDate,
                PJMFullName = uf.PJMFullname,
                PJMActionDate = uf.PJMActionDate,
                ProjectName = uf.ProjectName,
                UnitCode = uf.UnitCode,
                PMRemark = uf.PMRemark,
                PJMRemark = uf.PJMRemark
            };

            // ===== 2) ดึงรายชื่อ Admin ตาม Project =====
            var adminList = (from u in _context.tm_User
                             join pp in _context.tr_ProjectPermission on u.ID equals pp.UserID
                             where u.RoleID == ROLE_ADMIN
                                   && u.FlagActive == true
                                   && pp.ProjectID == uf.ProjectID
                             select new ListSendEmailAdmin
                             {
                                 AdminFullname = (u.FirstName ?? "") + " " + (u.LastName ?? ""),
                                 AdminEmail = u.Email ?? ""
                                 //AdminEmail = "sittikron.p@assetwise.co.th"
                             })
                             .Distinct()
                             .ToList();

            response.ListSendEmailAdmin = adminList;

            // ===== 3) ดึงรายการ Pass Condition สำหรับ Admin =====
            var passConds = (from pc in _context.tr_UnitFormPassCondition
                             join gn in _context.tm_FormGroup on pc.GroupID equals gn.ID into gnGroup
                             from gn in gnGroup.DefaultIfEmpty()
                             where pc.UnitFormID == unitFormId && pc.FlagActive == true
                             select new AdminRespondPassConditionModel
                             {
                                 FormGroupName = gn != null ? gn.Name : "",
                                 PCStatusID = pc.StatusID,
                                 PCStatusName = null, // ใส่ชื่อได้ถ้ามีตารางสถานะ
                                 RemarkPEPassCodition = pc.PE_Remark,
                                 RemarkPMPassCodition = pc.PM_Remark,
                                 RemarkPJMPassCodition = pc.PJM_Remark,
                             }).ToList();

            response.ListAdminRespondPassCondition = passConds;

            return response;
        }
    }
}

