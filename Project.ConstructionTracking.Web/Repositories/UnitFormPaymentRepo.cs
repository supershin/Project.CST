using DocumentFormat.OpenXml.Spreadsheet;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;
using Project.ConstructionTracking.Web.Services;
using QuestPDF.Infrastructure;
using System.Transactions;
using static Project.ConstructionTracking.Web.Models.PJMApproveModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class UnitFormPaymentRepo : IUnitFormPaymentRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public UnitFormPaymentRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }
         

        public UnitFormPaymentModel.getUnitFormGRDetail getUnitFormGRDetail(UnitFormPaymentModel.getUnitFormGRDetail Model)
        {

            var result = (from t1 in _context.tr_UnitForm
                         join t2 in _context.tm_Project on t1.ProjectID equals t2.ProjectID into projectJoin
                         from t2 in projectJoin.DefaultIfEmpty() 
                         join t3 in _context.tm_Unit on t1.UnitID equals t3.UnitID into unitJoin
                         from t3 in unitJoin.DefaultIfEmpty() 
                         join t4 in _context.tm_Form on t1.FormID equals t4.ID into formJoin
                         from t4 in formJoin.DefaultIfEmpty()
                         join t5 in _context.tm_CompanyVendor on t1.CompanyVendorID equals t5.ID into companyVendorJoin
                         from t5 in companyVendorJoin.DefaultIfEmpty() 
                         join t6 in _context.tm_Vendor on t1.VendorID equals t6.ID into vendorJoin
                         from t6 in vendorJoin.DefaultIfEmpty()
                         where t1.ID == Model.UnitFormID
                         select new UnitFormPaymentModel.getUnitFormGRDetail
                         {
                             ProjectID = t1.ProjectID,
                             ProjectName = t2.ProjectName,
                             UnitID = t1.UnitID,
                             UnitCode = t3.UnitCode,
                             UnitFormID = t1.ID,
                             FormName = t4.Name,
                             CompanyVenderName = t5.Name,
                             VenderName = t6.Name
                         }).FirstOrDefault();

            return result;
        }


        public string InsertNewGRPayment(UnitFormPaymentModel.insertGRPayment Model)
        {
            string returnUrlDoc = string.Empty;

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(5)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    var newGRPayment = new tr_UnitFormPayment
                    {
                        ID = Guid.NewGuid(),
                        ProjectID = Model.ProjectID,
                        UnitID = Model.UnitID,
                        UnitFormID = Model.UnitFormID,
                        GRNO = Model.GRNO,
                        PONO = Model.PONO,
                        Remark = Model.Remark,
                        PercentPayment = Model.PercentPayment,
                        SyncStatusID = Model.SyncStatusID,
                        SyncMessage = Model.SyncMessage,
                        CreateBy = Model.UserID,
                        CreateDate = DateTime.Now,
                        UpdateBy = Model.UserID,
                        UpdateDate = DateTime.Now,
                    };
                    _context.tr_UnitFormPayment.Add(newGRPayment);
                    _context.SaveChanges();

                    returnUrlDoc = "บันทึกข้อมูลสำเร็จ";

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("เกิดเหตุขัดข้องบันทึกไม่สำเร็จ", ex);
                }
            }

            return returnUrlDoc;
        }
    }
}
