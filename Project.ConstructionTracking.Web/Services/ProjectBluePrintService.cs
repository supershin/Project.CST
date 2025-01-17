using Project.ConstructionTracking.Web.Models.ProjectBluePrint;
using Project.ConstructionTracking.Web.Repositories;
using System.Transactions;
using static Project.ConstructionTracking.Web.Models.PJMApproveModel;

namespace Project.ConstructionTracking.Web.Services
{
    public class ProjectBluePrintService : IProjectBluePrintService
    {
        private readonly IProjectBluePrintRepo _IProjectBluePrintRepo;

        public ProjectBluePrintService(IProjectBluePrintRepo IProjectBluePrintRepo)
        {
            _IProjectBluePrintRepo = IProjectBluePrintRepo;
        }

        public List<ProjectBluePrintModel.GetListImageProjectFloorPlanModel> GetListImageProjectFloorPlan(Guid ProjectID)
        {
            var ListImageProjectFloorPlan = _IProjectBluePrintRepo.GetListImageProjectFloorPlan(ProjectID);
            return ListImageProjectFloorPlan;
        }

        public List<ProjectBluePrintModel.BlueprintElementModel> GetListProjectBlueprintElements(Guid ProjectID)
        {
            var ListProjectBlueprintElements = _IProjectBluePrintRepo.GetListProjectBlueprintElements(ProjectID);
            return ListProjectBlueprintElements;
        }

        public void InsertImageProjectFloorPlan(ProjectBluePrintModel.InsertImageProjectFloorPlanModel mode)
        {
            if (mode == null)
            {
                throw new ArgumentException("ไม่พบข้อมูล");
            }
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(10) 
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    _IProjectBluePrintRepo.InsertImageProjectFloorPlan(mode);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("เกิดข้อผิดพลาดขณะบันทึกรูปาพแผนผังของโครการ", ex);
                }
            }
        }

        public void SaveBlueprintElements(List<ProjectBluePrintModel.BlueprintElementModel> elements)
        {
            if (elements == null || !elements.Any())
            {
                throw new ArgumentException("No blueprint elements to save.");
            }

            // Define transaction options with a timeout of 10 minutes
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted, // Ensures data consistency
                Timeout = TimeSpan.FromMinutes(10) // Timeout after 10 minutes
            };

            // Create a transaction scope
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    // Save the blueprint elements through the repository
                    _IProjectBluePrintRepo.SaveBlueprintElements(elements);

                    scope.Complete(); // Mark the transaction as successful
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as needed
                    throw new Exception("An error occurred while saving blueprint elements.", ex);
                }
            } // TransactionScope is disposed here
        }

    }
}
