using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Project.ConstructionTracking.Web.Models.ProjectFormModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class ProjectFormRepo : IProjectFormRepo
    {
        private readonly ContructionTrackingDbContext _context;
        public ProjectFormRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }
        public List<ProjectFormModel.ProjectForm_getForm> GetFormCheckUnitList(int formID)
        {
            var query = from t1 in _context.tr_ProjectForm
                        where t1.ID == formID && t1.FormTypeID == SystemConstant.Ext.FORM_TYPE_PE && t1.FormRefID == null
                        join t2 in _context.tr_ProjectFormGroup on t1.ID equals t2.FormID into _t2Group
                        from t2Group in _t2Group.DefaultIfEmpty()
                        join t3 in _context.tr_ProjectFormPackage on t2Group.ID equals t3.GroupID into _t3Group
                        from t3Group in _t3Group.DefaultIfEmpty()
                        join t4 in _context.tr_ProjectFormCheckList on t3Group.ID equals t4.PackageID into _t4Group
                        from t4Group in _t4Group.DefaultIfEmpty()
                        select new
                        {
                            t1,
                            t2Group,
                            t3Group,
                            t4Group
                        };

            var queryResult = query.ToList();

            var results = new List<ProjectFormModel.ProjectForm_getForm>();

            foreach (var item in queryResult)
            {
                var existingForm = results.FirstOrDefault(f => f.FormID == item.t1.ID);
                if (existingForm == null)
                {
                    existingForm = new ProjectFormModel.ProjectForm_getForm
                    {
                        FormID = item.t1.ID,
                        FormName = item.t1.Name,
                        ListGroups = new List<ProjectFormModel.ProjectForm_getListGroups>()
                    };
                    results.Add(existingForm);
                }

                if (item.t2Group != null)
                {
                    var existingGroup = existingForm.ListGroups.FirstOrDefault(g => g.GroupID == item.t2Group.ID);
                    if (existingGroup == null)
                    {
                        existingGroup = new ProjectFormModel.ProjectForm_getListGroups
                        {
                            GroupID = item.t2Group.ID,
                            GroupName = item.t2Group.Name,
                            ListPackages = new List<ProjectFormModel.ProjectForm_getListPackages>()
                        };
                        existingForm.ListGroups.Add(existingGroup);
                    }

                    if (item.t3Group != null)
                    {
                        var existingPackage = existingGroup.ListPackages.FirstOrDefault(p => p.PackagesID == item.t3Group.ID);
                        if (existingPackage == null)
                        {
                            existingPackage = new ProjectFormModel.ProjectForm_getListPackages
                            {
                                PackagesID = item.t3Group.ID,
                                PackagesName = item.t3Group.Name,
                                ListCheckLists = new List<ProjectFormModel.ProjectForm_getListCheckLists>()
                            };
                            existingGroup.ListPackages.Add(existingPackage);
                        }

                        if (item.t4Group != null)
                        {
                            var existingCheckList = existingPackage.ListCheckLists.FirstOrDefault(c => c.CheckListID == item.t4Group.ID);
                            if (existingCheckList == null)
                            {
                                existingCheckList = new ProjectFormModel.ProjectForm_getListCheckLists
                                {
                                    CheckListID = item.t4Group.ID,
                                    CheckListName = item.t4Group.Name
                                };
                                existingPackage.ListCheckLists.Add(existingCheckList);
                            }
                        }
                    }
                }
            }

            return results;
        }

    }
}
