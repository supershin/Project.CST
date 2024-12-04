using DocumentFormat.OpenXml.InkML;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using QuestPDF.Infrastructure;
using System.Linq;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class GetDDLRepo : IGetDDLRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public GetDDLRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<GetDDL> GetDDLList(GetDDL Model)
        {
            switch (Model.Act)
            {
                case "Ext":
                    var extQuery = from ext in _context.tm_Ext
                                   where ext.ExtTypeID == Model.ID && ext.FlagActive == true
                                   orderby ext.LineOrder
                                   select new GetDDL
                                   {
                                       Value = ext.ID,
                                       Text = ext.Name
                                   };

                    return extQuery.ToList();

                case "Vender":
                    var result = from t1 in _context.tm_CompanyVendor
                                 join t2 in _context.tr_CompanyVendor
                                     on new { CompanyVendorID = (int?)t1.ID, FlagActive = (bool?)true } equals new { t2.CompanyVendorID, t2.FlagActive } into gj1
                                 from t2 in gj1.DefaultIfEmpty()
                                 join t3 in _context.tm_Vendor
                                     on new { t2.VendorID, FlagActive = (bool?)true } equals new { VendorID = (int?)t3.ID, t3.FlagActive } into gj2
                                 from t3 in gj2.DefaultIfEmpty()
                                 where t1.ID == Model.ID && t1.FlagActive == true
                                 orderby t2.VendorID
                                 select new GetDDL
                                 {
                                     Value = t3.ID,
                                     Text = t3.Name
                                 };

                    return result.ToList();

                case "Project":
                    var ListProject = from t1 in _context.tm_Project
                                      join t2 in _context.tr_ProjectPermission
                                        on t1.ProjectID equals t2.ProjectID into joined
                                      from t2 in joined.DefaultIfEmpty()
                                      where t1.FlagActive == true && t2.FlagActive == true && t2.UserID == Model.UserID
                                      select new GetDDL
                                      {
                                          ValueGuid = t1.ProjectID,
                                          Text = t1.ProjectName
                                      };

                    return ListProject.ToList();

                case "Unit":
                    var ListUnit = from tu in _context.tm_Unit
                                   where tu.FlagActive == true && tu.ProjectID == Model.ValueGuid
                                   orderby tu.UnitCode
                                   select new GetDDL
                                   {
                                       ValueGuid = tu.UnitID,
                                       Text = tu.UnitCode
                                   };

                    return ListUnit.ToList();

                case "UnitFormStatus":
                    var ListUnitFormStatus = from tu in _context.tm_UnitFormStatus
                                             where tu.FlagActive == true && tu.ID > 1
                                             orderby tu.LineOrder
                                             select new GetDDL
                                             {
                                                 Value = tu.ID,
                                                 Text = tu.Name
                                             };

                    return ListUnitFormStatus.ToList();

                case "UserName":
                    var UserName = from us in _context.tm_User
                                   where us.ID == Model.ValueGuid
                                   select new GetDDL
                                   {
                                       Text = us.FirstName + " " + us.LastName
                                   };

                    return UserName.ToList();

                case "ProjectAdmin":
                    //var ListProjectAdmint = from t1 in _context.tm_Project
                    //                        where t1.FlagActive == true
                    //                        select new GetDDL
                    //                        {
                    //                            ValueGuid = t1.ProjectID,
                    //                            Text = t1.ProjectName
                    //                        };

                    //return ListProjectAdmint.ToList();

                    var ListProjectAdmint = from t1 in _context.tm_Project
                                            where t1.FlagActive == true &&
                                                  (Model.GuID == null || t1.ProjectID == Model.GuID)
                                            select new GetDDL
                                            {
                                                ValueGuid = t1.ProjectID,
                                                Text = t1.ProjectName
                                            };

                    return ListProjectAdmint.ToList();

                case "DefectArea":
                    var ListDefectArea = from t1 in _context.tm_DefectArea
                                         where t1.FlagActive == true
                                               && t1.ProjectTypeID == Model.ID
                                               && (string.IsNullOrEmpty(Model.searchTerm) || t1.Name.Contains(Model.searchTerm))
                                         orderby t1.Name
                                         select new GetDDL
                                         {
                                             Value = t1.ID,
                                             Text = t1.Name
                                         };

                    return ListDefectArea.ToList();

                case "DefectType":
                    var ListDefectType = from t1 in _context.tm_DefectType
                                         join t2 in _context.tm_DefectAreaType_Mapping
                                         on t1.ID equals t2.DefectTypeID into mappingGroup
                                         from t2 in mappingGroup.DefaultIfEmpty() // Left join
                                         where t1.FlagActive == 1
                                               && t2.DefectAreaID == Model.ID
                                               && (string.IsNullOrEmpty(Model.searchTerm) || t1.Name.Contains(Model.searchTerm))
                                         orderby t1.Name
                                         select new GetDDL
                                         {
                                             Value = t1.ID,
                                             Text = t1.Name
                                         };

                    return ListDefectType.ToList();

                case "DefectDescription":
                    var ListDefectDescription = from t1 in _context.tm_DefectDescription
                                                where t1.FlagActive == 1
                                                      && t1.DefectTypeID == Model.ID
                                                      && (string.IsNullOrEmpty(Model.searchTerm) || t1.Name.Contains(Model.searchTerm))
                                                orderby t1.LineOrder
                                                select new GetDDL
                                                {
                                                    Value = t1.ID,
                                                    Text = t1.Name
                                                };

                    return ListDefectDescription.ToList();

                case "PEUnit":
                    var PEUnit = from t1 in _context.tr_PE_Unit
                                 join t2 in _context.tm_User on t1.UserID equals t2.ID into joined
                                 from t2 in joined.DefaultIfEmpty()
                                 where t1.UnitID == Model.GuID

                                 select new GetDDL
                                 {
                                     ValueGuid = t1.UserID,
                                     Text = t2.FirstName + " " + t2.LastName
                                 };

                    return PEUnit.ToList();

                case "ImageQC5Unit":
                    var ImageQC5UnitList = from t1 in _context.tr_QC_UnitCheckList_Resource
                                           join t2 in _context.tm_Resource on t1.ResourceID equals t2.ID into joined
                                           from t2 in joined.DefaultIfEmpty()
                                           where t1.QCUnitCheckListID == Model.GuID && t1.DefectID == null && t1.IsSign == false && t1.FlagActive == true && t2.FlagActive == true
                                           select new GetDDL
                                           {
                                               ValueGuid = t1.ResourceID,
                                               Text = t2.FilePath
                                           };

                    return ImageQC5UnitList.ToList();

                case "GetVenderSign":
                    var GetVenderSign = from ure in _context.tr_UnitFormResource
                                        join t2 in _context.tm_Resource on ure.ResourceID equals t2.ID into joined
                                        from t2 in joined.DefaultIfEmpty()
                                        where ure.UnitFormID == Model.GuID && ure.PassConditionID == null && ure.RoleID == 1 && ure.FormID == Model.ID
                                        orderby ure.CreateDate descending
                                        select new GetDDL
                                        {
                                            ValueGuid = ure.ResourceID,
                                            Text = t2.FilePath
                                        };
                    return GetVenderSign.ToList();

                case "GetUnitFornPDF":
                    var GetUnitFornPDF = from t1 in _context.tr_Document
                                         join t2 in _context.tm_Resource on t1.ResourceID equals t2.ID into joined
                                         from t2 in joined.DefaultIfEmpty()
                                         where t1.UnitFormID == Model.GuID && t1.QCUnitCheckListID == null 
                                         select new GetDDL
                                         {
                                            ValueGuid = t1.ResourceID,
                                            Text = t2.FilePath
                                         };
                    return GetUnitFornPDF.ToList();

                case "GetCheckQCPass":
                    var maxSeq = _context.tr_QC_UnitCheckList
                        .Where(t1 => t1.UnitID == Model.GuID)
                        .Max(t1 => t1.Seq);

                    var GetCheckQCPass = from t1 in _context.tr_QC_UnitCheckList
                                         join t2 in _context.tr_QC_UnitCheckList_Defect
                                         on t1.ID equals t2.QCUnitCheckListID into joined
                                         from t2 in joined.DefaultIfEmpty()
                                         where t1.UnitID == Model.GuID && t1.Seq == maxSeq && (t2 == null || t2.FlagActive == true)
                                         select new GetDDL
                                         {
                                             Value = t1.QCStatusID,
                                         };

                return GetCheckQCPass.ToList();

                case "GetListUnitPass":

                    var GetListUnitPass = (from t1 in _context.tm_Unit
                                  join t2 in _context.tm_Project on t1.ProjectID equals t2.ProjectID into projectJoin
                                  from t2 in projectJoin.DefaultIfEmpty() // LEFT JOIN equivalent for project
                                  join t4 in _context.tr_UnitForm on new { t1.ProjectID, UnitID = (Guid?)t1.UnitID } equals new { t4.ProjectID, t4.UnitID } into unitFormJoin
                                  from t4 in unitFormJoin.DefaultIfEmpty() // LEFT JOIN equivalent for unit form
                                  join t5 in _context.tr_Form_QCCheckList on t4.FormID equals t5.FormID into formQCCheckJoin
                                  from t5 in formQCCheckJoin.DefaultIfEmpty() // LEFT JOIN equivalent for form QC check
                                  join tqcMax in (
                                      from tqc in _context.tr_QC_UnitCheckList
                                      group tqc by new { tqc.ID, tqc.UnitID, tqc.CheckListID, tqc.QCStatusID } into grouped
                                      select new
                                      {
                                          MaxSeq = grouped.Max(x => x.Seq),
                                          grouped.Key.ID,
                                          grouped.Key.UnitID,
                                          grouped.Key.CheckListID,
                                          grouped.Key.QCStatusID
                                      }
                                  ) on new { UnitID = (Guid?)t1.UnitID, t5.CheckListID } equals new { tqcMax.UnitID, tqcMax.CheckListID } into tqcMaxJoin
                                  from tqcMax in tqcMaxJoin.DefaultIfEmpty() // LEFT JOIN equivalent for max sequence subquery
                                  where t1.ProjectID == Model.GuID
                                        && new[] { 4, 11 }.Contains(t4.StatusID ?? 0) // Handling nullable StatusID
                                        && new[] { 1, 4 }.Contains(tqcMax.QCStatusID ?? 0) // Handling nullable QCStatusID
                                  orderby t1.UnitCode
                                  select new GetDDL
                                  {
                                      ValueGuid = t1.UnitID,
                                      Text = t1.UnitCode
                                  }).Distinct().ToList();


                return GetListUnitPass;

                case "GetListDDLStatusPCtext":

                    var GetListDDLStatusPCtext = (from t1 in _context.tr_RoleActionStatus
                                              where (string.IsNullOrEmpty(Model.searchTerm) || ("," + Model.searchTerm + ",").Contains("," + t1.ID.ToString() + ","))
                                                select new GetDDL
                                                {
                                                    Value = t1.ID,
                                                    Text = t1.Name
                                                })
                                 .Distinct()
                                 .ToList();



                 return GetListDDLStatusPCtext;

                case "GetListDDLUnitRpPC":

                    var GetListDDLUnitRpPC = (from t1 in _context.tr_UnitForm
                                              join t2 in _context.tm_Unit on t1.UnitID equals t2.UnitID into t2Group
                                              from t2 in t2Group.DefaultIfEmpty()
                                              where t1.ProjectID == Model.GuID
                                                select new GetDDL
                                                {
                                                    ValueGuid = t1.UnitID,
                                                    Text = t2 != null ? t2.UnitCode : null
                                                }).Distinct().ToList();

                return GetListDDLUnitRpPC;

                case "GetListDDLStatusRpPC":

                    var GetListDDLStatusRpPC = (from t1 in _context.tr_RoleActionStatus
                                                select new GetDDL
                                                {
                                                    Value = t1.ID,
                                                    Text = t1.Name
                                                }).ToList();
                return GetListDDLStatusRpPC;

                case "GetListUnitFormPayment":

                    var GetListUnitFormPayment = (from t1 in _context.tr_UnitFormPayment
                                                  where t1.UnitFormID == Model.GuID && t1.FlagActive == true
                                                  select new GetDDL
                                                  {
                                                      Valuedecimal = t1.PercentPayment,
                                                      Text = t1.SyncMessage
                                                  }).ToList();
                return GetListUnitFormPayment;

                case "GetListUnitFormPayment2":

                    var GetListUnitFormPayment2 = (from t1 in _context.tr_UnitFormPayment
                                                  where t1.UnitFormID == Model.GuID && t1.FlagActive == true && t1.SyncStatusID == SystemConstant.Ext.SyncSuccess
                                                   select new GetDDL
                                                  {
                                                      Valuedecimal = t1.PercentPayment,
                                                      Text = t1.SyncMessage
                                                  }).ToList();
                    return GetListUnitFormPayment2;

                case "GetUnitFormPayment":

                    var GetUnitFormPayment = (from t1 in _context.tr_UnitFormPayment
                                                  where t1.ID == Model.GuID && t1.FlagActive == true
                                                  select new GetDDL
                                                  {
                                                      Value = t1.SyncStatusID,
                                                      Text = t1.SyncMessage
                                                  }).ToList();
                    return GetUnitFormPayment;

                case "GetUnitFormPassCondition":

                    var GetUnitFormPassCondition = (from t1 in _context.tr_UnitFormPassCondition
                                                    where t1.UnitFormID == Model.GuID && t1.FlagActive == true
                                              select new GetDDL
                                              {
                                                  Value = t1.ID,
                                                  //Text = t1.SyncMessage
                                              }).ToList();
                    return GetUnitFormPassCondition;

                case "GetDetailCommentPMPJM":

                    var GetDetailCommentPMPJM = (
                                                    from t1 in _context.tr_UnitFormAction
                                                    join t2 in _context.tm_User on t1.UpdateBy equals t2.ID into userGroup
                                                    from t2 in userGroup.DefaultIfEmpty()
                                                    where t1.UnitFormID == Model.GuID
                                                          && t1.RoleID == Model.ID
                                                    select new GetDDL
                                                    {
                                                        Text = FormatExtension.FormatDateToDayMonthNameYearTime(t1.UpdateDate),
                                                        Text2 = t2.FirstName + " " + t2.LastName,
                                                        Text3 = t1.Remark
                                                    }).ToList();
                    return GetDetailCommentPMPJM;

                case "GetListDDLCompanyVenderInProject":

                    //var GetListDDLCompanyVenderInProject = _context.tm_Unit
                    //        .Where(t1 => t1.ProjectID == Model.GuID && t1.CompanyVendorID != null)
                    //        .Join(
                    //            _context.tm_CompanyVendor,
                    //            t1 => t1.CompanyVendorID,
                    //            t2 => t2.ID,
                    //            (t1, t2) => new GetDDL
                    //            {
                    //                Value = t1.CompanyVendorID,
                    //                Text = t2.Name
                    //            }
                    //        )
                    //        .Distinct()
                    //        .ToList();

                    //return GetListDDLCompanyVenderInProject;

                    var GetListDDLCompanyVenderInProject = _context.tm_Unit
                        .Where(t1 => t1.ProjectID == Model.GuID && t1.CompanyVendorID != null)
                        .Join(
                            _context.tm_CompanyVendor,
                            t1 => t1.CompanyVendorID,
                            t2 => t2.ID,
                            (t1, t2) => new { t1, t2 }
                        )
                        .Where(joined => Model.ID == null || joined.t2.ID == Model.ID) // Check if Model.ID is null or matches
                        .Select(joined => new GetDDL
                        {
                            Value = joined.t1.CompanyVendorID,
                            Text = joined.t2.Name
                        })
                        .Distinct()
                        .ToList();
                    return GetListDDLCompanyVenderInProject;

                default:

                return new List<GetDDL>();
            }
        }
    }
}
