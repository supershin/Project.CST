using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.CodeAnalysis.Differencing;
using System.Xml.Linq;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MCompanyModel;
using Project.ConstructionTracking.Web.Models.MFormModel;
using Project.ConstructionTracking.Web.Models.MProjectModel;
using Project.ConstructionTracking.Web.Repositories;

namespace Project.ConstructionTracking.Web.Services
{
	public interface IMasterCompanyService
	{
		dynamic GetCompanyList(DTParamModel param, MasterCompanyModel criteria);

		DetailCompanyModel GetDetailCompanyVendor(int companyID);

        dynamic GetVendorList(DTParamModel param, MasterCompanyModel criteria);

        CompanyVendorResp CreateCompanyVendor(CreateCompanyVendorModel model);

        DetailCompanyModel CompanyVendorMappingProject(CompanyMappingProjectModel model);

        CreateVendorResp CreateVendor(CreateVendorModel model);

        ActionVendorResp ActionVendor(ActionVendorModel model);

        DeleteCompanyVendorResp ActionDeleteCompanyVendor(DeleteCompanyVendorModel model);
    } 

	public class MasterCompanyService : IMasterCompanyService
	{
		private readonly IMasterCompanyRepo _masterCompanyRepo;

        public MasterCompanyService(IMasterCompanyRepo masterCompanyRepo)
		{
			_masterCompanyRepo = masterCompanyRepo;
		}

		public dynamic GetCompanyList(DTParamModel param, MasterCompanyModel criteria)
		{
			var query = _masterCompanyRepo.GetCompanyList(param, criteria);

			return query;
		}

        public DetailCompanyModel GetDetailCompanyVendor(int companyID)
		{
            var query = _masterCompanyRepo.GetDetailCompanyVendor(companyID);

            DetailCompanyModel detail = new DetailCompanyModel
            {
                CompanyID = query.ID,
                CompanyName = query.Name,
                ProjectMappings = new List<ProjectMapping>(), // Initialize the list
                ProjectLists = new List<Projects>() // Initialize the list
            };

            // Populate the ProjectMappings list
            foreach (var data in query.ProjectMapping)
            {
                ProjectMapping mapping = new ProjectMapping
                {
                    ProjectID = data.ProjectID,
                    ProjectName = data.ProjectName
                };

                detail.ProjectMappings.Add(mapping);
            }

            // Populate the ProjectLists list
            var query2 = _masterCompanyRepo.GetProjectList();

            foreach (var data in query2)
            {
                Projects project = new Projects
                {
                    ProjectID = data.ProjectID,
                    ProjectName = data.ProjectName
                };

                detail.ProjectLists.Add(project);
            }

            return detail;
		}

        public dynamic GetVendorList(DTParamModel param, MasterCompanyModel criteria)
        {
            var query = _masterCompanyRepo.GetListVendor(param, criteria);

            return query;
        }

        public CompanyVendorResp CreateCompanyVendor(CreateCompanyVendorModel model)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var query = _masterCompanyRepo.CreateCompanyVendor(model);

                    CompanyVendorResp create = new CompanyVendorResp()
                    {
                        ID = query.ID,
                        CompanyVendorName = query.Name,
                        FlagActive = query.FlagActive
                    };

                    scope.Complete();
                    return create;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    scope.Dispose();
                }

            }
            
        }

        public DetailCompanyModel CompanyVendorMappingProject(CompanyMappingProjectModel model)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var query = _masterCompanyRepo.CompanyVendorMappingProject(model);

                    DetailCompanyModel resp = new DetailCompanyModel()
                    {
                        CompanyID = query.ID,
                        CompanyName = query.Name,
                        ProjectMappings = new List<ProjectMapping>(), // Initialize the list
                        ProjectLists = new List<Projects>() // Initialize the list
                    };

                    // Populate the ProjectMappings list
                    foreach (var data in query.ProjectMapping)
                    {
                        ProjectMapping mapping = new ProjectMapping
                        {
                            ProjectID = data.ProjectID,
                            ProjectName = data.ProjectName
                        };

                        resp.ProjectMappings.Add(mapping);
                    }

                    // Populate the ProjectLists list
                    var query2 = _masterCompanyRepo.GetProjectList();

                    foreach (var data in query2)
                    {
                        Projects project = new Projects
                        {
                            ProjectID = data.ProjectID,
                            ProjectName = data.ProjectName
                        };

                        resp.ProjectLists.Add(project);
                    }

                    scope.Complete();
                    return resp;
                }
                catch (Exception ex)
                    {
                    throw ex;
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        public CreateVendorResp CreateVendor(CreateVendorModel model)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var query = _masterCompanyRepo.CreateVendor(model);

                    CreateVendorResp resp = new CreateVendorResp()
                    {
                        VendorID = query.VendorID,
                        Name = query.VendorName,
                        Email = query.VendorEmail,
                        CompanyVendorID = query.CompanyVendorID
                    };

                    scope.Complete();
                    return resp;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        public ActionVendorResp ActionVendor(ActionVendorModel model)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var query = _masterCompanyRepo.ActionVendor(model);

                    ActionVendorResp resp = new ActionVendorResp()
                    {
                        VendorID = query.VendorID,
                        Name = query.Name,
                        Email = query.Email
                    };

                    scope.Complete();
                    return resp;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        public DeleteCompanyVendorResp ActionDeleteCompanyVendor(DeleteCompanyVendorModel model)
        {
            TransactionOptions option = new TransactionOptions();
            option.Timeout = new TimeSpan(1, 0, 0);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                try
                {
                    var query = _masterCompanyRepo.ActionDeleteCompanyVendor(model);

                    DeleteCompanyVendorResp resp = new DeleteCompanyVendorResp()
                    {
                        CompanyID = query.CompanyID != null ? query.CompanyID : null,
                        VendorID = query.VendorID != null ? query.VendorID : null
                    };

                    scope.Complete();
                    return resp;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }
    }
}

