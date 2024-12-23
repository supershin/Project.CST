using DocumentFormat.OpenXml.Spreadsheet;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;
using Project.ConstructionTracking.Web.Models.UnitFormPaymentModel;
using Project.ConstructionTracking.Web.Models.WebAPIRest;
using Project.ConstructionTracking.Web.Services;
using QuestPDF.Infrastructure;
using System.Transactions;
using static Project.ConstructionTracking.Web.Infras.Services.WebAPIRestService;
using static Project.ConstructionTracking.Web.Models.PJMApproveModel;
using static Project.ConstructionTracking.Web.Models.UnitFormPaymentModel.UnitFormPaymentModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class UnitFormPaymentRepo : IUnitFormPaymentRepo
    {

        private readonly ContructionTrackingDbContext _context;
        private readonly IGetDDLService _getDDLService;
        private readonly IGRVenderrportalService _VenderrportalService;

        public UnitFormPaymentRepo(ContructionTrackingDbContext context, IGetDDLService getDDLService , IGRVenderrportalService VenderrportalService)
        {
            _context = context;
            _getDDLService = getDDLService;
            _VenderrportalService = VenderrportalService;
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
                             VenderName = t6.Name,
                             PONo = t3.PONo
                         }).FirstOrDefault();

            return result;
        }


        public List<UnitFormPaymentModel.getListUnitFormGRPaymentTable> GetListUnitFormGRPaymentTable(UnitFormPaymentModel.getListUnitFormGRPaymentTable Model)
        {
            var query = from t1 in _context.tr_UnitFormPayment
                        join t2 in _context.tm_Project on t1.ProjectID equals t2.ProjectID into t2Join
                        from t2 in t2Join.DefaultIfEmpty()
                        join t3 in _context.tm_Unit on t1.UnitID equals t3.UnitID into t3Join
                        from t3 in t3Join.DefaultIfEmpty()
                        join t4 in _context.tm_User on t1.CreateBy equals t4.ID into t4Join
                        from t4 in t4Join.DefaultIfEmpty()
                        join t5 in _context.tm_Ext on new { t1.SyncStatusID, ExtTypeID = (int?)11 } equals new { SyncStatusID = (int?)t5.ID, t5.ExtTypeID } into t5Join
                        from t5 in t5Join.DefaultIfEmpty()
                        where t1.FlagActive == true
                              && t1.UnitFormID == Model.UnitFormID
                        orderby t1.CreateDate
                        select new getListUnitFormGRPaymentTable
                        {
                            ID = t1.ID,
                            ProjectID = t1.ProjectID,
                            ProjectName = t2 != null ? t2.ProjectName : null,
                            UnitID = t1.UnitID,
                            UnitCode = t3 != null ? t3.UnitCode : null,
                            UnitFormID = t1.UnitFormID,
                            GRNO = t1.GRNO,
                            PONO = t1.PONO,
                            Remark = t1.Remark,
                            PercentPayment = t1.PercentPayment,
                            SyncStatusID = t1.SyncStatusID,
                            SyncStatusName = t5 != null ? t5.Name : null,
                            SyncMessage = t1.SyncMessage,
                            UpdateDate = FormatExtension.FormatDateToDayMonthNameYearTime(t1.UpdateDate),
                            CreateBy = t4 != null ? t4.FirstName + " " + t4.LastName : null
                        };

            return query.ToList();
        }


        //public string InsertNewGRPayment(UnitFormPaymentModel.IUDGRPayment Model)
        //{
        //    string returnUrlDoc = string.Empty;

        //    var transactionOptions = new TransactionOptions
        //    {
        //        IsolationLevel = IsolationLevel.ReadCommitted,
        //        Timeout = TimeSpan.FromMinutes(5)
        //    };

        //    using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
        //    {
        //        try
        //        {
        //            // Generate a random number to decide SyncStatusID
        //            Random random = new Random();
        //            int syncStatusID = random.Next(0, 2) == 0 ? 31 : 32; // Randomly sets to 31 or 32
        //            string syncMessage = syncStatusID == 31 ? "ทำการ Sync สำเร็จ" : "ทำการ Sync ไม่สำเร็จ"; // Set SyncMessage based on SyncStatusID


        //            // Introduce delay based on SyncStatusID
        //            if (syncStatusID == 31)
        //            {
        //                System.Threading.Thread.Sleep(4000); // 4-second delay
        //            }
        //            else
        //            {
        //                System.Threading.Thread.Sleep(8000); // 8-second delay
        //            }

        //            var newGRPayment = new tr_UnitFormPayment
        //            {
        //                ID = Guid.NewGuid(),
        //                ProjectID = Model.ProjectID,
        //                UnitID = Model.UnitID,
        //                UnitFormID = Model.UnitFormID,
        //                GRNO = Model.GRNO,
        //                PONO = Model.PONO,
        //                Remark = Model.Remark,
        //                PercentPayment = Model.PercentPayment,
        //                //SyncStatusID = Model.SyncStatusID,
        //                //SyncMessage = Model.SyncMessage,
        //                SyncStatusID = syncStatusID, // Assign the randomly chosen value
        //                SyncMessage = syncMessage, // Assign the corresponding SyncMessage
        //                FlagActive = true,
        //                CreateBy = Model.UserID,
        //                CreateDate = DateTime.Now,
        //                UpdateBy = Model.UserID,
        //                UpdateDate = DateTime.Now,
        //            };
        //            _context.tr_UnitFormPayment.Add(newGRPayment);
        //            _context.SaveChanges();

        //            var Filters = new GetDDL { Act = "GetListUnitFormPayment", GuID = Model.UnitFormID };
        //            List<GetDDL> CheckPercentPayment = _getDDLService.GetDDLList(Filters);
        //            decimal totalValuedecimalSum = CheckPercentPayment?.Where(x => x.Valuedecimal.HasValue).Sum(x => x.Valuedecimal.Value) ?? 0;


        //            if (totalValuedecimalSum + Model.PercentPayment > 100)
        //            {
        //                returnUrlDoc = "บันทึกข้อมูลสำเร็จ";
        //            }
        //            else
        //            {
        //                returnUrlDoc = "บันทึกข้อมูลครบ100%";
        //            }

        //            scope.Complete();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("เกิดเหตุขัดข้องบันทึกไม่สำเร็จ", ex);
        //        }
        //    }

        //    return returnUrlDoc;
        //}

        public string InsertNewGRPayment(UnitFormPaymentModel.IUDGRPayment Model)
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

                    var queryfileData = from t1 in _context.tr_Document
                                join t2 in _context.tm_Resource
                                on t1.ResourceID equals t2.ID into t2Group
                                from t2 in t2Group.DefaultIfEmpty()
                                where t1.UnitFormID == Model.UnitFormID
                                select new
                                {
                                    UnitFormPDF = t2.FilePath
                                };


                    // 1) Call the GRVenderrportal API
                    //    Example: We assume 'UploadFileAsync' or a similar method 
                    //    returns a status code (200 = success) and message.
                    var request = new RequestPostModel.GRVenderrportal.Sends
                    {
                        grno = Model.GRNO,
                        pono = Model.PONO,
                        // If you have files to send, populate this collection:
                        // fileData = Model.Files or an empty list if no files, e.g.:
                        fileData = new List<IFormFile>()
                    };

                    var apiResponse = _VenderrportalService.UploadFileAsync(request).GetAwaiter().GetResult();

                    // 2) Decide syncStatusID (31=success, 32=fail) based on API response
                    int syncStatusID;
                    string syncMessage;

                    if (apiResponse.status == 200)
                    {
                        syncStatusID = 31; // success
                        syncMessage = "ทำการ Sync สำเร็จ";
                    }
                    else
                    {
                        syncStatusID = 32; // failure
                        syncMessage = "ทำการ Sync ไม่สำเร็จ";
                    }

                    // 3) Create and save a new tr_UnitFormPayment record
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
                        SyncStatusID = syncStatusID,
                        SyncMessage = syncMessage,
                        FlagActive = true,
                        CreateBy = Model.UserID,
                        CreateDate = DateTime.Now,
                        UpdateBy = Model.UserID,
                        UpdateDate = DateTime.Now,
                    };
                    _context.tr_UnitFormPayment.Add(newGRPayment);
                    _context.SaveChanges();

                    // 4) Check the total payment percentage
                    var Filters = new GetDDL
                    {
                        Act = "GetListUnitFormPayment",
                        GuID = Model.UnitFormID
                    };
                    List<GetDDL> CheckPercentPayment = _getDDLService.GetDDLList(Filters);
                    decimal totalValuedecimalSum = CheckPercentPayment?.Where(x => x.Valuedecimal.HasValue).Sum(x => x.Valuedecimal.Value) ?? 0;

                    // 5) Decide return message based on total percentage
                    if (totalValuedecimalSum + Model.PercentPayment > 100)
                    {
                        returnUrlDoc = "บันทึกข้อมูลสำเร็จ";
                    }
                    else
                    {
                        returnUrlDoc = "บันทึกข้อมูลครบ100%";
                    }

                    // Commit the transaction
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("เกิดเหตุขัดข้องบันทึกไม่สำเร็จ", ex);
                }
            }

            return returnUrlDoc;
        }


        public string RemoveGRPayment(UnitFormPaymentModel.IUDGRPayment Model)
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
                    var tbUnitFormPayment = _context.tr_UnitFormPayment.FirstOrDefault(d => d.ID == Model.ID);

                    if (tbUnitFormPayment != null)
                    {
                        tbUnitFormPayment.FlagActive = false;
                        tbUnitFormPayment.UpdateDate = DateTime.Now;
                        tbUnitFormPayment.UpdateBy = Model.UserID;
                        _context.tr_UnitFormPayment.Update(tbUnitFormPayment);
                    }               
                    _context.SaveChanges();

                    returnUrlDoc = "ลบข้อมูลสำเร็จ";

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("เกิดเหตุขัดข้องลบไม่สำเร็จ", ex);
                }
            }

            return returnUrlDoc;
        }


        public string SyncGRPayment(UnitFormPaymentModel.IUDGRPayment Model)
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
                    var tbUnitFormPayment = _context.tr_UnitFormPayment.FirstOrDefault(d => d.ID == Model.ID);

                    if (tbUnitFormPayment != null)
                    {
                        tbUnitFormPayment.SyncStatusID = 31;
                        tbUnitFormPayment.SyncMessage = "ทำการ Sync สำเร็จ";
                        tbUnitFormPayment.UpdateDate = DateTime.Now;
                        tbUnitFormPayment.UpdateBy = Model.UserID;
                        _context.tr_UnitFormPayment.Update(tbUnitFormPayment);
                    }
                    _context.SaveChanges();

                    returnUrlDoc = "Sync ข้อมูลสำเร็จ";

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("เกิดเหตุขัดข้อง Sync ไม่สำเร็จ", ex);
                }
            }

            return returnUrlDoc;
        }
    }
}
