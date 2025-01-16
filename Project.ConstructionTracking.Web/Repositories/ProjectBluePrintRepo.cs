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
    }
}
