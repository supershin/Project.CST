using System;
using Microsoft.CodeAnalysis;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models.GeneratePDFModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IGeneratePDFRepo
	{
	    dynamic GetDataToGeneratePDF(DataToGenerateModel model);

        string GenerateDocumentNO(Guid projectID);

        bool SaveFileDocument();
    }

	public class GeneratePDFRepo : IGeneratePDFRepo
	{
        private readonly ContructionTrackingDbContext _context;
        public GeneratePDFRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

        public dynamic GetDataToGeneratePDF(DataToGenerateModel model)
		{
            var query = (from truf in _context.tr_UnitForm
                         join tmp in _context.tm_Project on truf.ProjectID equals tmp.ProjectID
                         join tmu in _context.tm_Unit on truf.UnitID equals tmu.UnitID
                         join tmv in _context.tm_Vendor on truf.VendorID equals tmv.ID
                         join tmf in _context.tm_Form on truf.FormID equals tmf.ID
                         where truf.ProjectID == model.ProjectID
                            && truf.UnitID == model.UnitID
                            && truf.FormID == model.FormID
                            && truf.FlagActive == true
                         select new
                         {
                             // Header
                             ProjectName = tmp.ProjectName,
                             UnitCode = tmu.UnitCode,
                             VendorName = tmv.Name,
                             FormName = tmf.Name,
                             FormDesc = tmf.Description,

                             // Worker Data
                             WorkerData = (from trufa in _context.tr_UnitFormAction
                                           join tmus in _context.tm_User on trufa.UpdateBy equals tmus.ID
                                           where trufa.UnitFormID == truf.ID
                                              && trufa.RoleID != SystemConstant.UserRole.PJM
                                              && trufa.ActionType == "submit"
                                           select new
                                           {
                                               RoleID = tmus.RoleID,
                                               FullName = tmus.FirstName + " " + tmus.LastName,
                                               UpdateDate = trufa.UpdateDate.ToStringDateTime()
                                           }).ToList(),

                             // Data Checklist
                             DataCheckList = (from tmfg in _context.tm_FormGroup
                                              where tmfg.FormID == tmf.ID && tmfg.FlagActive == true
                                              select new
                                              {
                                                  FormGroupName = tmfg.Name,
                                                  PackageList = (from trufp in _context.tr_UnitFormPackage
                                                                 join tmfp in _context.tm_FormPackage on trufp.PackageID equals tmfp.ID
                                                                 where trufp.UnitFormID == truf.ID
                                                                    && trufp.FormID == tmf.ID
                                                                    && trufp.GroupID == tmfg.ID
                                                                 select new
                                                                 {
                                                                     PackageName = tmfp.Name,
                                                                     PackageDesc = trufp.Remark,
                                                                     CheckList = (from trufcl in _context.tr_UnitFormCheckList
                                                                                  join tmfcl in _context.tm_FormCheckList on trufcl.CheckListID equals tmfcl.ID
                                                                                  where trufcl.UnitFormID == truf.ID
                                                                                     && trufcl.FormID == tmf.ID
                                                                                     && trufcl.GroupID == tmfg.ID
                                                                                     && trufcl.PackageID == tmfp.ID
                                                                                     && (trufcl.StatusID == 9 || trufcl.StatusID == 11)
                                                                                  select new
                                                                                  {
                                                                                      CheckListName = tmfcl.Name,
                                                                                      CheckListStatus = trufcl.StatusID
                                                                                  }).ToList()
                                                                 }).ToList(),

                                                  ImageCheckList = (from trufr in _context.tr_UnitFormResource
                                                                    join tmr in _context.tm_Resource on trufr.ResourceID equals tmr.ID
                                                                    where trufr.GroupID == tmfg.ID
                                                                       && trufr.RoleID == SystemConstant.UserRole.PE
                                                                    select new
                                                                    {
                                                                        ImagePath = tmr.FilePath
                                                                    }).ToList()
                                              }).ToList(),

                             // Signatures
                             SignPE = (from trufa in _context.tr_UnitFormAction
                                       join trur in _context.tr_UserResource on trufa.UpdateBy equals trur.UserID
                                       join image in _context.tm_Resource on trur.ResourceID equals image.ID
                                       where trufa.UnitFormID == truf.ID && trufa.RoleID == SystemConstant.UserRole.PE
                                       && trufa.ActionType == "submit" && trufa.StatusID == 1
                                       select image.FilePath).FirstOrDefault(),

                             SignPM = (from trufa in _context.tr_UnitFormAction
                                       join trur in _context.tr_UserResource on trufa.UpdateBy equals trur.UserID
                                       join image in _context.tm_Resource on trur.ResourceID equals image.ID
                                       where trufa.UnitFormID == truf.ID && trufa.RoleID == SystemConstant.UserRole.PM
                                       && trufa.ActionType == "submit" && trufa.StatusID == 4
                                       select image.FilePath).FirstOrDefault(),

                             SignVendor = (from tr in _context.tm_Resource
                                           where tr.ID == truf.VendorResourceID && tr.FlagActive == true
                                           select tr.FilePath).FirstOrDefault()
                         }).FirstOrDefault();

            return query;
		}

        public string GenerateDocumentNO(Guid projectID)
        {
            tr_Document? document = _context.tr_Document.Where(o => o.FlagActive == true)
                                    .OrderByDescending(o => o.UpdateDate)
                                    .FirstOrDefault();

            tm_Project? project = _context.tm_Project
                                .Where(o => o.ProjectID == projectID && o.FlagActive == true)
                                .FirstOrDefault();

            string documentPrefix = "";

            if (project != null)
            {
                documentPrefix = "C" + project.ProjectCode;
            }

            int documentRunning;

            if (document == null)
            {
                documentRunning = 0;
            }
            else
            {
                documentRunning = Int32.Parse(document.DocuementRunning);
            }

            string formatString = HashExtension.GenerateApproveNumber(documentRunning, documentPrefix);
            return formatString;
        }
    }
}

