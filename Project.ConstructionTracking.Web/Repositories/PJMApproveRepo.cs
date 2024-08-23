﻿using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using static Project.ConstructionTracking.Web.Models.PJMApproveModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class PJMApproveRepo : IPJMApproveRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public PJMApproveRepo(ContructionTrackingDbContext context)
        {
            _context = context;
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
                                where unitFormAction.ActionType == filterData.ActionType && t1.StatusID > filterData.StatusID
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
                          from t3 in passConditions.Where(pc => pc.FlagActive == true).DefaultIfEmpty()
                          join t4 in _context.tm_Project on t1.ProjectID equals t4.ProjectID into projects
                          from t4 in projects.DefaultIfEmpty()
                          join t5 in _context.tm_Unit on t1.UnitID equals t5.UnitID into units
                          from t5 in units.DefaultIfEmpty()
                          join t6 in _context.tm_Form on t1.FormID equals t6.ID into forms
                          from t6 in forms.DefaultIfEmpty()
                          join t8 in _context.tr_UnitFormAction.Where(a => a.RoleID == 3)
                              on t1.ID equals t8.UnitFormID into actions
                          from t8 in actions.DefaultIfEmpty()
                          where t1.ID == filterData.UnitFormID && t3.ID != null && t1.StatusID > 5
                          select new GetlistChecklistPC
                          {
                              UnitFormID = t1.ID,
                              ProjectID = t1.ProjectID,
                              ProjectName = t4.ProjectName,
                              UnitID = t1.UnitID,
                              UnitCode = t5.UnitCode, 
                              FormID = t1.FormID,
                              FormName = t6.Name,
                              Grade = t1.Grade,
                              GroupID = t2.ID,
                              GroupName = t2.Name,
                              PC_ID = t3.ID,
                              LockStatusID = t3.LockStatusID,
                              PC_StatusID = t3.StatusID,
                              PE_Remark = t3.PE_Remark,
                              PM_Remark = t3.PM_Remark,
                              PJM_Remark = t3.PJM_Remark,
                              PJM_Actiontype = t8.ActionType,
                              PJM_ActionDate = t8.ActionDate,
                              PJM_StatusID = t8.StatusID,   
                              PJMUnitFormRemark = t8.Remark
                          }).ToList();

            return result;
        }

        public void SaveOrUpdateUnitFormAction(PJMApproveModel.PJMApproveIU model)
        {

            // Determine the StatusID based on the ListPCIC
            int? statusToUpdate = model.ListPCIC != null && model.ListPCIC.Any(pc => pc.StatusID == 9) ? 9 : 8;

            var unitFormAction = _context.tr_UnitFormAction.FirstOrDefault(a => a.UnitFormID == model.UnitFormID && a.RoleID == 3);

            if (unitFormAction == null)
            {
                // Insert a new UnitFormAction record
                unitFormAction = new tr_UnitFormAction
                {
                    UnitFormID = model.UnitFormID,
                    RoleID = 3,
                    ActionType = model.ActionType,
                    StatusID = statusToUpdate, // Use the determined StatusID
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
                unitFormAction.StatusID = statusToUpdate; // Use the determined StatusID if not null
                unitFormAction.Remark = model.Remark;
                unitFormAction.ActionDate = DateTime.Now;
                unitFormAction.UpdateDate = DateTime.Now;

                _context.tr_UnitFormAction.Update(unitFormAction);
            }

            _context.SaveChanges();

            int? statusForm = (statusToUpdate == 8) ? 7 : 8;
            UpdateUnitForm(model.UnitFormID, model.ActionType, statusForm);

            if (model.ListPCIC != null && model.ListPCIC.Count > 0)
            {
                foreach (var passConditionModel in model.ListPCIC)
                {
                    var passCondition = _context.tr_UnitFormPassCondition
                        .FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.GroupID == passConditionModel.Group_ID & pc.ID == passConditionModel.PC_ID);

                    if (passCondition != null)
                    {
                        passCondition.StatusID = passConditionModel.StatusID;
                        passCondition.PJM_Remark = passConditionModel.PJM_Remark;
                        passCondition.UpdateDate = DateTime.Now;

                        _context.tr_UnitFormPassCondition.Update(passCondition);
                    }
                    // Save changes for each PassCondition
                    _context.SaveChanges();
                }
            }

            // Save iamge 
            InsertImagesPM(model, null, 3); // RoleID = 2 for PM
        }

        private void UpdateUnitForm(Guid? unitformID, string? actiontype, int? StatusID)
        {
            var UnitForm = _context.tr_UnitForm.FirstOrDefault(tr => tr.ID == unitformID);

            if (UnitForm != null) 
            {
                UnitForm.StatusID = actiontype == "save" ? 9 : StatusID;
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
                            CreateDate = DateTime.Now,
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
                        };
                        _context.tr_UnitFormResource.Add(newFormResource);
                    }
                }

                _context.SaveChanges();
            }
        }

    }
}

