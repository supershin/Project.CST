using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.ProjectBluePrint;
using QuestPDF.Infrastructure;
using static Project.ConstructionTracking.Web.Models.ChatInBoxModel;
using static Project.ConstructionTracking.Web.Models.ProjectBluePrint.ProjectBluePrintModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class ProjectBluePrintRepo : IProjectBluePrintRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public ProjectBluePrintRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<ProjectBluePrintModel.BlueprintElementModel> GetListProjectBlueprintElements(Guid ProjectID)
        {
            var query = (from blueprint in _context.tr_ProjectBluePrint
                        join ext in _context.tm_Ext on blueprint.ElementType equals ext.ID into extJoin
                        from ext in extJoin.DefaultIfEmpty()
                        join unit in _context.tm_Unit on blueprint.UnitID equals unit.UnitID into unitJoin
                        from unit in unitJoin.DefaultIfEmpty()
                        where blueprint.ProjectFloorPlanID == ProjectID
                        select new BlueprintElementModel
                        {
                            ElementTypeName = ext.Name,
                            Coordinates = JsonConvert.DeserializeObject<List<PointModel>>(blueprint.Coordinates),
                            UnitName = unit.UnitCode
                        }).ToList();

            return query.ToList();
        }

        public void SaveBlueprintElements(List<ProjectBluePrintModel.BlueprintElementModel> elements)
        {
            if (elements == null || !elements.Any())
            {
                return;
            }

            foreach (var element in elements)
            {
                var insertProjectBluePrint = new tr_ProjectBluePrint
                {
                    ID = Guid.NewGuid(),
                    ProjectFloorPlanID = element.ProjectID,
                    ElementType = element.ElementType,
                    Coordinates = JsonConvert.SerializeObject(element.Coordinates),
                    UnitID = element.UnitID,
                    CreatedBy = element.UserID,
                    CreatedDate = DateTime.Now,
                    UpdateBy = element.UserID,
                    UpdateDate = DateTime.Now
                };

                _context.tr_ProjectBluePrint.Add(insertProjectBluePrint);
            }

            _context.SaveChanges();
        }

        public void InsertImageProjectFloorPlan(ProjectBluePrintModel.InsertImageProjectFloorPlanModel model)
        {
            if (model.Images != null && model.Images.Count > 0)
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

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                        }

                        string relativeFilePath = Path.Combine("Upload", "document", folder, "ImageProjectFloorPlan", fileName).Replace("\\", "/");

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

                        var newProjectFloorPlanResource = new tr_ProjectFloorPlan
                        {
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

                _context.SaveChanges();
            }
        }

        public List<ProjectBluePrintModel.GetListImageProjectFloorPlanModel> GetListImageProjectFloorPlan(Guid ProjectID)
        {

            var query = from floorPlan in _context.tr_ProjectFloorPlan
                        join resource in _context.tm_Resource on floorPlan.ResourceID equals resource.ID into resourceJoin
                        from resource in resourceJoin.DefaultIfEmpty()
                        where floorPlan.ProjectID == ProjectID && resource.FlagActive == true
                        select new GetListImageProjectFloorPlanModel
                        {
                            ResourceID = floorPlan.ResourceID,
                            FileName = resource != null ? resource.FileName : "",
                            FilePath = resource != null ? resource.FilePath : ""
                        };

            var results = query.ToList();


            return results;
        }
    }
}
