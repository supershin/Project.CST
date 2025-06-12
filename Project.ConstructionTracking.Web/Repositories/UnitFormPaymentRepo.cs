using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
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
        private readonly IWebAPIRestService _VenderrportalService;

        public UnitFormPaymentRepo(ContructionTrackingDbContext context, IGetDDLService getDDLService , IWebAPIRestService VenderrportalService)
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


        public UnitPaymentMail getUnitFormSendmMailDetail(Guid UnitFormID)
        {

            var result = (from t1 in _context.tr_UnitForm
                          join t2 in _context.tm_Vendor on t1.VendorID equals t2.ID into vendorJoin
                          from t2 in vendorJoin.DefaultIfEmpty()
                          join t3 in _context.tr_CompanyVendor on t2.ID equals t3.VendorID into companyVendorJoin
                          from t3 in companyVendorJoin.DefaultIfEmpty()
                          join t4 in _context.tm_CompanyVendor on t3.CompanyVendorID equals t4.ID into companyVendorNameJoin
                          from t4 in companyVendorNameJoin.DefaultIfEmpty()
                          join t5 in _context.tm_Project on t1.ProjectID equals t5.ProjectID into projectJoin
                          from t5 in projectJoin.DefaultIfEmpty()
                          join t6 in _context.tm_Unit on t1.UnitID equals t6.UnitID into unitJoin
                          from t6 in unitJoin.DefaultIfEmpty()
                          where t1.ID == UnitFormID
                          select new UnitPaymentMail
                          {
                              //VendorFullName = t2.Name,
                              VendorFullName = t4.Name,
                              VendorEmail = t2.Email,
                              //VendorEmail = "siripoj@assetwise.co.th",
                              ProjectName = t5.ProjectName,
                              UnitCode = t6.UnitCode
                          }).FirstOrDefault();

            return result;
        }


        public int InsertNewGRPayment(UnitFormPaymentModel.IUDGRPayment Model)
        {
            int returnUrlDoc = 0;

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

                    var queryQCfileData = from t1 in _context.tr_UnitForm
                                          join rawT2 in _context.tr_Form_QCCheckList on t1.FormID equals rawT2.FormID into t2Join
                                          from t2 in t2Join.DefaultIfEmpty()
                                          join rawT3 in _context.tr_QC_UnitCheckList.Where(x => x.UnitID == Model.UnitID && x.QCStatusID == 1) on t2.CheckListID equals rawT3.CheckListID into t3Join
                                          from t3 in t3Join.DefaultIfEmpty()
                                          join rawT4 in _context.tr_Document on t3.ID equals rawT4.QCUnitCheckListID into t4Join
                                          from t4 in t4Join.DefaultIfEmpty()
                                          join rawT5 in _context.tm_Resource on t4.ResourceID equals rawT5.ID into t5Join
                                          from t5 in t5Join.DefaultIfEmpty()
                                          where t1.UnitID == Model.UnitID && t1.ID == Model.UnitFormID

                                          select new
                                          {
                                            UnitQCPDF = t5.FilePath
                                          };

                    var request = new RequestPostModel.GRVenderrportal.Sends
                    {
                        grno = Model.GRNO,
                        pono = Model.PONO,
                        remark = Model.Remark,
                        fileData = new List<IFormFile>()
                    };

                    // ----- Add file data from queryfileData -----
                    foreach (var item in queryfileData)
                    {
                        if (!string.IsNullOrEmpty(item.UnitFormPDF))
                        {
                            string fullPath = Path.Combine(Model.ApplicationPath, item.UnitFormPDF);

                            if (File.Exists(fullPath))
                            {
                                IFormFile file = FormatExtension.CreateFormFileFromPath(fullPath);
                                request.fileData.Add(file);
                            }
                        }
                    }

                    // ----- Add file data from queryQCfileData -----
                    foreach (var item in queryQCfileData)
                    {
                        if (!string.IsNullOrEmpty(item.UnitQCPDF))
                        {
                            string fullPath = Path.Combine(Model.ApplicationPath, item.UnitQCPDF);

                            if (File.Exists(fullPath))
                            {
                                IFormFile file = FormatExtension.CreateFormFileFromPath(fullPath);
                                request.fileData.Add(file);
                            }
                        }
                    }


                    // 1) Call the GRVenderrportal API
                    var apiResponse = _VenderrportalService.UploadFileAsync(request).GetAwaiter().GetResult();

                    // 2) Decide syncStatusID (31=success, 32=fail) based on API response
                    int syncStatusID;

                    if (apiResponse.status == 200)
                    {
                        syncStatusID = 31; // success
                    }                  
                    else
                    {
                        syncStatusID = 32; // failure
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
                        SyncMessage = apiResponse?.message,
                        FlagActive = true,
                        CreateBy = Model.UserID,
                        CreateDate = DateTime.Now,
                        UpdateBy = Model.UserID,
                        UpdateDate = DateTime.Now,
                    };
                    _context.tr_UnitFormPayment.Add(newGRPayment);
                    _context.SaveChanges();

                    returnUrlDoc = syncStatusID;

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


        public int SyncGRPayment(UnitFormPaymentModel.IUDGRPayment Model)
        {
            int returnUrlDoc = 0;

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
                        // Prepare request model for GRVenderrportal
                        var request = new RequestPostModel.GRVenderrportal.Sends
                        {
                            grno = tbUnitFormPayment.GRNO,
                            pono = tbUnitFormPayment.PONO,
                            remark = tbUnitFormPayment.Remark,
                            fileData = new List<IFormFile>()
                        };


                        var queryfileData = from t1 in _context.tr_Document
                                            join t2 in _context.tm_Resource
                                                on t1.ResourceID equals t2.ID into t2Group
                                            from t2 in t2Group.DefaultIfEmpty()
                                            where t1.UnitFormID == tbUnitFormPayment.UnitFormID
                                            select new
                                            {
                                                UnitFormPDF = t2.FilePath
                                            };

                        var queryQCfileData = from t1 in _context.tr_UnitForm
                                              join rawT2 in _context.tr_Form_QCCheckList on t1.FormID equals rawT2.FormID into t2Join
                                              from t2 in t2Join.DefaultIfEmpty()
                                              join rawT3 in _context.tr_QC_UnitCheckList.Where(x => x.UnitID == tbUnitFormPayment.UnitID && x.QCStatusID == 1) on t2.CheckListID equals rawT3.CheckListID into t3Join
                                              from t3 in t3Join.DefaultIfEmpty()
                                              join rawT4 in _context.tr_Document on t3.ID equals rawT4.QCUnitCheckListID into t4Join
                                              from t4 in t4Join.DefaultIfEmpty()
                                              join rawT5 in _context.tm_Resource on t4.ResourceID equals rawT5.ID into t5Join
                                              from t5 in t5Join.DefaultIfEmpty()
                                              where t1.UnitID == tbUnitFormPayment.UnitID && t1.ID == tbUnitFormPayment.UnitFormID

                                              select new
                                              {
                                                  UnitQCPDF = t5.FilePath
                                              };

                        foreach (var item in queryfileData)
                        {
                            if (!string.IsNullOrEmpty(item.UnitFormPDF))
                            {
                                string fullPath = Path.Combine(Model.ApplicationPath, item.UnitFormPDF);

                                if (File.Exists(fullPath))
                                {
                                    IFormFile file = FormatExtension.CreateFormFileFromPath(fullPath);
                                    request.fileData.Add(file);
                                }
                                else
                                {
                                    // Handle the case if file does not exist
                                    // Optionally log or skip
                                }
                            }
                        }

                        foreach (var item in queryQCfileData)
                        {
                            if (!string.IsNullOrEmpty(item.UnitQCPDF))
                            {
                                string fullPath = Path.Combine(Model.ApplicationPath, item.UnitQCPDF);

                                if (File.Exists(fullPath))
                                {
                                    IFormFile file = FormatExtension.CreateFormFileFromPath(fullPath);
                                    request.fileData.Add(file);
                                }
                            }
                        }

                        var apiResponse = _VenderrportalService.UploadFileAsync(request).GetAwaiter().GetResult();

                        int syncStatusID;

                        if (apiResponse.status == 200)
                        {
                            syncStatusID = 31; // success
                        }
                        else
                        {
                            syncStatusID = 32; // failure

                        }

                        tbUnitFormPayment.SyncStatusID = syncStatusID;
                        tbUnitFormPayment.SyncMessage = apiResponse.message;
                        tbUnitFormPayment.UpdateDate = DateTime.Now;
                        tbUnitFormPayment.UpdateBy = Model.UserID;
                        _context.tr_UnitFormPayment.Update(tbUnitFormPayment);

                        returnUrlDoc = syncStatusID;
                    }

                    _context.SaveChanges();
                   
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
