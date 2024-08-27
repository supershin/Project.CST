using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MCompanyModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IMasterCompanyRepo
	{
		dynamic GetCompanyList(DTParamModel param, MasterCompanyModel criteria);

        dynamic GetDetailCompanyVendor(int companyID);

        dynamic GetProjectList();

        dynamic GetListVendor(DTParamModel param, MasterCompanyModel criteria);

        dynamic CreateCompanyVendor(CreateCompanyVendorModel model);

        dynamic CompanyVendorMappingProject(CompanyMappingProjectModel model);

        dynamic CreateVendor(CreateVendorModel model);

        dynamic ActionVendor(ActionVendorModel model);
        dynamic ActionDeleteCompanyVendor(DeleteCompanyVendorModel model);
	}
	public class MasterCompanyRepo : IMasterCompanyRepo
	{
        private readonly ContructionTrackingDbContext _context;

        public MasterCompanyRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public dynamic GetCompanyList(DTParamModel param, MasterCompanyModel criteria)
		{
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            //variable = (condition) ? expressionTrue :  expressionFalse;
            criteria.strSearch = (criteria.strSearch == null) ? string.Empty : criteria.strSearch.ToString() ?? string.Empty;

            var query = from ft in _context.tm_CompanyVendor
                        where ft.FlagActive == true
                        select new
                        {
                            ft.ID,
                            ft.Name,
                            ft.UpdateDate
                        };

            if (!string.IsNullOrEmpty(criteria.strSearch))
            {
                query = query.Where(o =>
                    o.Name.Contains(criteria.strSearch)
                );
            }

            var result = query.Page(param.start, param.length, i => i.UpdateDate, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;
            return result.AsEnumerable().Select(e => new
            {
                ID = e.ID,
                Name = e.Name,
                UpdateDate = e.UpdateDate.ToStringDateTime()
            }).ToList();
        }

        public dynamic GetProjectList()
        {
            var query = (from p in _context.tm_Project
                         where p.FlagActive == true
                         select new
                         {
                             p.ProjectID,
                             p.ProjectName
                         }).ToList();

            return query;
        }

        public dynamic GetDetailCompanyVendor(int companyID)
        {
            var query = (from cv in _context.tm_CompanyVendor
                         where cv.ID == companyID && cv.FlagActive == true
                         select new
                         {
                             cv.ID,
                             cv.Name,
                             ProjectMapping = (from cp in _context.tr_CompanyVendorProject
                                               join mp in _context.tm_Project on cp.ProjectID equals mp.ProjectID
                                               where cp.CompanyVendorID == cv.ID && cp.FlagActive == true
                                               select new
                                               {
                                                   ProjectID = cp.ProjectID,
                                                   ProjectName = mp.ProjectName
                                               }).ToList()
                         }).FirstOrDefault();

            return query;
        }

        public dynamic GetListVendor(DTParamModel param, MasterCompanyModel criteria)
        {
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            //variable = (condition) ? expressionTrue :  expressionFalse;
            criteria.strSearch = (criteria.strSearch == null) ? string.Empty : criteria.strSearch.ToString() ?? string.Empty;

            var query = from cv in _context.tr_CompanyVendor
                        join mv in _context.tm_Vendor on cv.VendorID equals mv.ID
                        where cv.CompanyVendorID == criteria.CompanyID && cv.FlagActive == true
                        select new
                        {
                            mv.ID,
                            mv.Name,
                            mv.Email,
                            mv.UpdateDate
                        };

            if (!string.IsNullOrEmpty(criteria.strSearch))
            {
                query = query.Where(o =>
                    o.Name.Contains(criteria.strSearch) ||
                    o.Email.Contains(criteria.strSearch)
                );
            }

            var result = query.Page(param.start, param.length, i => i.UpdateDate, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;
            return result.AsEnumerable().Select(e => new
            {
                ID = e.ID,
                Name = e.Name,
                Email = e.Email,
                UpdateDate = e.UpdateDate.ToStringDateTime()
            }).ToList();
        }

        public dynamic CreateCompanyVendor(CreateCompanyVendorModel model)
        {
            tm_CompanyVendor create = new tm_CompanyVendor();
            create.Name = model.CompanyVendorName;
            create.FlagActive = true;
            create.CreateDate = DateTime.Now;
            create.UpdateDate = DateTime.Now;
            //create.CreateBy =;
            //create.UpdateBy =;

            _context.tm_CompanyVendor.Add(create);
            _context.SaveChanges();

            return create;
        }

        public dynamic CompanyVendorMappingProject(CompanyMappingProjectModel model)
        {
            // Fetch the company vendor
            tm_CompanyVendor? company = _context.tm_CompanyVendor
                .Where(o => o.ID == model.CompanyVendorID && o.FlagActive == true)
                .FirstOrDefault();

            if (company == null) throw new Exception("ไม่พบข้อมูลของ Company Vendor");

            // Fetch all related projects
            List<tr_CompanyVendorProject> listProject = _context.tr_CompanyVendorProject
                .Where(o => o.CompanyVendorID == company.ID)
                .ToList();

            // Separate active and inactive project IDs
            List<Guid?> activeList = listProject
                .Where(o => o.FlagActive == true)
                .Select(o => o.ProjectID)
                .ToList();

            List<Guid?> inActiveList = listProject
                .Where(o => o.FlagActive == false)
                .Select(o => o.ProjectID)
                .ToList();

            List<Guid?> newList = null;

            if (listProject.Count > 0)
            {
                if (model.ProJectIDList == null)
                {
                    // If no projects are selected, deactivate all active projects
                    UpdateMapping(company.ID, activeList, false);
                }
                else
                {
                    // Calculate new projects to be added
                    newList = model.ProJectIDList.Except(activeList).Except(inActiveList).ToList();
                    if (newList.Any())
                    {
                        CreateMapping(company.ID, newList);
                    }

                    // Deactivate projects that are no longer selected
                    var projectsToDeactivate = activeList.Except(model.ProJectIDList).ToList();
                    if (projectsToDeactivate.Any())
                    {
                        UpdateMapping(company.ID, projectsToDeactivate, false);
                    }

                    // Activate inactive projects that are now selected
                    var projectsToActivate = inActiveList.Intersect(model.ProJectIDList).ToList();
                    if (projectsToActivate.Any())
                    {
                        UpdateMapping(company.ID, projectsToActivate, true);
                    }
                }
            }
            else
            {
                // If there are no existing projects, create mappings for all selected projects
                if (model.ProJectIDList != null && model.ProJectIDList.Any())
                {
                    CreateMapping(company.ID, model.ProJectIDList);
                }
            }

            // requery for get resp
            var resp = (from cv in _context.tm_CompanyVendor
                        where cv.ID == model.CompanyVendorID && cv.FlagActive == true
                        select new
                        {
                            cv.ID,
                            cv.Name,
                            ProjectMapping = (from cp in _context.tr_CompanyVendorProject
                                              join mp in _context.tm_Project on cp.ProjectID equals mp.ProjectID
                                              where cp.CompanyVendorID == cv.ID && cp.FlagActive == true
                                              select new
                                              {
                                                  ProjectID = cp.ProjectID,
                                                  ProjectName = mp.ProjectName
                                              }).ToList()
                        }).FirstOrDefault();
                        
            return resp;
        }
        private void CreateMapping(int companyID, List<Guid?> projectLists)
        {
            if(projectLists != null && projectLists.Count > 0)
            {
                List<tr_CompanyVendorProject> listCreate = new List<tr_CompanyVendorProject>();

                foreach(var create in projectLists)
                {
                    tr_CompanyVendorProject createData = new tr_CompanyVendorProject();
                    createData.CompanyVendorID = companyID;
                    createData.ProjectID = create;
                    createData.FlagActive = true;
                    createData.CreateDate = DateTime.Now;
                    createData.UpdateDate = DateTime.Now;
                    //createData.CreateBy = ;
                    //createData.UpdateBy = ;
                    listCreate.Add(createData);
                }

                _context.tr_CompanyVendorProject.AddRange(listCreate);
                _context.SaveChanges();
            }
        }

        private void UpdateMapping(int companyID, List<Guid?> projectLists, bool flag)
        {
            if (projectLists != null && projectLists.Count > 0)
            {
                List<tr_CompanyVendorProject> listUpdate = new List<tr_CompanyVendorProject>();

                foreach (var create in projectLists)
                {
                    tr_CompanyVendorProject? updateData = _context.tr_CompanyVendorProject
                                                        .Where(o => o.CompanyVendorID == companyID
                                                        && o.ProjectID == create).FirstOrDefault();

                    updateData.FlagActive = flag;
                    updateData.UpdateDate = DateTime.Now;
                    //updateData.UpdateBy = ;

                    listUpdate.Add(updateData);
                }

                _context.tr_CompanyVendorProject.UpdateRange(listUpdate);
                _context.SaveChanges();
            }
        }

        public dynamic CreateVendor(CreateVendorModel model)
        {
            tm_Vendor create = new tm_Vendor();
            create.Name = model.Name;
            create.Email = model.Email;
            create.FlagActive = true;
            create.CreateDate = DateTime.Now;
            create.UpdateDate = DateTime.Now;
            //create.CreateBy =;
            //create.UpdateBy =;

            _context.tm_Vendor.Add(create);
            _context.SaveChanges();

            tr_CompanyVendor mapping = new tr_CompanyVendor();
            mapping.CompanyVendorID = model.CompanyVendorID;
            mapping.VendorID = create.ID;
            mapping.FlagActive = true;
            mapping.CreateDate = DateTime.Now;
            mapping.UpdateDate = DateTime.Now;
            //create.CreateBy =;
            //create.UpdateBy =;

            _context.tr_CompanyVendor.Add(mapping);
            _context.SaveChanges();

            var resp = new
            {
                VendorID = create.ID,
                VendorName = create.Name,
                VendorEmail = create.Email,
                CompanyVendorID = mapping.CompanyVendorID
            };

            return resp;
        }

        public dynamic ActionVendor(ActionVendorModel model)
        {
            
            // detail data
            if (String.IsNullOrWhiteSpace(model.Name) || String.IsNullOrWhiteSpace(model.Email))
            {
                tm_Vendor? detail = _context.tm_Vendor
                                        .Where(o => o.ID == model.VendorID && o.FlagActive == true)
                                        .FirstOrDefault();
                if (detail == null) throw new Exception("ไม่พบข้อมูลของ Vendor");

                var resp = new
                {
                    VendorID = detail.ID,
                    Name = detail.Name,
                    Email = detail.Email
                };

                return resp;
            }
            // edit data
            else
            {
                tm_Vendor? edit = _context.tm_Vendor
                                        .Where(o => o.ID == model.VendorID && o.FlagActive == true)
                                        .FirstOrDefault();
                if (edit == null) throw new Exception("ไม่พบข้อมูลของ Vendor");
                edit.Name = model.Name;
                edit.Email = model.Email;
                edit.UpdateDate = DateTime.Now;
                //edit.UpdateBy = ;

                _context.tm_Vendor.Update(edit);
                _context.SaveChanges();

                var resp = new
                {
                    VendorID = edit.ID,
                    Name = edit.Name,
                    Email = edit.Email
                };

                return resp;
            }
        }

        public dynamic ActionDeleteCompanyVendor(DeleteCompanyVendorModel model)
        {
            if(model.CompanyID != null)
            {
                tm_CompanyVendor? delete = _context.tm_CompanyVendor
                                        .Where(o => o.ID == model.CompanyID && o.FlagActive == true)
                                        .FirstOrDefault();
                if (delete == null) throw new Exception("ไม่พบข้อมูลของ CompanyVendor");

                delete.FlagActive = false;
                delete.UpdateDate = DateTime.Now;
                //delete.UpdateBy = ;

                _context.tm_CompanyVendor.Update(delete);

                List<tr_CompanyVendor> deleteMapping = _context.tr_CompanyVendor
                                                .Where(o => o.CompanyVendorID == delete.ID && o.FlagActive == true)
                                                .ToList();

                foreach(var data in deleteMapping)
                {
                    data.FlagActive = false;
                    data.UpdateDate = DateTime.Now;
                    //data.UpdateBy = ;

                    tm_Vendor? vendor = _context.tm_Vendor
                                        .Where(o => o.ID == data.VendorID && o.FlagActive == true)
                                        .FirstOrDefault();
                    if (vendor == null) throw new Exception("ไม่พบข้อมูลของ Vendor");

                    vendor.FlagActive = false;
                    vendor.UpdateDate = DateTime.Now;
                    //vendor.UpdateBy = ;

                    _context.tm_Vendor.Update(vendor);
                }

                List<tr_CompanyVendorProject> deleteMappingP = _context.tr_CompanyVendorProject
                                                                .Where(o => o.CompanyVendorID == delete.ID
                                                                && o.FlagActive == true)
                                                                .ToList();
                foreach (var data2 in deleteMappingP)
                {
                    data2.FlagActive = false;
                    data2.UpdateDate = DateTime.Now;
                    //data.UpdateBy = ;
                }

                _context.SaveChanges();

                var resp = new
                {
                    CompanyID = delete.ID,
                    VendorID = model.VendorID
                };

                return resp;
            }
            else
            {
                tm_Vendor? delete = _context.tm_Vendor
                                    .Where(o => o.ID == model.VendorID && o.FlagActive == true)
                                    .FirstOrDefault();
                if (delete == null) throw new Exception("ไม่พบข้อมูลของ Vendor");

                delete.FlagActive = false;
                delete.UpdateDate = DateTime.Now;
                //delete.UpdateBy = ;

                _context.tm_Vendor.Update(delete);

                tr_CompanyVendor? deleteMapping = _context.tr_CompanyVendor
                                                .Where(o => o.VendorID == delete.ID && o.FlagActive == true)
                                                .FirstOrDefault();
                if (delete == null) throw new Exception("ไม่พบข้อมูล Mapping ของ CompanyVendor");

                deleteMapping.FlagActive = false;
                deleteMapping.UpdateDate = DateTime.Now;
                //deleteMapping.UpdateBy = ;

                _context.tr_CompanyVendor.Update(deleteMapping);
                _context.SaveChanges();

                var resp = new
                {
                    CompanyID = model.CompanyID,
                    VendorID = delete.ID
                };

                return resp;
            }
        }
    }
    
}

