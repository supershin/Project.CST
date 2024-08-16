using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
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
                          join t4 in _context.tm_Project on t1.ProjectID equals t4.ProjectID into projects
                          from project in projects.DefaultIfEmpty()
                          join t5 in _context.tm_Unit on t1.UnitID equals t5.UnitID into units
                          from unit in units.DefaultIfEmpty()
                          join t6 in _context.tm_Form on t1.FormID equals t6.ID into forms
                          from form in forms.DefaultIfEmpty()
                          where actionPE.ActionType == "submit"
                          select new
                          {
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
                              t1.StatusID,
                              //actionPE.PassConditionID,
                              RoleID_PE = actionPE.RoleID,
                              ActionType_PE = actionPE.ActionType,
                              StatusID_PE = actionPE.StatusID,
                              Remark_PE = actionPE.Remark,
                              ActionDate_PE = actionPE.ActionDate,
                              RoleID_PM = actionPM.RoleID,
                              ActionType_PM = actionPM.ActionType,
                              StatusID_PM = actionPM.StatusID,
                              Remark_PM = actionPM.Remark,
                              ActionDate_PM = actionPM.ActionDate
                          })
                          .OrderBy(item => item.ActionType_PE == "submit" && item.ActionType_PM == null ? 0 : 1)
                          .ThenBy(item => item.UnitFormID)
                          .ToList();

            // Format the date after retrieving the data from the database
            var formattedResult = result.Select(item => new PMApproveModel
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
                //PassConditionID = item.PassConditionID,
                RoleID_PE = item.RoleID_PE,
                ActionType_PE = item.ActionType_PE,
                StatusID_PE = item.StatusID_PE,
                Remark_PE = item.Remark_PE,
                ActionDate_PE = item.ActionDate_PE.HasValue
                                ? FormatExtension.ToStringFrom_DD_MM_YYYY_To_DD_MM_YYYY(item.ActionDate_PE.Value.ToString("dd/MM/yyyy"))
                                : null,
                RoleID_PM = item.RoleID_PM,
                ActionType_PM = item.ActionType_PM,
                StatusID_PM = item.StatusID_PM,
                Remark_PM = item.Remark_PM,
                ActionDate_PM = item.ActionDate_PM
            }).ToList();

            return formattedResult;
        }

        public ApproveFormcheckModel GetApproveFormcheck(ApproveFormcheckModel model)
        {
            var result = (from t1 in _context.tr_UnitForm
                          join t2 in _context.tm_Vendor on t1.VendorID equals t2.ID into vendors
                          from vendor in vendors.DefaultIfEmpty()
                          join t4 in _context.tm_Project on t1.ProjectID equals t4.ProjectID into projects
                          from project in projects.DefaultIfEmpty()
                          join t5 in _context.tm_Unit on t1.UnitID equals t5.UnitID into units
                          from unit in units.DefaultIfEmpty()
                          join t8 in _context.tm_Form on t1.FormID equals t8.ID into forms
                          from form in forms.DefaultIfEmpty()
                          join t10 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)1 } equals new { t10.UnitFormID, t10.RoleID } into PEUnitFormActions
                          from PEUnitFormAction in PEUnitFormActions.DefaultIfEmpty()
                          join t11 in _context.tr_UnitFormAction on new { UnitFormID = (Guid?)t1.ID, RoleID = (int?)2 } equals new { t11.UnitFormID, t11.RoleID } into PMUnitFormActions
                          from PMUnitFormAction in PMUnitFormActions.DefaultIfEmpty()
                          where t1.UnitID == model.UnitID && t1.FormID == model.FormID
                          select new ApproveFormcheckModel
                          {
                              ID = t1.ID,
                              ProjectID = t1.ProjectID,
                              ProjectName = project.ProjectName,
                              UnitID = t1.UnitID,
                              UnitCode = unit.UnitCode,
                              VendorID = t1.VendorID,
                              VenderName = vendor.Name,
                              VendorResourceID = t1.VendorResourceID,
                              Grade = t1.Grade,
                              FormID = t1.FormID,
                              FormName = form.Name,
                              Actiondate = PEUnitFormAction.ActionDate,
                              ActiondatePm = PMUnitFormAction.ActionDate,
                              PM_StatusID = PMUnitFormAction.StatusID,
                              PM_Remarkaction = PMUnitFormAction.Remark,
                              PM_Actiontype = PMUnitFormAction.ActionType,
                              PM_getListgroup = (from fg in _context.tm_FormGroup
                                                 join t7 in _context.tr_UnitFormPassCondition on new { UnitFormID = (Guid?)t1.ID, GroupID = (int?)fg.ID , FlagActive = (bool?)true } equals new { t7.UnitFormID, t7.GroupID ,t7.FlagActive } into unitFormPassConditions
                                                 from passCondition in unitFormPassConditions.DefaultIfEmpty()
                                                 where fg.FormID == t1.FormID
                                                 select new PM_getListgroup
                                                 {
                                                     Group_ID = fg.ID,
                                                     Group_Name = fg.Name,
                                                     PassConditionsID = passCondition.ID,
                                                     PC_StatusID = passCondition.StatusID,
                                                     LockStatusID = passCondition.LockStatusID,
                                                     PE_Remark = passCondition.PE_Remark,
                                                     PM_Remark = passCondition.PM_Remark
                                                 }).ToList(),
                              PM_getListImage = (from img in _context.tr_UnitFormResource
                                                 join res in _context.tm_Resource on img.ResourceID equals res.ID
                                                 where img.UnitFormID == t1.ID && img.RoleID == 2 && img.FormID == model.FormID
                                                 select new PM_getListImage
                                                 {
                                                     ResourceID = res.ID,
                                                     FileName = res.FileName,
                                                     FilePath = res.FilePath
                                                 }).ToList()
                          }).FirstOrDefault();

            return result;
        }

        public List<UnitFormResourceModel> GetImage(UnitFormResourceModel model)
        {
            var result = (from t1 in _context.tr_UnitFormResource
                          join t2 in _context.tm_Resource on t1.ResourceID equals t2.ID into resources
                          from resource in resources.DefaultIfEmpty()
                          where t1.UnitFormID == model.UnitFormID && t1.GroupID == model.GroupID && t1.RoleID == model.RoleID
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
            // Handle UnitFormAction for the RoleID = 2 (PM)
            var unitFormAction = _context.tr_UnitFormAction.FirstOrDefault(a => a.UnitFormID == model.UnitFormID && a.RoleID == 2);

            if (unitFormAction == null)
            {
                // Insert a new UnitFormAction record
                unitFormAction = new tr_UnitFormAction
                {
                    UnitFormID = model.UnitFormID,
                    RoleID = 2,
                    ActionType = model.ActionType,
                    StatusID = model.UnitFormStatus,
                    Remark = model.Remark,
                    ActionDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CraeteDate = DateTime.Now
                };

                _context.tr_UnitFormAction.Add(unitFormAction);
            }
            else
            {
                // Update the existing UnitFormAction record
                unitFormAction.ActionType = model.ActionType;
                unitFormAction.StatusID = model.UnitFormStatus;
                unitFormAction.Remark = model.Remark;
                unitFormAction.ActionDate = DateTime.Now;
                unitFormAction.UpdateDate = DateTime.Now;

                _context.tr_UnitFormAction.Update(unitFormAction);
            }

            // Save changes to UnitFormAction
            _context.SaveChanges();

            //if (model.ActionType == "submit" && model.UnitFormStatus == 5)
            //{
            //    UpdateUnitFormActionTypePE(model);
            //}

            bool PC = model.PassConditionsIUD != null && model.PassConditionsIUD.Count > 0;

            // UpdateUnitForm action
            UpdateUnitForm(model.UnitFormID, model.ActionType, model.UnitFormStatus);

            // Log the action
            InsertUnitFormActionLog(unitFormAction);

            // Handle PassConditions if they exist
            if (model.PassConditionsIUD != null && model.PassConditionsIUD.Count > 0)
            {
                foreach (var passConditionModel in model.PassConditionsIUD)
                {
                    var passCondition = _context.tr_UnitFormPassCondition
                        .FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.GroupID == passConditionModel.Group_ID);

                    if (passCondition == null)
                    {
                        // Insert a new PassCondition record
                        passCondition = new tr_UnitFormPassCondition
                        {
                            UnitFormID = model.UnitFormID,
                            GroupID = passConditionModel.Group_ID,
                            StatusID = passConditionModel.PassConditionsvalue,
                            PM_Remark = passConditionModel.Remark,
                            ActionDate = DateTime.Now,
                            CraeteDate = DateTime.Now
                        };

                        _context.tr_UnitFormPassCondition.Add(passCondition);

                    }
                    else
                    {
                        // Update the existing PassCondition record
                        passCondition.StatusID = passConditionModel.PassConditionsvalue;
                        passCondition.PM_Remark = passConditionModel.Remark;
                        passCondition.UpdateDate = DateTime.Now;

                        _context.tr_UnitFormPassCondition.Update(passCondition);
                    }

                    
                    // Log the action
                    InsertUnitFormActionLogPassCondition(passCondition);

                    // Save changes for each PassCondition
                    _context.SaveChanges();
                }
            }

            // Save iamge 
            InsertImagesPM(model, null, 2); // RoleID = 2 for PM
        }

        private void UpdateUnitFormActionTypePE(ApproveFormcheckIUDModel model)
        {
            var unitFormAction = _context.tr_UnitFormAction.FirstOrDefault(a => a.UnitFormID == model.UnitFormID && a.RoleID == 1);
            if (unitFormAction != null)
            {
                unitFormAction.StatusID = null;
                unitFormAction.UpdateDate = DateTime.Now;
                _context.tr_UnitFormAction.Update(unitFormAction);
                _context.SaveChanges();
            }
        }

        private void UpdateUnitForm(Guid? unitformID, string? actiontype, int? StatusID)
        {
            var UnitForm = _context.tr_UnitForm
                .FirstOrDefault(tr => tr.ID == unitformID);

            if (UnitForm != null)  // Updated this condition to ensure it only proceeds if UnitForm is found
            {
                int answer;

                if (actiontype == "save")
                {
                    answer = 3;
                }
                else if (actiontype == "submit")
                {
                    if (StatusID == 5)
                    {
                        answer = 5;
                    }
                    else if (StatusID == 4)
                    {
                        answer = 4;
                    }
                    else
                    {
                        answer = -1; 
                    }
                }
                else
                {
                    answer = -1; 
                }

                if (answer != -1) 
                {
                    UnitForm.StatusID = answer;
                    UnitForm.UpdateDate = DateTime.Now;

                    _context.tr_UnitForm.Update(UnitForm);
                    _context.SaveChanges(); 
                }
            }
        }

        private void InsertUnitFormActionLogPassCondition(tr_UnitFormPassCondition UnitFormPassCondition)
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
                // CreateBy = unitFormAction.CreateBy, // Uncomment and set appropriately if you have the CreateBy field
            };

            _context.tr_UnitFormActionLog.Add(actionLog);
            _context.SaveChanges();
        }

        private void InsertUnitFormActionLog(tr_UnitFormAction unitFormAction)
        {
            var actionLog = new tr_UnitFormActionLog
            {
                UnitFormID = unitFormAction.UnitFormID,
                RoleID = unitFormAction.RoleID,
                StatusID = unitFormAction.StatusID,
                Remark = "PM " + unitFormAction.ActionType + " UnitFormAction",
                ActionDate = unitFormAction.ActionDate,
                CraeteDate = DateTime.Now,
                // CreateBy = unitFormAction.CreateBy, // Uncomment and set appropriately if you have the CreateBy field
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
                            CreateDate = DateTime.Now,
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
