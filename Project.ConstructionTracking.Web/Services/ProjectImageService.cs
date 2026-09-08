using Project.ConstructionTracking.Web.Models.ProjectImage;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
    public class ProjectImageService : IProjectImageService
    {
        private readonly IProjectImageRepo _ProjectImageRepo;

        private static readonly string[] AllowExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public ProjectImageService(IProjectImageRepo ProjectImageRepo)
        {
            _ProjectImageRepo = ProjectImageRepo;
        }

        public List<ProjectImageModel.ListProjectImageModel> GetProjectImageList(string? strSearch)
        {
            return _ProjectImageRepo.GetProjectImageList(strSearch);
        }

        public string? GetProjectImagePath(Guid projectID)
        {
            return _ProjectImageRepo.GetProjectImagePath(projectID);
        }

        public void SaveProjectImage(ProjectImageModel.SaveProjectImageModel model)
        {
            if (model == null || model.ProjectID == Guid.Empty)
            {
                throw new ArgumentException("ไม่พบข้อมูลโครงการ");
            }

            if (model.Image == null || model.Image.Length == 0)
            {
                throw new ArgumentException("กรุณาเลือกรูปภาพ");
            }

            var extension = Path.GetExtension(model.Image.FileName).ToLower();
            if (!AllowExtensions.Contains(extension))
            {
                throw new ArgumentException("รองรับเฉพาะไฟล์รูปภาพนามสกุล .jpg .jpeg .png และ .webp");
            }

            if (model.Image.Length > MaxFileSize)
            {
                throw new ArgumentException("ขนาดไฟล์ต้องไม่เกิน 5 MB");
            }

            try
            {
                _ProjectImageRepo.SaveProjectImage(model);
            }
            catch (Exception ex)
            {
                throw new Exception("เกิดข้อผิดพลาดขณะบันทึกรูปภาพโครงการ", ex);
            }
        }

        public void RemoveProjectImage(ProjectImageModel.RemoveProjectImageModel model)
        {
            if (model == null || model.ProjectID == Guid.Empty)
            {
                throw new ArgumentException("ไม่พบข้อมูลโครงการ");
            }

            try
            {
                _ProjectImageRepo.RemoveProjectImage(model);
            }
            catch (Exception ex)
            {
                throw new Exception("เกิดข้อผิดพลาดขณะลบรูปภาพโครงการ", ex);
            }
        }
    }
}
