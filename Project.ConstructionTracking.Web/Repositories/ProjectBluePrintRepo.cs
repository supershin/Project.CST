using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.ProjectBluePrint;
using Project.ConstructionTracking.Web.Models.QC5CheckModel;
using QuestPDF.Infrastructure;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Project.ConstructionTracking.Web.Models.ChatInBoxModel;
using static Project.ConstructionTracking.Web.Models.ProjectBluePrint.ProjectBluePrintModel;
using static QuestPDF.Helpers.Colors;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class ProjectBluePrintRepo : IProjectBluePrintRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public ProjectBluePrintRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<ProjectBluePrintModel.BlueprintElementModel> GetListProjectBlueprintElements(Guid ProjectFloorPlanID)
        {
            var query = (from blueprint in _context.tr_ProjectBluePrint
                        join ext in _context.tm_Ext on blueprint.ElementType equals ext.ID into extJoin
                        from ext in extJoin.DefaultIfEmpty()
                        join unit in _context.tm_Unit on blueprint.UnitID equals unit.UnitID into unitJoin
                        from unit in unitJoin.DefaultIfEmpty()
                        where blueprint.ProjectFloorPlanID == ProjectFloorPlanID && blueprint.FlagActive == true
                         select new BlueprintElementModel
                        {
                            ElementTypeName = ext.Name,
                            Coordinates = JsonConvert.DeserializeObject<List<PointModel>>(blueprint.Coordinates),
                            UnitName = unit.UnitCode,
                            UnitID  = unit.UnitID

                         }).ToList();

            return query.ToList();
        }

        public void SaveBlueprintElements(List<ProjectBluePrintModel.BlueprintElementModel> elements)
        {
            if (elements == null || !elements.Any())
            {
                return;
            }

            // Define the transaction options with a 5-minute timeout
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(5)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                // Flag all existing records as inactive
                var projectBluePrints = _context.tr_ProjectBluePrint.Where(p => p.ProjectFloorPlanID == elements[0].ProjectFloorPlanID).ToList();
                if (projectBluePrints.Any())
                {
                    foreach (var projectBluePrint in projectBluePrints)
                    {
                        projectBluePrint.FlagActive = false;
                        _context.tr_ProjectBluePrint.Update(projectBluePrint);
                    }
                }


                foreach (var element in elements)
                {
                    // Check if a record exists in the database with the same UnitID
                    var existingRecord = _context.tr_ProjectBluePrint.FirstOrDefault(p => p.UnitID == element.UnitID);

                    if (existingRecord != null)
                    {
                        // Update the existing record
                        existingRecord.ProjectFloorPlanID = element.ProjectFloorPlanID;
                        existingRecord.ElementType = element.ElementType;
                        existingRecord.Coordinates = JsonConvert.SerializeObject(element.Coordinates);
                        existingRecord.FlagActive = true;
                        existingRecord.UpdateBy = element.UserID;
                        existingRecord.UpdateDate = DateTime.Now;
                        _context.tr_ProjectBluePrint.Update(existingRecord);
                    }
                    else
                    {
                        // Insert a new record
                        var insertProjectBluePrint = new tr_ProjectBluePrint
                        {
                            ID = Guid.NewGuid(),
                            ProjectFloorPlanID = element.ProjectFloorPlanID,
                            ElementType = element.ElementType,
                            Coordinates = JsonConvert.SerializeObject(element.Coordinates),
                            UnitID = element.UnitID,
                            FlagActive = true,
                            CreatedBy = element.UserID,
                            CreatedDate = DateTime.Now,
                            UpdateBy = element.UserID,
                            UpdateDate = DateTime.Now
                        };

                        _context.tr_ProjectBluePrint.Add(insertProjectBluePrint);
                    }
                }

                _context.SaveChanges(); // Save all changes within the transaction scope

                scope.Complete(); // Commit the transaction
            }
        }

        public void InsertImageProjectFloorPlan(ProjectBluePrintModel.InsertImageProjectFloorPlanModel model)
        {
            if (model.Images == null || model.Images.Count == 0)
            {
                return;
            }

            // Define the transaction options with a 5-minute timeout
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(5)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                var folder = DateTime.Now.ToString("yyyyMM");
                var dirPath = Path.Combine(model.ApplicationPath, "wwwroot", "Upload", "document", folder, "ImageProjectFloorPlan");

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                foreach (var image in model.Images)
                {
                    if (image.Length > 0)
                    {
                        Guid guidId = Guid.NewGuid();
                        string fileName = guidId + ".jpg";
                        var filePath = Path.Combine(dirPath, fileName);

                        // Save the image file to the file system
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                        }

                        string relativeFilePath = Path.Combine("Upload", "document", folder, "ImageProjectFloorPlan", fileName).Replace("\\", "/");

                        // Create a new resource entry
                        var newResource = new tm_Resource
                        {
                            ID = Guid.NewGuid(),
                            FileName = fileName,
                            FilePath = relativeFilePath,
                            MimeType = "image/jpeg",
                            FlagActive = true,
                            CreateBy = model.UserID,
                            CreateDate = DateTime.Now,
                            UpdateBy = model.UserID,
                            UpdateDate = DateTime.Now,
                        };
                        _context.tm_Resource.Add(newResource);

                        // Create a new project floor plan entry
                        var newProjectFloorPlanResource = new tr_ProjectFloorPlan
                        {
                            ID = Guid.NewGuid(),
                            ProjectID = model.ProjectID,
                            ResourceID = newResource.ID,
                            FlagActive = true,
                            CreateDate = DateTime.Now,
                            CreateBy = model.UserID,
                            UpdateBy = model.UserID,
                            UpdateDate = DateTime.Now,
                        };
                        _context.tr_ProjectFloorPlan.Add(newProjectFloorPlanResource);
                    }
                }

                // Save changes within the transaction
                _context.SaveChanges();
                scope.Complete(); // Commit the transaction
            }
        }

        public void RemoveImageProjectFloorPlan(ProjectBluePrintModel.RemoveImageProjectFloorPlanModel model)
        {
            var ProjectFloorPlan = _context.tr_ProjectFloorPlan.FirstOrDefault(p => p.ID == model.ProjectFloorPlanID);

            if (ProjectFloorPlan != null)
            {
                ProjectFloorPlan.FlagActive = false;
                ProjectFloorPlan.UpdateBy = model.UserID;
                ProjectFloorPlan.UpdateDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public List<ProjectBluePrintModel.GetListImageProjectFloorPlanModel> GetListImageProjectFloorPlan(Guid ProjectID)
        {

            var query = from floorPlan in _context.tr_ProjectFloorPlan
                        join resource in _context.tm_Resource on floorPlan.ResourceID equals resource.ID into resourceJoin
                        from resource in resourceJoin.DefaultIfEmpty()
                        where floorPlan.ProjectID == ProjectID && resource.FlagActive == true && floorPlan.FlagActive == true
                        select new GetListImageProjectFloorPlanModel
                        {
                            ProjectFloorPlanID = floorPlan.ID,
                            ResourceID = floorPlan.ResourceID,
                            FileName = resource != null ? resource.FileName : "",
                            FilePath = resource != null ? resource.FilePath : ""
                        };

            var results = query.ToList();


            return results;
        }
    }
}
