using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json.Linq;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
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
                          where t1.UnitFormID == filterData.UnitFormID
                              && (filterData.GroupID == -1 || t1.GroupID == filterData.GroupID)
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
                              StatusID = t1.StatusID,
                              PE_Remark = t1.PE_Remark,
                              PM_Remark = t1.PM_Remark,
                              PJM_Remark = t1.PJM_Remark,
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
        public void RequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model)
        {


            if (model.RoleID == 1)
            {
                PERequestUnlock(model);
            }
            else {
                PMRequestUnlock(model);
            }

            _context.SaveChanges();
        }

        public void PERequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model)
        {
            var passCondition = _context.tr_UnitFormPassCondition.FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.ID == model.PC_ID && pc.FlagActive == true);

            if (passCondition != null)
            {
                passCondition.LockStatusID = 8;
                passCondition.StatusID = 12;
                passCondition.PEUnLock_Remark = model.PEUnLock_Remark;
                passCondition.UpdateBy = model.UserID;
                passCondition.UpdateDate = DateTime.Now;
                _context.tr_UnitFormPassCondition.Update(passCondition);
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


            _context.SaveChanges();
        }

        public void PMRequestUnlock(UnLockPassConditionModel.UpdateUnlockPC model)
        {
            var passCondition = _context.tr_UnitFormPassCondition.FirstOrDefault(pc => pc.UnitFormID == model.UnitFormID && pc.ID == model.PC_ID && pc.FlagActive == true);

            if (passCondition != null)
            {
                passCondition.StatusID = model.Action == "Approved" ? 13 : 14;
                passCondition.PMUnLock_Remark = model.PMUnLock_Remark;
                passCondition.UpdateBy = model.UserID;
                passCondition.UpdateDate = DateTime.Now;
                _context.tr_UnitFormPassCondition.Update(passCondition);
            }

            var actionLog = new tr_UnitFormActionLog
            {
                UnitFormID = model.UnitFormID,
                GroupID = model.GroupID,
                RoleID = model.RoleID,
                Remark = "PM/"+ model.Action + "/Unlock/UnitFormPassCondition",
                ActionDate = DateTime.Now,
                CraeteDate = DateTime.Now,
                CreateBy = model.UserID
            };
            _context.tr_UnitFormActionLog.Add(actionLog);
            _context.SaveChanges();
        }

    }
}
