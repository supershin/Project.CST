using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json.Linq;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.SendMail;
using System.Text.RegularExpressions;
using System.Transactions;
using static Project.ConstructionTracking.Web.Models.ApproveFormcheckIUDModel;
using static Project.ConstructionTracking.Web.Models.UnLockPassConditionModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class UnLockPassConditionRepo : IUnLockPassConditionRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public UnLockPassConditionRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }
        public UnLockPassConditionModel.GetDataUnlockDetail GetListUnlockDetail(UnLockPassConditionModel.GetDataUnlockDetail filterData)
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

                          join t10 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)SystemConstant.UserRole.PE } equals new { t10.UnitFormID, t10.RoleID } into PEUnitFormActions
                          from PEUnitFormAction in PEUnitFormActions.DefaultIfEmpty()
                          join t10U in _context.tm_User on new { PEUnitFormAction.UpdateBy } equals new { UpdateBy = (Guid?)t10U.ID } into PEUserActions
                          from PEUserAction in PEUserActions.DefaultIfEmpty()


                          join t11 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)SystemConstant.UserRole.PM } equals new { t11.UnitFormID, t11.RoleID } into PMUnitFormActions
                          from PMUnitFormAction in PMUnitFormActions.DefaultIfEmpty()
                          join t11U in _context.tm_User on new { PMUnitFormAction.UpdateBy } equals new { UpdateBy = (Guid?)t11U.ID } into PMUserActions
                          from PMUserAction in PMUserActions.DefaultIfEmpty()


                          join t12 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)SystemConstant.UserRole.PJM } equals new { t12.UnitFormID, t12.RoleID } into PJMUnitFormActions
                          from PJMUnitFormAction in PJMUnitFormActions.DefaultIfEmpty()
                          join t12U in _context.tm_User on new { PJMUnitFormAction.UpdateBy } equals new { UpdateBy = (Guid?)t12U.ID } into PJMUserActions
                          from PJMUserAction in PJMUserActions.DefaultIfEmpty()


                          where t1.ID == filterData.UnitFormID
                          select new GetDataUnlockDetail
                          {
                              ProjectName = project.ProjectName,
                              UnitFormID = t1.ID,
                              UnitCode = unit.UnitCode,
                              VenderName = vendor.Name,
                              CompanyName = Company.Name,
                              FormName = form.Name,
                              PEName = PEUserAction.FirstName + ' ' + PEUserAction.LastName,
                              PMName = PMUserAction.FirstName + ' ' + PMUserAction.LastName,
                              PJMName = PJMUserAction.FirstName + ' ' + PJMUserAction.LastName

                          }).FirstOrDefault();

            return result;
        }

        public List<UnLockPassConditionModel.GetDataUnlockPC> GetListUnlockPC(UnLockPassConditionModel.GetDataUnlockPC filterData)
        {
            var result = (from t1 in _context.tr_UnitFormPassCondition
                          join t2 in _context.tr_UnitForm on t1.UnitFormID equals t2.ID into unitForms
                          from t2 in unitForms.DefaultIfEmpty()
                          join t3 in _context.tm_Project on t2.ProjectID equals t3.ProjectID into projects
                          from t3 in projects.DefaultIfEmpty()
                          join t4 in _context.tm_Unit on t2.UnitID equals t4.UnitID into units
                          from t4 in units.DefaultIfEmpty()
                          join t5 in _context.tm_Form on t2.FormID equals t5.ID into forms
                          from t5 in forms.DefaultIfEmpty()
                          join t6 in _context.tm_Vendor on t2.VendorID equals t6.ID into vendors
                          from t6 in vendors.DefaultIfEmpty()
                          join t7 in _context.tm_FormGroup on t1.GroupID equals t7.ID into groups
                          from t7 in groups.DefaultIfEmpty()
                          join t2Pe in _context.tr_UnitFormAction on new { t1.UnitFormID , RoleID = (int?)1 } equals new { t2Pe.UnitFormID, t2Pe.RoleID } into unitFormsPE
                          from t2pe in unitFormsPE.DefaultIfEmpty()
                          join t2Pm in _context.tr_UnitFormAction on new { t1.UnitFormID, RoleID = (int?)2 } equals new { t2Pm.UnitFormID, t2Pm.RoleID } into unitFormsPM
                          from t2pm in unitFormsPM.DefaultIfEmpty()
                          join t2PJm in _context.tr_UnitFormAction on new { t1.UnitFormID, RoleID = (int?)3 } equals new { t2PJm.UnitFormID, t2PJm.RoleID } into unitFormsPJM
                          from t2PJm in unitFormsPJM.DefaultIfEmpty()
                          where t1.UnitFormID == filterData.UnitFormID && t1.FlagActive == true && (filterData.GroupID == -1 || t1.GroupID == filterData.GroupID)
                              //&& (t1.StatusID == 8 || t1.StatusID == 12 || t1.StatusID == 13 || t1.StatusID == 14)
                          select new UnLockPassConditionModel.GetDataUnlockPC
                          {
                              PC_ID = t1.ID,
                              ProjectID = t3.ProjectID,
                              ProjectName = t3.ProjectName,
                              UnitID = t4.UnitID,
                              UnitCode = t4.UnitCode,
                              FormID = t2.FormID,
                              FormName = t5.Name,
                              VenderName = t6.Name,
                              UnitFormID = t1.UnitFormID,
                              GroupID = t1.GroupID,
                              GroupName = t7.Name,
                              LockStatusID = t1.LockStatusID,
                              UnitFormStatusID = t2.StatusID,
                              StatusID = t1.StatusID,
                              PE_Remark = t1.PE_Remark,
                              PE_ActionDate = t2pe.ActionDate.HasValue ? t2pe.ActionDate.Value.ToString("dd/MM/yyyy") : "",
                              PM_Remark = t1.PM_Remark,
                              PM_ActionDate = t2pm.ActionDate.HasValue ? t2pm.ActionDate.Value.ToString("dd/MM/yyyy") : "",
                              PJM_Remark = t1.PJM_Remark,
                              PJM_ActionDate = t2PJm.ActionDate.HasValue ? t2PJm.ActionDate.Value.ToString("dd/MM/yyyy") : "",
                              PEUnLock_Remark = t1.PEUnLock_Remark,
                              PMUnLock_Remark = t1.PMUnLock_Remark,
                              // Fetch images related to this item
                              listImageUnlock = (from t1Image in _context.tr_UnitFormResource
                                                 join t2Image in _context.tm_Resource on t1Image.ResourceID equals t2Image.ID into resources
                                                 from resource in resources.DefaultIfEmpty()
                                                 where t1Image.UnitFormID == t1.UnitFormID && t1Image.PassConditionID == t1.ID && t1Image.RoleID == 1
                                                 select new UnLockPassConditionModel.GetImageUnlock
                                                 {
                                                     UnitFormResourceID = t1Image.ID,
                                                     ResourceID = t1Image.ResourceID,
                                                     MasterResourceID = resource.ID,
                                                     FileName = resource.FileName,
                                                     FilePath = resource.FilePath
                                                 }).ToList()
                          }).ToList();

            return result;
        }

        public List<UnLockPassConditionModel.GetImageUnlock> GetImage(UnLockPassConditionModel.GetImageUnlock filterData)
        {
            var result = (from t1 in _context.tr_UnitFormResource
                          join t2 in _context.tm_Resource on t1.ResourceID equals t2.ID into resources
                          from resource in resources.DefaultIfEmpty()
                          where t1.UnitFormID == filterData.UnitFormID && t1.PassConditionID == filterData.PC_ID && t1.RoleID == filterData.RoleID
                          select new GetImageUnlock
                          {
                              UnitFormResourceID = t1.ID,
                              ResourceID = t1.ResourceID,
                              MasterResourceID = resource.ID,
                              FileName = resource.FileName,
                              FilePath = resource.FilePath
                          }).ToList();

            return result;
        }

        public List<PERequesUnlockModel> GetImage(int PC_ID , Guid UnitFormID)
        {
            var unitFormPassConditionData = (from t1 in _context.tr_UnitFormPassCondition
                                             join t2 in _context.tr_UnitFormUnLockPassCondition on new { t1.UnitFormID, PassConditionID = (int)t1.ID, RoleID = (int?)SystemConstant.UserRole.PE } equals new { t2.UnitFormID, t2.PassConditionID, t2.RoleID } into t2Group
                                             from t2 in t2Group.DefaultIfEmpty()
                                             join t3 in _context.tr_UnitForm on t1.UnitFormID equals t3.ID
                                             join t4 in _context.tr_ProjectPermission on t3.ProjectID equals t4.ProjectID
                                             join t5 in _context.tm_User on t4.UserID equals t5.ID
                                             join t6 in _context.tm_Form on t3.FormID equals t6.ID
                                             join t7 in _context.tm_FormGroup on t1.GroupID equals t7.ID
                                             join t8 in _context.tm_User on t2.UpdateBy equals t8.ID into t8Group
                                             from t8 in t8Group.DefaultIfEmpty()
                                             join t9 in _context.tm_Project on t3.ProjectID equals t9.ProjectID
                                             join t10 in _context.tm_Unit on t3.UnitID equals t10.UnitID
                                             where t1.UnitFormID == UnitFormID
                                                && t1.ID == PC_ID
                                                && t5.RoleID == SystemConstant.UserRole.PM
                                             select new PERequesUnlockModel
                                             {
                                                 PMFullname = (t5.FirstName ?? string.Empty) + " " + (t5.LastName ?? string.Empty),
                                                 FormName = (t6.Name ?? string.Empty) + " " + (t7.Name ?? string.Empty),
                                                 PEFullname = (t8.FirstName ?? string.Empty) + " " + (t8.LastName ?? string.Empty),
                                                 ActionDate = FormatExtension.FormatDateToDayMonthNameYearTime(t2.ActionDate),
                                                 ProjectName = t9.ProjectName ?? string.Empty,
                                                 UnitCode = t10.UnitCode ?? string.Empty,
                                                 PERemark = t2.Remark ?? string.Empty,
                                                 Email = t5.Email ?? string.Empty
                                             }).ToList();

            return unitFormPassConditionData;
        }

        public void RequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model)
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
                    if (model.RoleID == SystemConstant.UserRole.PE)
                    {
                        PERequestUnlock(model);
                    }
                    else
                    {
                        PMRequestUnlock(model);
                    }

                    _context.SaveChanges();

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("บันทึกลงฐานข้อมูลไม่สำเร็จ", ex);
                }
            }
        }

        public void PERequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model)
        {

            var passCondition = _context.tr_UnitFormPassCondition.FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.ID == model.PC_ID && pc.FlagActive == true);

            if (passCondition != null)
            {
                //passCondition.LockStatusID = 8;
                passCondition.StatusID = 12;

                // Check if model.PEUnLock_Remark has a value
                if (!string.IsNullOrEmpty(model.PEUnLock_Remark))
                {
                    if (passCondition.PEUnLock_Remark != model.PEUnLock_Remark)
                    {
                        passCondition.PEUnLock_Remark = model.PEUnLock_Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now);
                    }
                }
                else {
                    passCondition.PEUnLock_Remark = "";
                }

                passCondition.UpdateBy = model.UserID;
                passCondition.UpdateDate = DateTime.Now;
                _context.tr_UnitFormPassCondition.Update(passCondition);
            }

            var UnLockPassCondition = _context.tr_UnitFormUnLockPassCondition
               .FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.PassConditionID == model.PC_ID && pc.RoleID == SystemConstant.UserRole.PE && pc.FlagActive == true);

            if (UnLockPassCondition == null)
            {
                // Create a new instance if it doesn't exist
                UnLockPassCondition = new tr_UnitFormUnLockPassCondition
                {
                    UnitFormID = model.UnitFormID,
                    PassConditionID = model.PC_ID,
                    RoleID = SystemConstant.UserRole.PE,
                    StatusID = 12,
                    Remark = string.IsNullOrEmpty(model.PEUnLock_Remark)
                        ? ""
                        : model.PEUnLock_Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now),
                    FlagActive = true,
                    ActionDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    UpdateBy = model.UserID,
                    CreateBy = model.UserID,
                    CraeteDate = DateTime.Now
                };

                _context.tr_UnitFormUnLockPassCondition.Add(UnLockPassCondition);
            }
            else
            {
                // Update existing instance
                UnLockPassCondition.UnitFormID = model.UnitFormID;
                UnLockPassCondition.PassConditionID = model.PC_ID;
                UnLockPassCondition.RoleID = SystemConstant.UserRole.PE;
                UnLockPassCondition.StatusID = 12;

                if (!string.IsNullOrEmpty(model.PEUnLock_Remark))
                {
                    if (UnLockPassCondition.Remark != model.PEUnLock_Remark)
                    {
                        UnLockPassCondition.Remark = model.PEUnLock_Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now);
                    }
                }
                else
                {
                    UnLockPassCondition.Remark = "";
                }

                UnLockPassCondition.FlagActive = true;
                UnLockPassCondition.ActionDate = DateTime.Now;
                UnLockPassCondition.UpdateDate = DateTime.Now;
                UnLockPassCondition.UpdateBy = model.UserID;

                _context.tr_UnitFormUnLockPassCondition.Update(UnLockPassCondition);
            }

            var actionLog = new tr_UnitFormActionLog
            {
                UnitFormID = model.UnitFormID,
                GroupID = model.GroupID,
                RoleID = model.RoleID,
                //StatusID = UnitFormPassCondition.StatusID,
                Remark = "PE/RequestUnlock/UnitFormPassCondition",
                ActionDate = DateTime.Now,
                CraeteDate = DateTime.Now,
                CreateBy = model.UserID
            };
            _context.tr_UnitFormActionLog.Add(actionLog);

            if (model.Images != null && model.Images.Count > 0)
            {
                var folder = DateTime.Now.ToString("yyyyMM");
                var dirPath = Path.Combine(model.ApplicationPath, "wwwroot", "Upload", "document", folder, "RequestUnlock");
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
                        string relativeFilePath = Path.Combine("Upload", "document", folder, "RequestUnlock", fileName).Replace("\\", "/");

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
                            PassConditionID = model.PC_ID,
                            RoleID = model.RoleID,
                            UnitFormID = model.UnitFormID,
                            ResourceID = newResource.ID,
                            CreateBy = model.UserID,
                            CreateDate = DateTime.Now,
                        };
                        _context.tr_UnitFormResource.Add(newFormResource);
                    }
                }
            }

            var UnitForm = _context.tr_UnitForm.FirstOrDefault(tr => tr.ID == model.UnitFormID);
            if (UnitForm != null)
            {
                UnitForm.StatusID = SystemConstant.Unit_Form_Status.PE_REQ_Unlock;
                UnitForm.UpdateBy = model.UserID;
                UnitForm.UpdateDate = DateTime.Now;

                _context.tr_UnitForm.Update(UnitForm);
            }

            _context.SaveChanges();
        }

        public void PMRequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model)
        {

            var passCondition = _context.tr_UnitFormPassCondition.FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.ID == model.PC_ID && pc.FlagActive == true);

            if (passCondition != null)
            {
                passCondition.StatusID = model.Action == "Reject" ? 14 : 13;
                passCondition.LockStatusID = model.Action == "Reject" ? 7 : 8;
                passCondition.PMUnLock_Remark = !string.IsNullOrEmpty(model.PMUnLock_Remark)
                    ? model.PMUnLock_Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now)
                    : "";

                passCondition.UpdateBy = model.UserID;
                passCondition.UpdateDate = DateTime.Now;
                _context.tr_UnitFormPassCondition.Update(passCondition);

                _context.SaveChanges(); 

                if (model.Action != "Reject")
                {
                    bool allItemsHaveSameStatus = _context.tr_UnitFormPassCondition
                        .Where(t => t.UnitFormID == model.UnitFormID && t.FlagActive == true)
                        .All(t => t.StatusID == 13);

                    if (allItemsHaveSameStatus)
                    {
                        var unitForm = _context.tr_UnitForm.FirstOrDefault(tr => tr.ID == model.UnitFormID);
                        if (unitForm != null)
                        {
                            unitForm.StatusID = SystemConstant.Unit_Form_Status.PM_Approve_Unlock;
                            unitForm.UpdateBy = model.UserID;
                            unitForm.UpdateDate = DateTime.Now;

                            _context.tr_UnitForm.Update(unitForm);
                        }
                    }
                }
            }

            var UnLockPassCondition = _context.tr_UnitFormUnLockPassCondition
                .FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.PassConditionID == model.PC_ID && pc.RoleID == SystemConstant.UserRole.PM && pc.FlagActive == true);

            if (UnLockPassCondition == null)
            {
                // Create a new instance if it doesn't exist
                UnLockPassCondition = new tr_UnitFormUnLockPassCondition
                {
                    UnitFormID = model.UnitFormID,
                    PassConditionID = model.PC_ID,
                    RoleID = SystemConstant.UserRole.PM,
                    StatusID = model.Action == "Reject" ? 14 : 13,
                    Remark = string.IsNullOrEmpty(model.PMUnLock_Remark)
                        ? ""
                        : model.PMUnLock_Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now),
                    FlagActive = true,
                    ActionDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    UpdateBy = model.UserID,
                    CreateBy = model.UserID,
                    CraeteDate = DateTime.Now
                };

                _context.tr_UnitFormUnLockPassCondition.Add(UnLockPassCondition);
            }
            else
            {
                // Update existing instance
                UnLockPassCondition.UnitFormID = model.UnitFormID;
                UnLockPassCondition.PassConditionID = model.PC_ID;
                UnLockPassCondition.RoleID = SystemConstant.UserRole.PM;
                UnLockPassCondition.StatusID = model.Action == "Reject" ? 14 : 13; 

                if (!string.IsNullOrEmpty(model.PMUnLock_Remark))
                {
                    if (UnLockPassCondition.Remark != model.PMUnLock_Remark)
                    {
                        UnLockPassCondition.Remark = model.PMUnLock_Remark + ' ' + FormatExtension.FormatDateToDayMonthNameYearTime(DateTime.Now);
                    }
                }
                else
                {
                    UnLockPassCondition.Remark = "";
                }

                UnLockPassCondition.FlagActive = true;
                UnLockPassCondition.ActionDate = DateTime.Now;
                UnLockPassCondition.UpdateDate = DateTime.Now;
                UnLockPassCondition.UpdateBy = model.UserID;

                _context.tr_UnitFormUnLockPassCondition.Update(UnLockPassCondition);
            }

            // Add action log
            var actionLog = new tr_UnitFormActionLog
            {
                UnitFormID = model.UnitFormID,
                GroupID = model.GroupID,
                RoleID = model.RoleID,
                Remark = "PM/" + model.Action + "/Unlock/UnitFormPassCondition",
                ActionDate = DateTime.Now,
                CraeteDate = DateTime.Now,
                CreateBy = model.UserID
            };

            _context.tr_UnitFormActionLog.Add(actionLog);
        }

    }
}
