using Project.ConstructionTracking.Web.Models.ImportQC5Model;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IImportQC5Repo
    {
        void ValidateRows(List<ImportQC5RowModel> rows);
        ImportQC5CommitResult ImportRows(List<ImportQC5RowModel> rowsToImport, Guid userID);
    }
}
