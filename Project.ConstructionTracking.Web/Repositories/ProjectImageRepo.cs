using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models.ProjectImage;
using System.Transactions;
using static Project.ConstructionTracking.Web.Models.ProjectImage.ProjectImageModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class ProjectImageRepo : IProjectImageRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public ProjectImageRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<ProjectImageModel.ListProjectImageModel> GetProjectImageList(string? strSearch)
        {
            var query = from project in _context.tm_Project
                        join projectImage in _context.tr_ProjectImage.Where(p => p.FlagActive == true)
                            on project.ProjectID equals projectImage.ProjectID into projectImageJoin
                        from projectImage in projectImageJoin.DefaultIfEmpty()
                        join resource in _context.tm_Resource.Where(r => r.FlagActive == true)
                            on projectImage.ResourceID equals resource.ID into resourceJoin
                        from resource in resourceJoin.DefaultIfEmpty()
                        where project.FlagActive == true
                           && (string.IsNullOrEmpty(strSearch)
                               || project.ProjectName.Contains(strSearch)
                               || project.ProjectCode.Contains(strSearch))
                        orderby project.ProjectCode
                        select new ListProjectImageModel
                        {
                            ProjectID = project.ProjectID,
                            ProjectCode = project.ProjectCode,
                            ProjectName = project.ProjectName,
                            ProjectImageID = projectImage != null ? projectImage.ID : (Guid?)null,
                            ResourceID = projectImage != null ? projectImage.ResourceID : null,
                            FileName = resource != null ? resource.FileName : null,
                            FilePath = resource != null ? resource.FilePath : null,
                            UpdateDate = projectImage != null ? projectImage.UpdateDate : null
                        };

            return query.ToList();
        }

        public string? GetProjectImagePath(Guid projectID)
        {
            var filePath = (from projectImage in _context.tr_ProjectImage
                            join resource in _context.tm_Resource on projectImage.ResourceID equals resource.ID
                            where projectImage.ProjectID == projectID
                               && projectImage.FlagActive == true
                               && resource.FlagActive == true
                            select resource.FilePath).FirstOrDefault();

            return filePath;
        }

        public void SaveProjectImage(ProjectImageModel.SaveProjectImageModel model)
        {
            if (model.Image == null || model.Image.Length == 0)
            {
                return;
            }

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(5)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                var folder = DateTime.Now.ToString("yyyyMM");
                var dirPath = Path.Combine(model.ApplicationPath ?? "", "wwwroot", "Upload", "document", folder, "ProjectImage");

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                string extension = Path.GetExtension(model.Image.FileName).ToLower();
                string fileName = Guid.NewGuid() + extension;
                var filePath = Path.Combine(dirPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }

                string relativeFilePath = Path.Combine("Upload", "document", folder, "ProjectImage", fileName).Replace("\\", "/");

                // 1 โครงการมีรูป Active ได้ 1 รูป — ปิดรูปเดิมก่อนบันทึกรูปใหม่
                var oldProjectImages = _context.tr_ProjectImage
                    .Where(p => p.ProjectID == model.ProjectID && p.FlagActive == true)
                    .ToList();

                foreach (var oldProjectImage in oldProjectImages)
                {
                    oldProjectImage.FlagActive = false;
                    oldProjectImage.UpdateBy = model.UserID;
                    oldProjectImage.UpdateDate = DateTime.Now;

                    var oldResource = _context.tm_Resource.FirstOrDefault(r => r.ID == oldProjectImage.ResourceID);
                    if (oldResource != null)
                    {
                        oldResource.FlagActive = false;
                        oldResource.UpdateBy = model.UserID;
                        oldResource.UpdateDate = DateTime.Now;
                    }
                }

                var newResource = new tm_Resource
                {
                    ID = Guid.NewGuid(),
                    FileName = model.Image.FileName,
                    FilePath = relativeFilePath,
                    MimeType = model.Image.ContentType,
                    FlagActive = true,
                    CreateBy = model.UserID,
                    CreateDate = DateTime.Now,
                    UpdateBy = model.UserID,
                    UpdateDate = DateTime.Now,
                };
                _context.tm_Resource.Add(newResource);

                var newProjectImage = new tr_ProjectImage
                {
                    ID = Guid.NewGuid(),
                    ProjectID = model.ProjectID,
                    ResourceID = newResource.ID,
                    FlagActive = true,
                    CreateBy = model.UserID,
                    CreateDate = DateTime.Now,
                    UpdateBy = model.UserID,
                    UpdateDate = DateTime.Now,
                };
                _context.tr_ProjectImage.Add(newProjectImage);

                _context.SaveChanges();
                scope.Complete();
            }
        }

        public void RemoveProjectImage(ProjectImageModel.RemoveProjectImageModel model)
        {
            var projectImages = _context.tr_ProjectImage
                .Where(p => p.ProjectID == model.ProjectID && p.FlagActive == true)
                .ToList();

            if (!projectImages.Any())
            {
                return;
            }

            foreach (var projectImage in projectImages)
            {
                projectImage.FlagActive = false;
                projectImage.UpdateBy = model.UserID;
                projectImage.UpdateDate = DateTime.Now;

                var resource = _context.tm_Resource.FirstOrDefault(r => r.ID == projectImage.ResourceID);
                if (resource != null)
                {
                    resource.FlagActive = false;
                    resource.UpdateBy = model.UserID;
                    resource.UpdateDate = DateTime.Now;
                }
            }

            _context.SaveChanges();
        }
    }
}
