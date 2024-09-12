using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Project.ConstructionTracking.Web.Models.ApproveFormcheckIUDModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class PMApproveRepo : IPMApproveRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public PMApproveRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }
        public List<PMApproveModel> GetPMApproveFormList()
        {
            var result = (from t1 in _context.tr_UnitForm
                          join t2 in _context.tm_Vendor on t1.VendorID equals t2.ID into vendors
                          from vendor in vendors.DefaultIfEmpty()
                          join t3 in _context.tr_UnitFormAction.Where(a => a.RoleID == 1)
                              on t1.ID equals t3.UnitFormID into unitFormActionsPE
                          from actionPE in unitFormActionsPE.DefaultIfEmpty()
                          join t3m in _context.tr_UnitFormAction.Where(a => a.RoleID == 2)
                              on t1.ID equals t3m.UnitFormID into unitFormActionsPM
                          from actionPM in unitFormActionsPM.DefaultIfEmpty()
                          join t3J in _context.tr_UnitFormAction.Where(a => a.RoleID == 3)
                              on t1.ID equals t3J.UnitFormID into unitFormActionsPJM
                          from actionPJM in unitFormActionsPJM.DefaultIfEmpty()
                          join t4 in _context.tm_Project on t1.ProjectID equals t4.ProjectID into projects
                          from project in projects.DefaultIfEmpty()
                          join t5 in _context.tm_Unit on t1.UnitID equals t5.UnitID into units
                          from unit in units.DefaultIfEmpty()
                          join t6 in _context.tm_Form on t1.FormID equals t6.ID into forms
                          from form in forms.DefaultIfEmpty()
                          let allpassConditionCount = _context.tr_UnitFormPassCondition
                                                                  .Where(pc => pc.UnitFormID == t1.ID && pc.FlagActive == true)
                                                                  .Count()

                          let passConditionCount = _context.tr_UnitFormPassCondition
                                                      .Where(pc => pc.UnitFormID == t1.ID && pc.FlagActive == true
                                                                && (pc.StatusID == 6 || pc.StatusID == 8))
                                                      .Count()

                          let notpassConditionCount = _context.tr_UnitFormPassCondition
                                                      .Where(pc => pc.UnitFormID == t1.ID && pc.FlagActive == true
                                                                && (pc.StatusID == 7 || pc.StatusID == 9))
                                                      .Count()

                          let UnlockpassConditionall = _context.tr_UnitFormPassCondition
                                                      .Where(pc => pc.UnitFormID == t1.ID && pc.FlagActive == true && pc.LockStatusID == 8)
                                                      .Count()

                          let UnlockpassConditionapprove = _context.tr_UnitFormPassCondition
                                                      .Where(pc => pc.UnitFormID == t1.ID && pc.FlagActive == true && pc.LockStatusID == 8 && pc.StatusID == 13)
                                                      .Count()

                          let UnlockpassConditionreject = _context.tr_UnitFormPassCondition
                                                      .Where(pc => pc.UnitFormID == t1.ID && pc.FlagActive == true && pc.LockStatusID == 8 && pc.StatusID == 14)
                                                      .Count()

                          where t1.StatusID > 1
                          select new
                          {
                              t1.StatusID,
                              UnitFormID = t1.ID,
                              UnitFormActionID = actionPE.ID,
                              t1.ProjectID,
                              project.ProjectName,
                              t1.UnitID,
                              unit.UnitCode,
                              t1.VendorID,
                              VenderName = vendor.Name,
                              t1.Grade,
                              t1.FormID,
                              FormName = form.Name,
                              RoleID_PE = actionPE.RoleID,
                              ActionType_PE = actionPE.ActionType,
                              StatusID_PE = actionPE.StatusID,
                              Remark_PE = actionPE.Remark,
                              ActionDate_PE = actionPE.ActionDate,
                              RoleID_PM = actionPM.RoleID,
                              ActionType_PM = actionPM.ActionType,
                              StatusID_PM = actionPM.StatusID,
                              Remark_PM = actionPM.Remark,
                              ActionDate_PM = actionPM.ActionDate,
                              RoleID_PJM = actionPJM.RoleID,
                              ActionType_PJM = actionPJM.ActionType,
                              StatusID_PJM = actionPJM.StatusID,
                              Remark_PJM = actionPJM.Remark,
                              ActionDate_PJM = actionPJM.ActionDate,
                              PC_Cnt = allpassConditionCount,
                              AnswerColor = notpassConditionCount > 0 ? "bg-danger"
                                          : (notpassConditionCount == 0 && passConditionCount == allpassConditionCount && allpassConditionCount > 0) ? "bg-success"
                                          : (allpassConditionCount == 0) ? ""
                                          : (notpassConditionCount == 0 && allpassConditionCount > 0) ? "bg-primary"
                                          : "" ,
                              PC_UnlockAll = UnlockpassConditionall,
                              PC_UnlockColor = UnlockpassConditionreject > 0 ? "bg-danger" : UnlockpassConditionapprove == UnlockpassConditionall ? "bg-success" : "bg-primary"

                          })
                          .OrderBy(item => item.StatusID)
                          .ThenBy(item => item.UnitFormID)
                          .ToList();

            // Apply formatting after ordering is ensured
            var formattedResult = result
                .Select(item => new PMApproveModel
                {
                    UnitFormID = item.UnitFormID,
                    UnitFormActionID = item.UnitFormActionID,
                    ProjectID = item.ProjectID,
                    ProjectName = item.ProjectName,
                    UnitID = item.UnitID,
                    UnitCode = item.UnitCode,
                    VendorID = item.VendorID,
                    VenderName = item.VenderName,
                    Grade = item.Grade,
                    FormID = item.FormID,
                    FormName = item.FormName,
                    StatusID = item.StatusID,
                    RoleID_PE = item.RoleID_PE,
                    ActionType_PE = item.ActionType_PE,
                    StatusID_PE = item.StatusID_PE,
                    Remark_PE = item.Remark_PE,
                    ActionDate_PE = item.ActionDate_PE.HasValue
                                    ? FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(item.ActionDate_PE.Value)
                                    : null,
                    RoleID_PM = item.RoleID_PM,
                    ActionType_PM = item.ActionType_PM,
                    StatusID_PM = item.StatusID_PM,
                    Remark_PM = item.Remark_PM,
                    ActionDate_PM = item.ActionDate_PM.HasValue
                                    ? FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(item.ActionDate_PM.Value)
                                    : null,
                    RoleID_PJM = item.RoleID_PJM,
                    ActionType_PJM = item.ActionType_PJM,
                    StatusID_PJM = item.StatusID_PJM,
                    Remark_PJM = item.Remark_PJM,
                    ActionDate_PJM = item.ActionDate_PJM.HasValue
                                    ? FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(item.ActionDate_PJM.Value)
                                    : null,
                    PC_CNT = item.PC_Cnt,
                    PC_Color = item.AnswerColor,
                    PC_UnlockAll = item.PC_UnlockAll,
                    PC_UnlockColor = item.PC_UnlockColor
                })
                .ToList();

            return formattedResult;
        }

        public ApproveFormcheckModel GetApproveFormcheck(ApproveFormcheckModel model)
        {
            var result = (from t1 in _context.tr_UnitForm
                          join t2 in _context.tm_Vendor on t1.VendorID equals t2.ID into vendors
                          from vendor in vendors.DefaultIfEmpty()
                          join t4 in _context.tm_Project on t1.ProjectID equals t4.ProjectID into projects
                          from project in projects.DefaultIfEmpty()
                          join t4com in _context.tm_CompanyVendor on t1.CompanyVendorID equals t4com.ID into Companys
                          from Company in Companys.DefaultIfEmpty()
                          join t5 in _context.tm_Unit on t1.UnitID equals t5.UnitID into units
                          from unit in units.DefaultIfEmpty()
                          join t8 in _context.tm_Form on t1.FormID equals t8.ID into forms
                          from form in forms.DefaultIfEmpty()
                          join t10 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)1 } equals new { t10.UnitFormID, t10.RoleID } into PEUnitFormActions
                          from PEUnitFormAction in PEUnitFormActions.DefaultIfEmpty()
                          join t11 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)2 } equals new { t11.UnitFormID, t11.RoleID } into PMUnitFormActions
                          from PMUnitFormAction in PMUnitFormActions.DefaultIfEmpty()
                          join t12 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)3 } equals new { t12.UnitFormID, t12.RoleID } into PJMUnitFormActions
                          from PJMUnitFormAction in PJMUnitFormActions.DefaultIfEmpty()
                          join t13 in _context.tm_User on new { PMUnitFormAction.UpdateBy } equals new { UpdateBy = (Guid?)t13.ID } into PMUserActions
                          from PMUserAction in PMUserActions.DefaultIfEmpty()
                          where t1.UnitID == model.UnitID && t1.FormID == model.FormID
                          select new ApproveFormcheckModel
                          {
                              ID = t1.ID,
                              ProjectID = t1.ProjectID,
                              ProjectName = project.ProjectName,
                              UnitID = t1.UnitID,
                              UnitFormID = t1.ID,
                              UnitCode = unit.UnitCode,
                              VendorID = t1.VendorID,
                              VenderName = vendor.Name,
                              CompanyName = Company.Name,
                              VendorResourceID = t1.VendorResourceID,
                              Grade = t1.Grade,
                              UnitFormStatusID = t1.StatusID,
                              FormID = t1.FormID,
                              FormName = form.Name,
                              ActionByPE = PEUnitFormAction.UpdateBy,
                              Actiondate = PEUnitFormAction.ActionDate,
                              ActiondatePm = PMUnitFormAction.ActionDate,
                              ActiondatePJm = PJMUnitFormAction.ActionDate,
                              PM_StatusID = PMUnitFormAction.StatusID,
                              PM_Remarkaction = PMUnitFormAction.Remark,
                              PM_Actiontype = PMUnitFormAction.ActionType,
                              PM_ActionBy = PMUserAction.FirstName + " " + PMUserAction.LastName,
                              PJM_StatusID = PJMUnitFormAction.StatusID,
                              PJM_Remarkaction = PJMUnitFormAction.Remark,
                              PJM_Actiontype = PJMUnitFormAction.ActionType,

                              PM_getListgroup = (from fg in _context.tm_FormGroup
                                                 join t7 in _context.tr_UnitFormPassCondition on new { UnitFormID = (Guid?)t1.ID, GroupID = (int?)fg.ID } equals new { t7.UnitFormID, t7.GroupID } into unitFormPassConditions
                                                 from passCondition in unitFormPassConditions.DefaultIfEmpty()
                                                 where fg.FormID == t1.FormID
                                                 select new PM_getListgroup
                                                 {
                                                     Group_ID = fg.ID,
                                                     Group_Name = fg.Name,
                                                     PassConditionsID = passCondition.ID,
                                                     PC_StatusID = passCondition.StatusID,
                                                     LockStatusID = passCondition.LockStatusID,
                                                     PE_RemarkPC = passCondition.PE_Remark,
                                                     PM_RemarkPC = passCondition.PM_Remark,
                                                     PJM_RemarkPC = passCondition.PJM_Remark,
                                                     PCFlageActive = passCondition.FlagActive,
                                                     PM_getListpackage = (from tpk in _context.tr_UnitFormPackage
                                                                          join ftpk in _context.tm_FormPackage on new { tpk.GroupID, tpk.PackageID } equals new { ftpk.GroupID, PackageID = (int?)ftpk.ID } into FormPackages
                                                                          from FormPackage in FormPackages.DefaultIfEmpty()
                                                                          where tpk.UnitFormID == PEUnitFormAction.UnitFormID && tpk.FormID == t1.FormID && tpk.GroupID == fg.ID
                                                                          select new PM_getListpackage
                                                                          {
                                                                              Package_ID = tpk.PackageID,
                                                                              Package_Name = FormPackage.Name,
                                                                              Package_Remark = tpk.Remark
                                                                          }).ToList(),
                                                 }).ToList(),
                              PM_getListImage = (from img in _context.tr_UnitFormResource
                                                 join res in _context.tm_Resource on img.ResourceID equals res.ID
                                                 where img.UnitFormID == t1.ID && img.RoleID == 2 && img.FormID == model.FormID
                                                 select new PM_getListImage
                                                 {
                                                     ResourceID = res.ID,
                                                     FileName = res.FileName,
                                                     FilePath = res.FilePath
                                                 }).ToList(),
                              PCAllcount = _context.tr_UnitFormPassCondition.Count(x => x.UnitFormID == t1.ID)


                          }).FirstOrDefault();

             return result;
        }

        public List<UnitFormResourceModel> GetImage(UnitFormResourceModel model)
        {
            var result = (from t1 in _context.tr_UnitFormResource
                          join t2 in _context.tm_Resource on t1.ResourceID equals t2.ID into resources
                          from resource in resources.DefaultIfEmpty()
                          where t1.UnitFormID == model.UnitFormID
                                && (model.RoleID > 1 ? t1.FormID == model.FormID : t1.GroupID == model.GroupID)
                                && t1.RoleID == model.RoleID
                          select new UnitFormResourceModel
                          {
                              UnitFormResourceID = t1.ID,
                              ResourceID = t1.ResourceID,
                              MasterResourceID = resource.ID,
                              FileName = resource.FileName,
                              FilePath = resource.FilePath
                          }).ToList();

            return result;
        }

        public void SaveOrUpdateUnitFormAction(ApproveFormcheckIUDModel model)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(3)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    var unitFormAction = _context.tr_UnitFormAction.FirstOrDefault(a => a.UnitFormID == model.UnitFormID && a.RoleID == 2);

                    if (unitFormAction == null)
                    {
                        unitFormAction = new tr_UnitFormAction
                        {
                            UnitFormID = model.UnitFormID,
                            RoleID = 2,
                            ActionType = model.ActionType,
                            StatusID = model.UnitFormStatus,
                            Remark = string.IsNullOrEmpty(model.Remark) ? "" : FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now) + " : " + model.Remark ,
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
                        unitFormAction.StatusID = model.UnitFormStatus;
                        if (!string.IsNullOrEmpty(model.Remark))
                        {
                            if (unitFormAction.Remark != model.Remark)
                            {
                                unitFormAction.Remark = FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now) + " : " + model.Remark;
                            }
                        }
                        else
                        {
                            unitFormAction.Remark = "";
                        }
                        unitFormAction.ActionDate = DateTime.Now;
                        unitFormAction.UpdateBy = model.UserID;
                        unitFormAction.UpdateDate = DateTime.Now;

                        _context.tr_UnitFormAction.Update(unitFormAction);
                    }


                    _context.SaveChanges();

                    UpdateUnitForm(model.UnitFormID, model.ActionType, model.UnitFormStatus, model.UserID);

                    InsertUnitFormActionLog(unitFormAction, model.UserID);

                    if (model.PassConditionsIUD != null && model.PassConditionsIUD.Count > 0)
                    {
                        foreach (var passConditionModel in model.PassConditionsIUD)
                        {
                            var passCondition = _context.tr_UnitFormPassCondition
                                .FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.GroupID == passConditionModel.Group_ID && pc.FlagActive == true);

                            if (passCondition != null)
                            {
                                if (passCondition.StatusID != 8)
                                {
                                    passCondition.StatusID = passConditionModel.PassConditionsvalue;
                                    if (!string.IsNullOrEmpty(passConditionModel.Remark))
                                    {
                                        if (passCondition.PM_Remark != passConditionModel.Remark)
                                        {
                                            passCondition.PM_Remark = FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now) + " : " + passConditionModel.Remark ;
                                        }
                                    }
                                    else
                                    {
                                        passCondition.PM_Remark = "";
                                    }
                                    passCondition.UpdateBy = model.UserID;
                                    passCondition.UpdateDate = DateTime.Now;
                                    _context.tr_UnitFormPassCondition.Update(passCondition);
                                    InsertUnitFormActionLogPassCondition(passCondition, model.UserID);
                                }
                            }
                            _context.SaveChanges();
                        }
                    }

                    InsertImagesPM(model, null, 2);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("บันทึกลงฐานข้อมูลไม่สำเร็จ", ex);
                }
            }
        }

        private void UpdateUnitForm(Guid? unitformID, string? actiontype, int? StatusID ,Guid? userID)
        {
            var UnitForm = _context.tr_UnitForm
                .FirstOrDefault(tr => tr.ID == unitformID);

            if (UnitForm != null)  // Ensure UnitForm is found
            {
                UnitForm.StatusID = actiontype == "save" ? 3 : StatusID;
                UnitForm.UpdateBy = userID;
                UnitForm.UpdateDate = DateTime.Now;

                _context.tr_UnitForm.Update(UnitForm);
                _context.SaveChanges();
            }
        }

        private void InsertUnitFormActionLogPassCondition(tr_UnitFormPassCondition UnitFormPassCondition ,Guid? UserID)
        {
            var pcvalue = UnitFormPassCondition.StatusID == 6 ? "ให้ผ่านเพื่อส่ง PJM Head" : "ให้ไม่ผ่าน";

            var actionLog = new tr_UnitFormActionLog
            {
                UnitFormID = UnitFormPassCondition.UnitFormID,
                GroupID = UnitFormPassCondition.GroupID,
                RoleID = 2,
                StatusID = UnitFormPassCondition.StatusID,
                Remark = "PM " + pcvalue + " UnitFormPassCondition",
                ActionDate = DateTime.Now,
                CraeteDate = DateTime.Now,
                CreateBy = UserID
            };

            _context.tr_UnitFormActionLog.Add(actionLog);
            _context.SaveChanges();
        }

        private void InsertUnitFormActionLog(tr_UnitFormAction unitFormAction , Guid? userID)
        {
            var actionLog = new tr_UnitFormActionLog
            {
                UnitFormID = unitFormAction.UnitFormID,
                RoleID = unitFormAction.RoleID,
                StatusID = unitFormAction.StatusID,
                Remark = "PM " + unitFormAction.ActionType + " UnitFormAction",
                ActionDate = unitFormAction.ActionDate,
                CraeteDate = DateTime.Now,
                CreateBy = userID
            };

            _context.tr_UnitFormActionLog.Add(actionLog);
            _context.SaveChanges();
        }

        private void InsertImagesPM(ApproveFormcheckIUDModel model, Guid? userID, int RoleID)
        {
            if (model.Images != null && model.Images.Count > 0)
            {
                var folder = DateTime.Now.ToString("yyyyMM");
                var dirPath = Path.Combine(model.ApplicationPath, "wwwroot", "Upload", "document", folder, "PMImage");
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
                        string relativeFilePath = Path.Combine("Upload", "document", folder, "PMImage", fileName).Replace("\\", "/");

                        // Save the image details in the tm_Resource table
                        var newResource = new tm_Resource
                        {
                            ID = Guid.NewGuid(),
                            FileName = fileName,
                            FilePath = relativeFilePath, // Store the relative path with forward slashes
                            MimeType = "image/jpeg", // Ensure the MimeType is set to "image/jpeg"
                            FlagActive = true,
                            CreateBy = model.UserID,
                            CreateDate = DateTime.Now,
                            UpdateBy = model.UserID,    
                            UpdateDate = DateTime.Now,
                        };
                        _context.tm_Resource.Add(newResource);

                        // Link this resource to the pass condition, if necessary
                        var newFormResource = new tr_UnitFormResource
                        {
                            FormID = model.FormID,
                            GroupID = model.Group_ID,
                            RoleID = RoleID,
                            UnitFormID = model.UnitFormID,
                            ResourceID = newResource.ID,
                            CreateBy = model.UserID,
                            CreateDate = DateTime.Now,
                        };
                        _context.tr_UnitFormResource.Add(newFormResource);
                    }
                }

                _context.SaveChanges();
            }
        }

    }
}
